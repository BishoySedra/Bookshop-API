using Moq;
using AutoMapper;
using API.Controllers;
using Core.Interfaces;
using Models.Entities;
using API.DTOs.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;

namespace API.Tests
{
    public class ProductControllerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ProductController _controller;

        public ProductControllerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _controller = new ProductController(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsListOfProducts()
        {
            var products = new List<Product> { new Product { Id = 1, Title = "Book1", Category = new Category { catName = "Fiction" } } };
            _unitOfWorkMock.Setup(u => u.Products.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Product, bool>>>())).ReturnsAsync(products);

            var result = await _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<ProductReadDTO>>(okResult.Value);
        }

        [Fact]
        public async Task GetById_ReturnsProduct_WhenFound()
        {
            var product = new Product { Id = 1, Title = "Test", Category = new Category { catName = "TestCat" } };
            _unitOfWorkMock.Setup(u => u.Products.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Product, bool>>>())).ReturnsAsync(new List<Product> { product });

            var result = await _controller.GetById(1);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.IsType<ProductReadDTO>(okResult.Value);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenNotFound()
        {
            _unitOfWorkMock.Setup(u => u.Products.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Product, bool>>>())).ReturnsAsync(new List<Product>());

            var result = await _controller.GetById(99);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_ReturnsCreatedProduct()
        {
            // Arrange
            var dto = new ProductCreateDTO { Title = "New Product", Description = "Description", Author = "Author", Price = 10, CategoryId = 1 };
            var entity = new Product { Id = 1, Title = "New Product", Description = "Description", Author = "Author", Price = 10, CategoryId = 1 };
            var readDto = new ProductReadDTO { Id = 1, Title = "New Product", Description = "Description", Author = "Author", Price = 10, CategoryId = 1, CategoryName = "Category" };

            var productRepoMock = new Mock<IProductRepository>();
            _unitOfWorkMock.Setup(u => u.Products).Returns(productRepoMock.Object);

            _mapperMock.Setup(m => m.Map<Product>(dto)).Returns(entity);
            _mapperMock.Setup(m => m.Map<ProductReadDTO>(entity)).Returns(readDto);

            productRepoMock.Setup(r => r.AddAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.IsType<ProductReadDTO>(createdResult.Value);
            Assert.Equal("New Product", ((ProductReadDTO)createdResult.Value).Title);
        }

        [Fact]
        public async Task Update_ReturnsNoContent_WhenSuccess()
        {
            var dto = new ProductUpdateDTO { Title = "Updated Title" };
            var entity = new Product { Id = 1, Title = "Old Title" };

            _unitOfWorkMock.Setup(u => u.Products.GetByIdAsync(1)).ReturnsAsync(entity);

            var result = await _controller.Update(1, dto);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenProductMissing()
        {
            _unitOfWorkMock.Setup(u => u.Products.GetByIdAsync(99)).ReturnsAsync((Product)null);

            var result = await _controller.Update(99, new ProductUpdateDTO());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task PatchProduct_ReturnsNoContent_WhenPatched()
        {
            var product = new Product { Id = 1, Title = "Title", Description = "Desc", Author = "A", Price = 10, CategoryId = 1 };
            _unitOfWorkMock.Setup(u => u.Products.GetByIdAsync(1)).ReturnsAsync(product);

            var patchDoc = new JsonPatchDocument<ProductPatchDTO>();
            patchDoc.Replace(p => p.Title, "Updated Title");

            var result = await _controller.PatchProduct(1, patchDoc);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PatchProduct_ReturnsNotFound_WhenNotExist()
        {
            _unitOfWorkMock.Setup(u => u.Products.GetByIdAsync(999)).ReturnsAsync((Product)null);

            var result = await _controller.PatchProduct(999, new JsonPatchDocument<ProductPatchDTO>());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenDeleted()
        {
            var product = new Product { Id = 1 };
            _unitOfWorkMock.Setup(u => u.Products.GetByIdAsync(1)).ReturnsAsync(product);

            var result = await _controller.Delete(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenMissing()
        {
            _unitOfWorkMock.Setup(u => u.Products.GetByIdAsync(999)).ReturnsAsync((Product)null);

            var result = await _controller.Delete(999);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}

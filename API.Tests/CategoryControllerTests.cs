using Moq;
using AutoMapper;
using Core.Interfaces;
using API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using API.DTOs.Category;
using Microsoft.Extensions.Caching.Memory;
using Core.Params;

namespace API.Tests
{
    public class CategoryControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly IMemoryCache _memoryCache;
        private readonly CategoryController _controller;

        public CategoryControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _controller = new CategoryController(_mockUnitOfWork.Object, _mockMapper.Object, _memoryCache);
        }

        // 1. GET ALL
        [Fact]
        public async Task GetAll_ReturnsOk_WithCachedCategories()
        {
            // Arrange
            var cacheKey = "allCategories";
            var cachedList = new List<CategoryReadDTO>
            {
                new CategoryReadDTO { Id = 1, catName = "Cached", catOrder = 1 }
            };

            _memoryCache.Set(cacheKey, cachedList, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(1)));

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<CategoryReadDTO>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult_FromDatabase_AndSetsCache()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 2, catName = "Database", catOrder = 2 }
            };

            var categoryDTOs = new List<CategoryReadDTO>
            {
                new CategoryReadDTO { Id = 2, catName = "Database" }
            };

            _mockUnitOfWork.Setup(u => u.Categories.GetAllOrderedAsync()).ReturnsAsync(categories);
            _mockMapper.Setup(m => m.Map<IEnumerable<CategoryReadDTO>>(categories)).Returns(categoryDTOs);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<CategoryReadDTO>>(okResult.Value);
            Assert.Single(returnValue);
        }

        // 1.5. GET PAGED
        [Fact]
        public async Task GetPagedCategories_ReturnsPagedList()
        {
            // arrange
            var paginationParams = new PaginationParams { PageNumber = 1, PageSize = 5 };

            var pagedCategories = new List<Category>
            {
                new Category { Id = 1, catName = "A", catOrder = 1 },
                new Category { Id = 2, catName = "B", catOrder = 2 },
                new Category { Id = 3, catName = "C", catOrder = 3 },
                new Category { Id = 4, catName = "D", catOrder = 4 },
                new Category { Id = 5, catName = "E", catOrder = 5 }
            };

            var mappedDtos = new List<CategoryReadDTO>
            {
                new CategoryReadDTO { Id = 1, catName = "A", catOrder = 1 },
                new CategoryReadDTO { Id = 2, catName = "B", catOrder = 2 },
                new CategoryReadDTO { Id = 3, catName = "C", catOrder = 3 },
                new CategoryReadDTO { Id = 4, catName = "D", catOrder = 4 },
                new CategoryReadDTO { Id = 5, catName = "E", catOrder = 5 },
            };

            _mockUnitOfWork.Setup(u => u.Categories.GetPagedCategoriesAsync(paginationParams))
                .ReturnsAsync(pagedCategories);

            _mockMapper.Setup(m => m.Map<IEnumerable<CategoryReadDTO>>(pagedCategories))
                .Returns(mappedDtos);

            // Act
            var actionResult = await _controller.GetPagedCategories(paginationParams);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<CategoryReadDTO>>(okResult.Value);
            Assert.Equal(5, ((List<CategoryReadDTO>)returnValue).Count);
        }

        // 2. GET BY ID
        [Fact]
        public async Task GetById_ReturnsNotFound_WhenCategoryIsNull()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.Categories.GetByIdAsync(10)).ReturnsAsync((Category)null);

            // Act
            var result = await _controller.GetById(10);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetById_ReturnsCategory_WhenExists()
        {
            // Arrange
            var category = new Category { Id = 3, catName = "Test" };
            var dto = new CategoryReadDTO { Id = 3, catName = "Test" };

            _mockUnitOfWork.Setup(u => u.Categories.GetByIdAsync(3)).ReturnsAsync(category);
            _mockMapper.Setup(m => m.Map<CategoryReadDTO>(category)).Returns(dto);

            // Act
            var result = await _controller.GetById(3);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<CategoryReadDTO>(okResult.Value);
            Assert.Equal(3, returnValue.Id);
        }

        // 3. CREATE
        [Fact]
        public async Task Create_ReturnsBadRequest_IfModelInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await _controller.Create(new CategoryCreateDTO());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenValid()
        {
            // Arrange
            var createDto = new CategoryCreateDTO { catName = "NewCat", catOrder = 1 };
            var category = new Category { Id = 4, catName = "NewCat", catOrder = 1 };
            var readDto = new CategoryReadDTO { Id = 4, catName = "NewCat", catOrder = 1 };

            _mockMapper.Setup(m => m.Map<Category>(createDto)).Returns(category);
            _mockMapper.Setup(m => m.Map<CategoryReadDTO>(category)).Returns(readDto);
            _mockUnitOfWork.Setup(u => u.Categories.AddAsync(category)).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
            
            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var createdAt = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<CategoryReadDTO>(createdAt.Value);
            Assert.Equal(4, returnValue.Id);
        }

        // 4. UPDATE
        [Fact]
        public async Task Update_ReturnsBadRequest_WhenModelInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await _controller.Update(5, new CategoryUpdateDTO());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenCategoryDoesNotExist()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.Categories.GetByIdAsync(5)).ReturnsAsync((Category)null);

            // Act
            var result = await _controller.Update(5, new CategoryUpdateDTO());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsNoContent_WhenSuccessful()
        {
            // Arrange
            var dto = new CategoryUpdateDTO { catName = "Updated", catOrder = 2 };
            var entity = new Category { Id = 6, catName = "Old", catOrder = 1 };

            _mockUnitOfWork.Setup(u => u.Categories.GetByIdAsync(6)).ReturnsAsync(entity);
            _mockMapper.Setup(m => m.Map(dto, entity));
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            // Act
            var result = await _controller.Update(6, dto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        // 5. DELETE
        [Fact]
        public async Task Delete_ReturnsNotFound_WhenCategoryNotExist()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.Categories.GetByIdAsync(7)).ReturnsAsync((Category)null);

            // Act
            var result = await _controller.Delete(7);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenDeleted()
        {
            // Arrange
            var entity = new Category { Id = 8, catName = "DeleteMe" };

            _mockUnitOfWork.Setup(u => u.Categories.GetByIdAsync(8)).ReturnsAsync(entity);
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            // Act
            var result = await _controller.Delete(8);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}

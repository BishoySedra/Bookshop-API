using Xunit;
using Moq;
using AutoMapper;
using Core.Interfaces;
using API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using API.DTOs.Category;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

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

            _mockUnitOfWork.Setup(u => u.Categories.GetAllOrderedAsync())
                .ReturnsAsync(categories);
            _mockMapper.Setup(m => m.Map<IEnumerable<CategoryReadDTO>>(categories))
                .Returns(categoryDTOs);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<CategoryReadDTO>>(okResult.Value);
            Assert.Single(returnValue);
        }

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
    }
}

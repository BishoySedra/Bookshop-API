using Microsoft.AspNetCore.Mvc;
using Core.Interfaces;
using Models.Entities;
using API.DTOs.Category;
using AutoMapper;
using Core.Params;
using Microsoft.Extensions.Caching.Memory;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;

        public CategoryController(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache memoryCache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// Retrieves all categories.
        /// </summary>
        /// <remarks>Returns all categories ordered by CatOrder then CatName. Data is cached for 1 minute.</remarks>
        /// <returns>A list of CategoryReadDTO</returns>
        /// <response code="200">Returns the list of categories</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CategoryReadDTO>), 200)]
        public async Task<ActionResult<IEnumerable<CategoryReadDTO>>> GetAll()
        {
            string cacheKey = "allCategories";

            if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<CategoryReadDTO> cachedCategories))
            {
                return Ok(cachedCategories);
            }

            var categories = await _unitOfWork.Categories.GetAllOrderedAsync();
            var result = _mapper.Map<IEnumerable<CategoryReadDTO>>(categories);

            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(1));
            _memoryCache.Set(cacheKey, result, cacheEntryOptions);

            return Ok(result);
        }

        /// <summary>
        /// Retrieves paged list of categories.
        /// </summary>
        /// <param name="paginationParams">Pagination parameters</param>
        /// <returns>Paged list of CategoryReadDTO</returns>
        /// <response code="200">Returns paged categories</response>
        [HttpGet("paged")]
        [ProducesResponseType(typeof(IEnumerable<CategoryReadDTO>), 200)]
        public async Task<IActionResult> GetPagedCategories([FromQuery] PaginationParams paginationParams)
        {
            string cacheKey = $"pagedCategories_{paginationParams.PageNumber}_{paginationParams.PageSize}";

            if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<CategoryReadDTO> cachedCategories))
            {
                return Ok(cachedCategories);
            }

            var categories = await _unitOfWork.Categories.GetPagedCategoriesAsync(paginationParams);
            var categoryDTOs = _mapper.Map<IEnumerable<CategoryReadDTO>>(categories);

            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(1));
            _memoryCache.Set(cacheKey, categoryDTOs, cacheEntryOptions);

            return Ok(categoryDTOs);
        }

        /// <summary>
        /// Retrieves a category by its ID.
        /// </summary>
        /// <param name="id">The ID of the category</param>
        /// <returns>A single category</returns>
        /// <response code="200">Returns the category</response>
        /// <response code="404">If the category is not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CategoryReadDTO), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<CategoryReadDTO>> GetById(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CategoryReadDTO>(category));
        }

        /// <summary>
        /// Creates a new category.
        /// </summary>
        /// <param name="dto">The category to create</param>
        /// <returns>The created category</returns>
        /// <response code="201">Returns the newly created category</response>
        /// <response code="400">If the input is invalid</response>
        [HttpPost]
        [ProducesResponseType(typeof(CategoryReadDTO), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<CategoryReadDTO>> Create(CategoryCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = _mapper.Map<Category>(dto);
            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.CompleteAsync();

            var result = _mapper.Map<CategoryReadDTO>(category);

            return CreatedAtAction(nameof(GetById), new { id = category.Id }, result);
        }

        /// <summary>
        /// Updates an existing category.
        /// </summary>
        /// <param name="id">ID of the category to update</param>
        /// <param name="dto">The updated category values</param>
        /// <response code="204">If the update was successful</response>
        /// <response code="400">If the input is invalid</response>
        /// <response code="404">If the category is not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, CategoryUpdateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingCategory = await _unitOfWork.Categories.GetByIdAsync(id);
            if (existingCategory == null)
            {
                return NotFound();
            }

            _mapper.Map(dto, existingCategory);
            _unitOfWork.Categories.Update(existingCategory);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        /// <summary>
        /// Deletes a category by its ID.
        /// </summary>
        /// <param name="id">ID of the category to delete</param>
        /// <response code="204">If the deletion was successful</response>
        /// <response code="404">If the category is not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            var existingCategory = await _unitOfWork.Categories.GetByIdAsync(id);

            if (existingCategory == null)
            {
                return NotFound();
            }

            _unitOfWork.Categories.Delete(existingCategory);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
    }
}

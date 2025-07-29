using Microsoft.AspNetCore.Mvc;
using Core.Interfaces;
using Models.Entities;
using API.DTOs.Category;
using AutoMapper;
using Core.Params;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/Category
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryReadDTO>>> GetAll()
        {
            // This method retrieves all categories from the database and maps them to DTOs.
            var categories = await _unitOfWork.Categories.GetAllOrderedAsync();

            // result is a collection of CategoryReadDTO objects
            var result = _mapper.Map<IEnumerable<CategoryReadDTO>>(categories);

            // If no categories are found, return an empty list with a 200 OK status.
            return Ok(result);
        }

        // GET: api/Category/paged
        [HttpGet("paged")]
        public async Task<IActionResult> GetPagedCategories([FromQuery] PaginationParams paginationParams)
        {
            var categories = await _unitOfWork.Categories.GetPagedCategoriesAsync(paginationParams);
            var categoryDTOs = _mapper.Map<IEnumerable<CategoryReadDTO>>(categories);
            return Ok(categoryDTOs);
        }


        // GET: api/Category/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryReadDTO>> GetById(int id)
        {
            // This method retrieves a specific category by its ID and maps it to a DTO.
            var category = await _unitOfWork.Categories.GetByIdAsync(id);

            // If the category is not found, return a 404 Not Found status.
            if (category == null)
            {
                return NotFound();
            }

            // If the category is found, map it to a CategoryReadDTO and return it with a 200 OK status.
            return Ok(_mapper.Map<CategoryReadDTO>(category));
        }

        // POST: api/Category
        [HttpPost]
        public async Task<ActionResult<CategoryReadDTO>> Create(CategoryCreateDTO dto)
        {
            // Check if the model state is valid before proceeding with the creation.
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // CategoryCreateDTO is mapped to a Category entity.
            var category = _mapper.Map<Category>(dto);

            // Add the new category to the database using the unit of work pattern.
            await _unitOfWork.Categories.AddAsync(category);

            // Complete the transaction to save changes to the database.
            await _unitOfWork.CompleteAsync();

            // After the category is created, it is mapped back to a CategoryReadDTO for the response.
            var result = _mapper.Map<CategoryReadDTO>(category);

            // Return a 201 Created response with the location of the newly created category.
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, result);
        }

        // PUT: api/Category/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CategoryUpdateDTO dto)
        {
            // Check if the model state is valid before proceeding with the update.
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if the category with the specified ID exists.
            var existingCategory = await _unitOfWork.Categories.GetByIdAsync(id);
            if (existingCategory == null)
            {
                return NotFound();
            }

            // Map the updated DTO to the existing category entity.
            _mapper.Map(dto, existingCategory);

            // Update the existing category in the database using the unit of work pattern.
            _unitOfWork.Categories.Update(existingCategory);

            // Complete the transaction to save changes to the database.
            await _unitOfWork.CompleteAsync();

            // Return a 204 No Content response to indicate that the update was successful.
            return NoContent();
        }

        // DELETE: api/Category/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // Getting the existing category by ID to check if it exists.
            var existingCategory = await _unitOfWork.Categories.GetByIdAsync(id);

            // Check if the category exists before attempting to delete it.
            if (existingCategory == null)
            {
                return NotFound();
            }

            // Delete the existing category using the unit of work pattern.
            _unitOfWork.Categories.Delete(existingCategory);

            // Complete the transaction to save changes to the database.
            await _unitOfWork.CompleteAsync();

            // Return a 204 No Content response to indicate that the deletion was successful.
            return NoContent();
        }
    }
}

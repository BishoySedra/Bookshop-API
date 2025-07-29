using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Core.Interfaces;
using Models.Entities;
using API.DTOs.Product;

namespace API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class ProductController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/Product
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductReadDTO>>> GetAll()
        {
            // Fetch all products with their categories
            var products = await _unitOfWork.Products.FindAsync(p => true);

            // Include Category manually
            var result = products.Select(p => new ProductReadDTO
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                Author = p.Author,
                Price = p.Price,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.catName
            });

            // Return the list of products
            return Ok(result);
        }

        // GET: api/Product/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductReadDTO>> GetById(int id)
        {
            // Fetch product by id with its category
            var product = await _unitOfWork.Products.FindAsync(p => p.Id == id);

            // If no product is found, return NotFound
            var p = product.FirstOrDefault();

            if (p == null) { 
                return NotFound();
            }

            // Map the product to ProductReadDTO
            var result = new ProductReadDTO
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                Author = p.Author,
                Price = p.Price,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.catName
            };

            return Ok(result);
        }

        // POST: api/Product
        [HttpPost]
        public async Task<ActionResult<ProductReadDTO>> Create(ProductCreateDTO dto)
        {
            // Check if the model state is valid to ensure all required fields are provided
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            };

            // Map the DTO to the Product entity
            var product = _mapper.Map<Product>(dto);

            // Add the product to the database
            await _unitOfWork.Products.AddAsync(product);

            // Save changes to the database
            await _unitOfWork.CompleteAsync();

            // Map the created product back to ProductReadDTO for the response
            var result = _mapper.Map<ProductReadDTO>(product);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, result);
        }

        // PUT: api/Product/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProductUpdateDTO dto)
        {
            // Check if the model state is valid to ensure all required fields are provided
            if (!ModelState.IsValid) { 
                return BadRequest(ModelState);
            }

            // Check if the product exists
            var existingProduct = await _unitOfWork.Products.GetByIdAsync(id);

            // If the product does not exist, return NotFound
            if (existingProduct == null) { 
                return NotFound();
            }

            // Map the DTO to the existing product entity
            _mapper.Map(dto, existingProduct);

            // Update the product in the database
            _unitOfWork.Products.Update(existingProduct);

            // Save changes to the database
            await _unitOfWork.CompleteAsync();

            // Return NoContent to indicate the update was successful
            return NoContent();
        }

        // PATCH: api/Product/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchProduct(int id, [FromBody] JsonPatchDocument<ProductPatchDTO> patchDoc)
        {
            // Check if the patch document is null
            if (patchDoc == null)
            {
                return BadRequest("Invalid patch document.");
            }

            // Check if the product exists
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Create a DTO to hold the patchable fields
            var patchDTO = new ProductPatchDTO
            {
                Title = product.Title,
                Description = product.Description,
                Author = product.Author,
                Price = product.Price,
                CategoryId = product.CategoryId
            };

            // Apply the patch document to the DTO
            patchDoc.ApplyTo(patchDTO, ModelState);

            // Check if the model state is valid after applying the patch
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Map updated fields back to the entity
            product.Title = patchDTO.Title;
            product.Description = patchDTO.Description;
            product.Author = patchDTO.Author;
            product.Price = patchDTO.Price;
            product.CategoryId = patchDTO.CategoryId;

            // Update the product in the database
            _unitOfWork.Products.Update(product);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        // DELETE: api/Product/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existingProduct = await _unitOfWork.Products.GetByIdAsync(id);
            if (existingProduct == null) return NotFound();

            _unitOfWork.Products.Delete(existingProduct);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
    }
}

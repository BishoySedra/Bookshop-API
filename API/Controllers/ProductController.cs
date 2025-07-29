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

        /// <summary>
        /// Retrieves all products with their associated categories.
        /// </summary>
        /// <returns>A list of products with category names.</returns>
        [HttpGet]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client, NoStore = false)]
        public async Task<ActionResult<IEnumerable<ProductReadDTO>>> GetAll()
        {
            var products = await _unitOfWork.Products.FindAsync(p => true);

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

            return Ok(result);
        }

        /// <summary>
        /// Retrieves a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product.</param>
        /// <returns>A single product with its details and category.</returns>
        [HttpGet("{id}")]
        [ResponseCache(CacheProfileName = "Default60")]
        public async Task<ActionResult<ProductReadDTO>> GetById(int id)
        {
            var product = await _unitOfWork.Products.FindAsync(p => p.Id == id);
            var p = product.FirstOrDefault();

            if (p == null)
            {
                return NotFound();
            }

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

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="dto">The product data to create.</param>
        /// <returns>The newly created product with a 201 response.</returns>
        [HttpPost]
        [ResponseCache(CacheProfileName = "NoCache")]
        public async Task<ActionResult<ProductReadDTO>> Create(ProductCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = _mapper.Map<Product>(dto);
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.CompleteAsync();

            var result = _mapper.Map<ProductReadDTO>(product);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, result);
        }

        /// <summary>
        /// Updates an existing product.
        /// </summary>
        /// <param name="id">The ID of the product to update.</param>
        /// <param name="dto">The updated product data.</param>
        /// <returns>NoContent if the update is successful; NotFound if the product doesn't exist.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProductUpdateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingProduct = await _unitOfWork.Products.GetByIdAsync(id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            _mapper.Map(dto, existingProduct);
            _unitOfWork.Products.Update(existingProduct);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        /// <summary>
        /// Partially updates a product using a JSON Patch document.
        /// </summary>
        /// <param name="id">The ID of the product to patch.</param>
        /// <param name="patchDoc">The patch document containing operations to perform.</param>
        /// <returns>NoContent if the patch is applied; NotFound or BadRequest otherwise.</returns>
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchProduct(int id, [FromBody] JsonPatchDocument<ProductPatchDTO> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest("Invalid patch document.");
            }

            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var patchDTO = new ProductPatchDTO
            {
                Title = product.Title,
                Description = product.Description,
                Author = product.Author,
                Price = product.Price,
                CategoryId = product.CategoryId
            };

            patchDoc.ApplyTo(patchDTO, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            product.Title = patchDTO.Title;
            product.Description = patchDTO.Description;
            product.Author = patchDTO.Author;
            product.Price = patchDTO.Price;
            product.CategoryId = patchDTO.CategoryId;

            _unitOfWork.Products.Update(product);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        /// <summary>
        /// Deletes a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to delete.</param>
        /// <returns>NoContent if deletion is successful; NotFound if the product does not exist.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existingProduct = await _unitOfWork.Products.GetByIdAsync(id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            _unitOfWork.Products.Delete(existingProduct);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
    }
}

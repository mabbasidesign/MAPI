using AutoMapper;
using MAPI.Data;
using MAPI.Dto;
using MAPI.IServices;
using MAPI.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductOrderController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public ProductOrderController(IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductsDto>>> GetAllProducts()
        {
            var products = await _productService.GetAll();
            var activeProducts = products.Where(p => !p.IsDeleted).ToList();
            var productsDto = _mapper.Map<List<ProductsDto>>(activeProducts);
            return Ok(productsDto);
        }

        [HttpGet("{id}", Name = "GetProductById")]
        public async Task<ActionResult<ProductsDto>> GetProductById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid product id.");
            }

            var product = await _productService.Get(id);

            if (product == null)
            {
                return NotFound("Product not found.");
            }

            var productDto = _mapper.Map<ProductsDto>(product);

            return Ok(productDto);
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] ProductsDto productsDto)
        {
            if (productsDto == null)
            {
                return BadRequest("Product is null.");
            }

            var product = _mapper.Map<Products>(productsDto);
            await _productService.Create(product);

            return CreatedAtRoute("GetProductById", new { id = product.Id }, product);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (id <=0)
            {
                return BadRequest("Invalid product id.");
            }

            var product = await _productService.Get(id);

            if (product == null)
            {
                return NotFound("Product not found.");
            }

            product.IsDeleted = true;
            await _productService.Update(product);

            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] ProductsDto productDto)
        {
            if(productDto == null || id <= 0)
            {
                return BadRequest("Invalid data");
            }

            var existingProduct = await _productService.Get(id);
            if(existingProduct == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }

            var updatedProduct = _mapper.Map<Products>(productDto);
            //await _productService.Update(updatedProduct);
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<ProductsDto>> PatchProduct(int id, [FromBody] JsonPatchDocument<ProductsDto> patchDoc)
        {
            if (patchDoc == null || id <= 0)
            {
                return BadRequest("Invalid patch document or id.");
            }

            var product = await _productService.Get(id);
            if (product == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }

            var productDto = new ProductsDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Status = product.Status
            };

            patchDoc.ApplyTo(productDto, (Microsoft.AspNetCore.JsonPatch.Adapters.IObjectAdapter)ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            product.Name = productDto.Name;
            product.Price = productDto.Price;
            product.Status = productDto.Status;

            await _productService.Update(product);

            return Ok(productDto);
        }

    }
}


// Patch update (JSON Patch)
// Soft Delete
// Query string search
// Filtering + Sorting
// Pagination
// Image upload
// Advanced Routing
// Global Error Handling Middleware
// Validation Filter
// Swagger documentation
// JWT Authentication
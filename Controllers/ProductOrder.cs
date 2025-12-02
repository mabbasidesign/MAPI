using AutoMapper;
using MAPI.Data;
using MAPI.Dto;
using MAPI.IServices;
using MAPI.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace MAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductOrderController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;

        public ProductOrderController(IProductService productService, IMapper mapper, IDistributedCache cache)
        {
            _productService = productService;
            _mapper = mapper;
            _cache = cache;
        }

        [HttpGet]
        //[ProducesResponseType(typeof(List<ProductsDto>), StatusCodes.Status200OK)]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Roles = "admin")]
        //[Authorize]
        //public async Task<ActionResult<List<ProductsDto>>> GetAllProducts()
        //{
        //    var products = await _productService.GetAll();
        //    var activeProducts = products.Where(p => !p.IsDeleted).ToList();
        //    var productsDto = _mapper.Map<List<ProductsDto>>(activeProducts);
        //    return Ok(productsDto);
        //}
        public async Task<ActionResult<List<ProductsDto>>> GetAllProducts()
        {
            const string cacheKey = "all_products";
            var cachedProducts = await _cache.GetStringAsync(cacheKey);

            List<ProductsDto> productsDto;
            if (!string.IsNullOrEmpty(cachedProducts))
            {
                productsDto = JsonSerializer.Deserialize<List<ProductsDto>>(cachedProducts)!;
            }
            else
            {
                var products = await _productService.GetAll();
                var activeProducts = products.Where(p => !p.IsDeleted).ToList();
                productsDto = _mapper.Map<List<ProductsDto>>(activeProducts);

                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                };
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(productsDto), options);
            }

            return Ok(productsDto);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<List<ProductsDto>>> GetPagedProducts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var products = await _productService.GetAll();
            var activeProducts = products.Where(p => !p.IsDeleted);

            var pagedProducts = activeProducts
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var productsDto = _mapper.Map<List<ProductsDto>>(pagedProducts);
            return Ok(productsDto);
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<ProductsDto>>> SearchProducts([FromQuery] string search)
        {
            var products = await _productService.GetAll();
            var activeProducts = products.Where(p => !p.IsDeleted);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.ToLower();
                activeProducts = activeProducts.Where(p =>
                    (!string.IsNullOrEmpty(p.Name) && p.Name.ToLower().Contains(lowerSearch)) ||
                    (!string.IsNullOrEmpty(p.Status) && p.Status.ToLower().Contains(lowerSearch))
                );
            }

            var productsDto = _mapper.Map<List<ProductsDto>>(activeProducts.ToList());
            return Ok(productsDto);
        }

        [HttpGet("filter")]
        public async Task<ActionResult<List<ProductsDto>>> FilterAndSortProducts(
            [FromQuery] string? status,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] string? sortBy,
            [FromQuery] bool desc = false)
        {
            var products = await _productService.GetAll();
            var activeProducts = products.Where(p => !p.IsDeleted);

            // Filtering
            if (!string.IsNullOrWhiteSpace(status))
                activeProducts = activeProducts.Where(p => p.Status != null && p.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
            if (minPrice.HasValue)
                activeProducts = activeProducts.Where(p => p.Price >= minPrice.Value);
            if (maxPrice.HasValue)
                activeProducts = activeProducts.Where(p => p.Price <= maxPrice.Value);

            // Sorting
            activeProducts = sortBy switch
            {
                "name" => desc ? activeProducts.OrderByDescending(p => p.Name) : activeProducts.OrderBy(p => p.Name),
                "price" => desc ? activeProducts.OrderByDescending(p => p.Price) : activeProducts.OrderBy(p => p.Price),
                "status" => desc ? activeProducts.OrderByDescending(p => p.Status) : activeProducts.OrderBy(p => p.Status),
                _ => activeProducts
            };

            var productsDto = _mapper.Map<List<ProductsDto>>(activeProducts.ToList());
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

        [HttpPost("upload-image/{id}")]
        public async Task<IActionResult> UploadImage(int id, IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest("No image uploaded.");

            try
            {
                var imagePath = await _productService.UploadImage(id, image);
                return Ok(new { imagePath });
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Product not found.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
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
// Rate limiting
// JWT Authentication
// Unit tests
// Entity Framework advanced queries
// Background jobs
// Caching (Redis)
// CI/CD basics
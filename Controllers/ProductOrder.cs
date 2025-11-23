using AutoMapper;
using MAPI.Data;
using MAPI.Dto;
using MAPI.IServices;
using MAPI.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MAPI.Controllers
{
    [Route("Product/[controller]")]
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
            var productsDto = _mapper.Map<ProductsDto>(products);
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

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct([FromForm] ProductsDto productsDto)
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
            if (id <= 0)
            {
                return BadRequest("Invalid product id.");
            }

            var product = await _productService.Get(id);

            if (product == null)
            {
                return NotFound("Product not found.");
            }

            await _productService.Delete(id);

            return NoContent();
        }
    }
}


// data anotations
// update
// update
// patch update
// soft delete
// dto
// automapper
// query string search string from route
// upload image from file
// pagination
// filter
// advance routing
// versioning
// middleware error
// middleware filter validation
// api documantation
// jwt
// 
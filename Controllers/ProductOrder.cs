using MAPI.Data;
using MAPI.IServices;
using MAPI.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MAPI.Controllers
{
    [Route("Product/[controller]")]
    [ApiController]
    public class ProductOrder: ControllerBase
    {
        private readonly AppDbContext _db;
        public ProductOrder(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<List<Products>>> GettAll()
        {
            var products = await _db.Products.ToListAsync();

            if(products == null)
            {
                return NotFound("No product found");
            }

            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Products>> GetById(int id)
        {
            if(id <= 0)
            {
                return BadRequest("id is null");
            }

            var product = await _db.Products.Where(p => p.Id == id).FirstOrDefaultAsync();

            if(product == null)
            {
                return NotFound("No product found");
            }

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromForm] Products product)
        {
            if (product == null)
            {
                return BadRequest("product is null");
            }

            _db.Products.Add(product);
            await _db.SaveChangesAsync();

            return CreatedAtRoute(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if(id <= 0)
            {
                return BadRequest("Invalid id!");
            }

            var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == id);

            if(product == null)
            {
                return NotFound("product is null");
            }

            return NotFound();
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
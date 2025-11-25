using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using MAPI.Dto;
using System.Text.Json;

namespace MAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductOrderDapperController : ControllerBase
    {
        private readonly string _connectionString = "ConnectionStringHere";

        [HttpGet]
        public async Task<ActionResult<List<ProductsDto>>> GetAllProducts()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var products = (await conn.QueryAsync<ProductsDto>(
                "SELECT Id, Name, Price, Status FROM Products WHERE IsDeleted =0")).ToList();
                return Ok(products);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductsDto>> GetProductById(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var product = await conn.QueryFirstOrDefaultAsync<ProductsDto>(
                "SELECT Id, Name, Price, Status FROM Products WHERE Id = @Id AND IsDeleted =0", new { Id = id });
                if (product == null)
                    return NotFound();
                return Ok(product);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] ProductsDto productsDto)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var sql = "INSERT INTO Products (Name, Price, Status, IsDeleted) VALUES (@Name, @Price, @Status,0); SELECT CAST(SCOPE_IDENTITY() as int);";
                var newId = await conn.QuerySingleAsync<int>(sql, productsDto);
                productsDto.Id = newId;
                return CreatedAtAction(nameof(GetProductById), new { id = newId }, productsDto);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductsDto productsDto)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var sql = "UPDATE Products SET Name = @Name, Price = @Price, Status = @Status WHERE Id = @Id AND IsDeleted =0";
                var rows = await conn.ExecuteAsync(sql, new { Id = id, productsDto.Name, productsDto.Price, productsDto.Status });
                if (rows > 0)
                    return NoContent();
                return NotFound();
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchProduct(int id, [FromBody] JsonElement patchDoc)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var product = await conn.QueryFirstOrDefaultAsync<ProductsDto>(
                "SELECT Id, Name, Price, Status FROM Products WHERE Id = @Id AND IsDeleted =0", new { Id = id });
                if (product == null)
                    return NotFound();

                foreach (var op in patchDoc.EnumerateArray())
                {
                    var path = op.GetProperty("path").GetString();
                    var value = op.GetProperty("value");
                    if (path == "/name") product.Name = value.GetString();
                    if (path == "/price") product.Price = value.GetDecimal();
                    if (path == "/status") product.Status = value.GetString();
                }

                var sql = "UPDATE Products SET Name = @Name, Price = @Price, Status = @Status WHERE Id = @Id AND IsDeleted =0";
                await conn.ExecuteAsync(sql, new { Id = id, product.Name, product.Price, product.Status });
                return Ok(product);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var sql = "UPDATE Products SET IsDeleted =1 WHERE Id = @Id";
                var rows = await conn.ExecuteAsync(sql, new { Id = id });
                if (rows > 0)
                    return NoContent();
                return NotFound();
            }
        }
    }
}

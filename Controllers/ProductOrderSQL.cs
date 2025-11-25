using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MAPI.Dto;
using System.Data;
using System.Text.Json;

namespace MAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductOrderSQLController : ControllerBase
    {
        private readonly string _connectionString = "ConnectionStringHere";

        [HttpGet]
        public async Task<ActionResult<List<ProductsDto>>> GetAllProducts()
        {
            var products = new List<ProductsDto>();
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("SELECT Id, Name, Price, Status FROM Products WHERE IsDeleted =0", conn);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        products.Add(new ProductsDto
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Price = reader.GetDecimal(2),
                            Status = reader.GetString(3)
                        });
                    }
                }
            }
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductsDto>> GetProductById(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("SELECT Id, Name, Price, Status FROM Products WHERE Id = @Id AND IsDeleted =0", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        var product = new ProductsDto
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Price = reader.GetDecimal(2),
                            Status = reader.GetString(3)
                        };
                        return Ok(product);
                    }
                }
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] ProductsDto productsDto)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("INSERT INTO Products (Name, Price, Status, IsDeleted) VALUES (@Name, @Price, @Status,0); SELECT SCOPE_IDENTITY();", conn);
                cmd.Parameters.AddWithValue("@Name", productsDto.Name);
                cmd.Parameters.AddWithValue("@Price", productsDto.Price);
                cmd.Parameters.AddWithValue("@Status", productsDto.Status);
                var newId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                productsDto.Id = newId;
                return CreatedAtAction(nameof(GetProductById), new { id = newId }, productsDto);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductsDto productsDto)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("UPDATE Products SET Name = @Name, Price = @Price, Status = @Status WHERE Id = @Id AND IsDeleted =0", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Name", productsDto.Name);
                cmd.Parameters.AddWithValue("@Price", productsDto.Price);
                cmd.Parameters.AddWithValue("@Status", productsDto.Status);
                var rows = await cmd.ExecuteNonQueryAsync();
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
                await conn.OpenAsync();
                var getCmd = new SqlCommand("SELECT Id, Name, Price, Status FROM Products WHERE Id = @Id AND IsDeleted =0", conn);
                getCmd.Parameters.AddWithValue("@Id", id);
                ProductsDto product = null;
                using (var reader = await getCmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        product = new ProductsDto
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Price = reader.GetDecimal(2),
                            Status = reader.GetString(3)
                        };
                    }
                }
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

                var updateCmd = new SqlCommand("UPDATE Products SET Name = @Name, Price = @Price, Status = @Status WHERE Id = @Id AND IsDeleted =0", conn);
                updateCmd.Parameters.AddWithValue("@Id", id);
                updateCmd.Parameters.AddWithValue("@Name", product.Name);
                updateCmd.Parameters.AddWithValue("@Price", product.Price);
                updateCmd.Parameters.AddWithValue("@Status", product.Status);
                await updateCmd.ExecuteNonQueryAsync();

                return Ok(product);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("UPDATE Products SET IsDeleted =1 WHERE Id = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                var rows = await cmd.ExecuteNonQueryAsync();
                if (rows > 0)
                    return NoContent();
                return NotFound();
            }
        }
    }
}

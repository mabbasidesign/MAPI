using System.ComponentModel.DataAnnotations;

namespace MAPI.Dto
{
    public class ProductsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}

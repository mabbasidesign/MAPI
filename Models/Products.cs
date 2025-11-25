using MAPI.Helper;
using System.ComponentModel.DataAnnotations;

namespace MAPI.Model
{
    public class Products
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]

        public string Name { get; set; } = null!;
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]

        public decimal Price { get; set; }
        [Required(ErrorMessage = "Status is required.")]
        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters.")]

        public string Status { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public string? ImagePath { get; set; }
    }
}

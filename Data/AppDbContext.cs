using MAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace MAPI.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Products> Products { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("ProductDb");
        }
    }
}

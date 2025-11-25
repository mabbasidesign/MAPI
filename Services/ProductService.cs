using MAPI.Data;
using MAPI.IServices;
using MAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace MAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _db;
        public ProductService(AppDbContext db)
        {
            _db = db;
        }

        public async Task Create(Products product)
        {
            await _db.Products.AddAsync(product);
            await _db.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product != null)
            {
                product.IsDeleted = true;
                await _db.SaveChangesAsync();
            }
        }

        public async Task<Products?> Get(int id)
        {
            return await _db.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Products>> GetAll()
        {
            return await _db.Products
                .Where(p => !p.IsDeleted)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task Update(Products product)
        {
            var existingProduct = await _db.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
            if(existingProduct == null)
            {
                throw new KeyNotFoundException($"Product with Id {product.Id} not found.");
            }
            _db.Update(product);
            await _db.SaveChangesAsync();
        }

        public async Task<string> UploadImage(int productId, IFormFile image)
        {
            if (image == null || image.Length == 0)
                throw new ArgumentException("No image uploaded.");

            var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null)
                throw new KeyNotFoundException($"Product with Id {productId} not found.");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            Directory.CreateDirectory(uploadsFolder);
            var fileName = $"{Guid.NewGuid()}_{image.FileName}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            // Save image path to product
            product.ImagePath = $"/images/{fileName}";
            await _db.SaveChangesAsync();

            return product.ImagePath;
        }

    }
}

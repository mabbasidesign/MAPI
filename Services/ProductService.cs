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
                _db.Products.Remove(product);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<Products?> Get(int id)
        {
            return await _db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Products>> GetAll()
        {
            return await _db.Products.AsNoTracking().ToListAsync();
        }
    }
}

using MAPI.Model;

namespace MAPI.IServices
{
    public interface IProductService
    {
        Task<List<Products>> GetAll();
        Task<Products> Get(int id);
        Task Create(Products product);
        Task Delete(int id);
        Task Update(Products product);
    }
}

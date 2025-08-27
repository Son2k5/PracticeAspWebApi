
using MyApiProject.Models;

namespace MyApiProject.Repository.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProductAsync();
        Task<Product> AddAsync(Product product);
    }
}
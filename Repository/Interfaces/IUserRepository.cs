using MyApiProject.Models;

namespace MyApiProject.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string userName);
        Task<User> AddAsync(User User);
        Task<List<User>> GetAllAsync();
    }
}
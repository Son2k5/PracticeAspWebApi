
using MyApiProject.Repository.Interfaces;
using MyApiProject.Data;
using MyApiProject.Models;
using Microsoft.EntityFrameworkCore;

namespace MyApiProject.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context) => _context = context;

        public async Task<User> AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;

        }
        public async Task<List<User>> GetAllAsync() => await _context.Users.ToListAsync();

        public async Task<User?> GetByUsernameAsync(string username) =>
           await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
    }
}
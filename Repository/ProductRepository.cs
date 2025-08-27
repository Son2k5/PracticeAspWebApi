using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyApiProject.Data;
using MyApiProject.Models;
using MyApiProject.Repository.Interfaces;

namespace MyApiProject.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context) { _context = context; }

        public async Task<List<Product>> GetAllProductAsync()
        {
            return await _context.Products.ToListAsync();
        }
        public async Task<Product> AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }
    }
}
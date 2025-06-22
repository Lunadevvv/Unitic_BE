using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitic_BE.Abstracts;
using Unitic_BE.Entities;

namespace Unitic_BE.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;
        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.Where(u => !u.Is_Disable).ToListAsync();
        }
        public async Task<List<Category>> GetAllDisabledCategoriesAsync()
        {
            return await _context.Categories.Where(u => u.Is_Disable).ToListAsync();
        }
        public async Task AddCategoryAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }
        public async Task<Category?> GetCategoryByIdAsync(string id)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.CateID == id );
        }
        public async Task UpdateCategoryAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteCategoryAsync(Category category)
        {

            category.Is_Disable = !category.Is_Disable; // switch category is disabled
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();

        }
        public async Task<string> GetLastId()
        {
            string id = await _context.Categories
                .OrderByDescending(c => c.CateID)
                .Select(c => c.CateID)
                .FirstOrDefaultAsync();
            return id;
        }
        public async Task<Category> GetCategoryByNameAsync(string name)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Name == name);
        }


    }
}

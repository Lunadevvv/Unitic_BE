using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitic_BE.Entities;

namespace Unitic_BE.Abstracts
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllCategoriesAsync();
        Task<List<Category>> GetAllDisabledCategoriesAsync();
        Task AddCategoryAsync(Category category);
        Task<Category?> GetCategoryByIdAsync(string id);
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(Category category);
        Task<string> GetLastId();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitic_BE.Abstracts;
using Unitic_BE.Entities;
using Unitic_BE.Requests;
using Unitic_BE.Exceptions;

namespace Unitic_BE.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;
        private readonly CustomValidator _validator;

        public CategoryService(ICategoryRepository repo, CustomValidator validator)
        {
            _validator = validator;
            _repo = repo;
        }

        public Task<List<Category>> GetAllCategories()
        {
            return _repo.GetAllCategoriesAsync();
        }
        public Task<List<Category>> GetAllDisabledCategories()
        {
            return _repo.GetAllDisabledCategoriesAsync();
        }
        public async Task AddCategoryAsync(CategoryRequest categoryRequest)
        {
            // Trim all string properties in the request
            StringTrimmerExtension.TrimAllString(categoryRequest);
            //Check xem categoryName tồn tại chưa
            var categoryExists = await _repo.GetCategoryByNameAsync(categoryRequest.Name);
            if (categoryExists != null)
            {
                throw new ObjPropertyAlreadyExists(categoryRequest.Name);
            }
            // Create a new Category entity from the request
            Category category = new Category
            {
                CateID = await GenerateCategoryId(),
                Name = categoryRequest.Name,
                Is_Disable = categoryRequest.IsDisabled
            };
            // Add the category to the repository
            await _repo.AddCategoryAsync(category);
        }
        public async Task<Category?> GetCategoryByIdAsync(string id)
        {
            // Retrieve the category by ID
            var category = await _repo.GetCategoryByIdAsync(id);
            if(category == null)
            {
                throw new ObjectNotFoundException($"Category with id {id} is disabled or");
            }
            return category;
        }
        public async Task UpdateCategoryAsync(string id, CategoryUpdateRequest categoryRequest)
        {
            // Trim all string properties in the request
            StringTrimmerExtension.TrimAllString(categoryRequest);
            // Check xem category đã tồn tại chưa
            var category = await _repo.GetCategoryByIdAsync(id);
            if (category == null)
            {
                throw new ObjectNotFoundException($"Category with id {id} not found.");
            }
            //check categoryName tồn tại chưa
            var categoryExists = await _repo.GetCategoryByNameAsync(categoryRequest.Name);
            if (categoryExists != null && categoryExists.CateID != category.CateID)
            {
                throw new ObjPropertyAlreadyExists(categoryRequest.Name);
            }
            // Update the category properties
            category.Name = categoryRequest.Name;
            // Update the category in the repository
            await _repo.UpdateCategoryAsync(category);
        }
        public async Task DeleteCategoryAsync(string id)
        {
            // Retrieve the category by ID
            var category = await _repo.GetCategoryByIdAsync(id);
            if (category == null)
            {
                throw new ObjectNotFoundException($"Category with id {id} not found.");
            }
            // Delete the category from the repository
            await _repo.DeleteCategoryAsync(category);
        }
        private async Task<string> GenerateCategoryId()
        {
            string lastId = await _repo.GetLastId();
            if (lastId == null) return "Cate0001";
            int id = int.Parse(lastId.Substring(4)) + 1; // lấy id cuối cùng và cộng thêm 1
            string generatedId = "Cate" + id.ToString("D4");
            return generatedId;
        }
    }
}

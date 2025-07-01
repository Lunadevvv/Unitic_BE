using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitic_BE.Entities;
using Unitic_BE.DTOs.Requests;
using Unitic_BE.Exceptions;

namespace Unitic_BE.Abstracts
{
    public interface ICategoryService
    {

        Task<List<Category>> GetAllCategories();

        Task<List<Category>> GetAllDisabledCategories();

        Task AddCategoryAsync(CategoryRequest categoryRequest);

        Task<Category?> GetCategoryByIdAsync(string id);

        Task UpdateCategoryAsync(string id, CategoryUpdateRequest categoryRequest);

        Task DeleteCategoryAsync(string id);
    }

}


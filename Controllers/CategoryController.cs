using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Unitic_BE.Abstracts;
using Unitic_BE.DTOs.Requests;

namespace Unitic_BE.Controllers
{
    [Route("Unitic/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategories();
            return Ok(categories);
        }
        [HttpGet("Disabled")]
        public async Task<IActionResult> GetAllDisabledCategories()
        {
            var categories = await _categoryService.GetAllDisabledCategories();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(string id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound("Category not found.");
            }
            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] CategoryRequest category)
        {
            if (category == null)
            {
                return BadRequest("Bad request");
            }
            await _categoryService.AddCategoryAsync(category);
            return Ok("Category added successfully");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(string id, [FromBody] CategoryUpdateRequest request)
        {
            if (request == null)
            {
                return BadRequest("Bad request");
            }
            await _categoryService.UpdateCategoryAsync(id, request);
            return Ok("Category updated successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> SwitchCategory(string id)
        {
            await _categoryService.DeleteCategoryAsync(id);
            return Ok("Category switched successfully");
        }
    }
}

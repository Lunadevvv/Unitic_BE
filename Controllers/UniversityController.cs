using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Unitic_BE.Abstracts;
using Unitic_BE.Requests;
using Unitic_BE.Entities;

namespace Unitic_BE.Controllers
{
    [Route("Unitic/[controller]")]
    [ApiController]
    public class UniversityController : ControllerBase
    {
        private readonly IUniversityService _universityService;
        public UniversityController(IUniversityService universityService)
        {
            _universityService = universityService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllUniversities()
        {
            var universities = await _universityService.GetAllUniversity();
            return Ok(universities);
        }

        [HttpGet("Get/{id}")]
        public async Task<IActionResult> GetUniversityById(string id)
        {
            var university = await _universityService.GetUniversityById(id);
            if (university == null)
            {
                return NotFound("University not found.");
            }
            return Ok(university);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddUniversity([FromBody] UniversityRequest university)
        {
            if (university == null)
            {
                return BadRequest("Invalid university data.");
            }
            await _universityService.AddUniversity(university);
            return Ok("University added successfully");
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateUniversity(string id, [FromBody] UniversityRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid update data.");
            }
            await _universityService.UpdateUniversityById(id, request);
            return Ok("University updated successfully");
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteUniversity(string id)
        {
            await _universityService.DeleteUniversityById(id);
            return Ok("University deleted successfully");
        }
    }
}

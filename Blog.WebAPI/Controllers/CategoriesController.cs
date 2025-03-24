using BlogSystem.BLL.DTO;
using BlogSystem.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blog.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // API методи
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CategoryDto>> Create(CategoryDto categoryDto)
        {
            var createdCategory = await _categoryService.CreateAsync(categoryDto);
            return CreatedAtAction(nameof(GetById), new { id = createdCategory.Id }, createdCategory);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, CategoryDto categoryDto)
        {
            if (id != categoryDto.Id) return BadRequest();
            await _categoryService.UpdateAsync(categoryDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            await _categoryService.DeleteAsync(id);
            return NoContent();
        }

        // Web методи
        [HttpGet("/Categories/List")]
        public async Task<IActionResult> List()
        {
            var categories = await _categoryService.GetAllAsync();
            return View(categories);
        }

        [HttpGet("/Categories/Create")]
        [Authorize]
        public IActionResult CreateView()
        {
            return View("Create");
        }

        [HttpPost("/Categories/Create")]
        [Authorize]
        public async Task<IActionResult> CreateView([FromForm] CategoryDto categoryDto)
        {
            await _categoryService.CreateAsync(categoryDto);
            return RedirectToAction("List");
        }

        [HttpGet("/Categories/Edit/{id}")]
        [Authorize]
        public async Task<IActionResult> EditView(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null) return NotFound();
            return View("Edit", category);
        }

        [HttpPost("/Categories/Edit/{id}")]
        [Authorize]
        public async Task<IActionResult> EditView(int id, CategoryDto categoryDto)
        {
            if (id != categoryDto.Id) return BadRequest();
            await _categoryService.UpdateAsync(categoryDto);
            return RedirectToAction("List");
        }

        [HttpGet("/Categories/Delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteView(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null) return NotFound();
            return View("Delete", category);
        }

        [HttpPost("/Categories/Delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _categoryService.DeleteAsync(id);
            return RedirectToAction("List");
        }
    }
}
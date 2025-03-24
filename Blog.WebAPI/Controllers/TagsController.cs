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
    public class TagsController : Controller
    {
        private readonly ITagService _tagService;

        public TagsController(ITagService tagService)
        {
            _tagService = tagService;
        }

        // API методи
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagDto>>> GetAll()
        {
            var tags = await _tagService.GetAllAsync();
            return Ok(tags);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TagDto>> GetById(int id)
        {
            var tag = await _tagService.GetByIdAsync(id);
            if (tag == null) return NotFound();
            return Ok(tag);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<TagDto>> Create(string name)
        {
            var createdTag = await _tagService.CreateAsync(name);
            return CreatedAtAction(nameof(GetById), new { id = createdTag.Id }, createdTag);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, TagDto tagDto)
        {
            if (id != tagDto.Id) return BadRequest();
            await _tagService.UpdateAsync(tagDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            await _tagService.DeleteAsync(id);
            return NoContent();
        }

        // Web методи
        [HttpGet("/Tags/List")]
        public async Task<IActionResult> List()
        {
            var tags = await _tagService.GetAllAsync();
            return View(tags);
        }

        [HttpGet("/Tags/Create")]
        [Authorize]
        public IActionResult CreateView()
        {
            return View("Create");
        }

        [HttpPost("/Tags/Create")]
        [Authorize]
        public async Task<IActionResult> CreateView([FromForm] string name)
        {
            await _tagService.CreateAsync(name);
            return RedirectToAction("List");
        }

        [HttpGet("/Tags/Edit/{id}")]
        [Authorize]
        public async Task<IActionResult> EditView(int id)
        {
            var tag = await _tagService.GetByIdAsync(id);
            if (tag == null) return NotFound();
            return View("Edit", tag);
        }

        [HttpPost("/Tags/Edit/{id}")]
        [Authorize]
        public async Task<IActionResult> EditView(int id, TagDto tagDto)
        {
            if (id != tagDto.Id) return BadRequest();
            await _tagService.UpdateAsync(tagDto);
            return RedirectToAction("List");
        }

        [HttpGet("/Tags/Delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteView(int id)
        {
            var tag = await _tagService.GetByIdAsync(id);
            if (tag == null) return NotFound();
            return View("Delete", tag);
        }

        [HttpPost("/Tags/Delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _tagService.DeleteAsync(id);
            return RedirectToAction("List");
        }
    }
}
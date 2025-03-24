using BlogSystem.BLL.DTO;
using BlogSystem.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Blog.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : Controller
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        // API методи
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetAll()
        {
            var comments = await _commentService.GetByArticleIdAsync(0);
            return Ok(comments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CommentDto>> GetById(int id)
        {
            var comment = await _commentService.GetByIdAsync(id);
            if (comment == null) return NotFound();
            return Ok(comment);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CommentDto>> Create(CommentDto commentDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            commentDto.UserId = userId;
            var createdComment = await _commentService.CreateAsync(commentDto);
            return CreatedAtAction(nameof(GetById), new { id = createdComment.Id }, createdComment);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, CommentDto commentDto)
        {
            if (id != commentDto.Id) return BadRequest();
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var existingComment = await _commentService.GetByIdAsync(id);
            if (existingComment.UserId != userId) return Forbid();
            await _commentService.UpdateAsync(commentDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var existingComment = await _commentService.GetByIdAsync(id);
            if (existingComment.UserId != userId) return Forbid();
            await _commentService.DeleteAsync(id);
            return NoContent();
        }

        // Web методи
        [HttpPost("/Comments/Create")]
        [Authorize]
        public async Task<IActionResult> CreateView(CommentDto commentDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            commentDto.UserId = userId;
            await _commentService.CreateAsync(commentDto);
            return RedirectToAction("Details", "Articles", new { id = commentDto.ArticleId });
        }

        [HttpGet("/Comments/Edit/{id}")]
        [Authorize]
        public async Task<IActionResult> EditView(int id)
        {
            var comment = await _commentService.GetByIdAsync(id);
            if (comment == null) return NotFound();
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (comment.UserId != userId) return Forbid();
            return View("Edit", comment);
        }

        [HttpPost("/Comments/Edit/{id}")]
        [Authorize]
        public async Task<IActionResult> EditView(int id, CommentDto commentDto)
        {
            if (id != commentDto.Id) return BadRequest();
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var existingComment = await _commentService.GetByIdAsync(id);
            if (existingComment.UserId != userId) return Forbid();
            await _commentService.UpdateAsync(commentDto);
            return RedirectToAction("Details", "Articles", new { id = commentDto.ArticleId });
        }

        [HttpGet("/Comments/Delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteView(int id)
        {
            var comment = await _commentService.GetByIdAsync(id);
            if (comment == null) return NotFound();
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (comment.UserId != userId) return Forbid();
            return View("Delete", comment);
        }

        [HttpPost("/Comments/Delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = await _commentService.GetByIdAsync(id);
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (comment.UserId != userId) return Forbid();
            await _commentService.DeleteAsync(id);
            return RedirectToAction("Details", "Articles", new { id = comment.ArticleId });
        }
    }
}
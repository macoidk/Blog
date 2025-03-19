using BlogSystem.BLL.DTO;
using BlogSystem.BLL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CommentDto commentDto)
        {
            if (!HttpContext.Session.GetInt32("UserId").HasValue)
                return RedirectToAction("Login", "Users");

            commentDto.UserId = HttpContext.Session.GetInt32("UserId").Value;
            try
            {
                await _commentService.CreateAsync(commentDto);
            }
            catch (Exception ex)
            {
                // Логування або повернення помилки для діагностики
                TempData["Error"] = $"Помилка при додаванні коментаря: {ex.Message}";
            }
            return RedirectToAction("Details", "Articles", new { id = commentDto.ArticleId });
        }
    }
}
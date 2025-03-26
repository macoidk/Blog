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
    public class ArticlesController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly ICategoryService _categoryService;
        private readonly IUserService _userService;
        private readonly ICommentService _commentService;

        public ArticlesController(IArticleService articleService, ICategoryService categoryService, IUserService userService, ICommentService commentService)
        {
            _articleService = articleService;
            _categoryService = categoryService;
            _userService = userService;
            _commentService = commentService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArticleDto>>> GetAll()
        {
            var articles = await _articleService.GetAllAsync();
            return Ok(articles);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ArticleDto>> GetById(int id)
        {
            var article = await _articleService.GetByIdAsync(id);
            if (article == null) return NotFound();
            return Ok(article);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ArticleDto>> Create([FromBody] ArticleDto articleDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var user = await _userService.GetByIdAsync(userId);
            articleDto.UserId = userId;
            articleDto.AuthorName = user.Username;

            var category = await _categoryService.GetByIdAsync(articleDto.CategoryId);
            articleDto.CategoryName = category.Name;

            var createdArticle = await _articleService.CreateAsync(articleDto);
            return CreatedAtAction(nameof(GetById), new { id = createdArticle.Id }, createdArticle);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, ArticleDto articleDto)
        {
            if (id != articleDto.Id) return BadRequest();
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var existingArticle = await _articleService.GetByIdAsync(id);
            if (existingArticle.UserId != userId) return Forbid();

            var user = await _userService.GetByIdAsync(userId);
            articleDto.AuthorName = user.Username;
            var category = await _categoryService.GetByIdAsync(articleDto.CategoryId);
            if (category == null) return BadRequest("Invalid CategoryId");
            articleDto.CategoryName = category.Name;

            await _articleService.UpdateAsync(articleDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var existingArticle = await _articleService.GetByIdAsync(id);
            if (existingArticle.UserId != userId) return Forbid();
            await _articleService.DeleteAsync(id);
            return NoContent();
        }

        // Web методи
        [HttpGet("/Articles/List")]
        public async Task<IActionResult> List()
        {
            var articles = await _articleService.GetAllAsync();
            return View(articles);
        }

        [HttpGet("/Articles/Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var article = await _articleService.GetByIdAsync(id);
            if (article == null) return NotFound();
            
            var comments = await _commentService.GetRootCommentsByArticleIdAsync(id);
            ViewBag.Comments = comments;
            ViewBag.ArticleId = id;
            
            return View(article);
        }

        [HttpGet("/Articles/Create")]
        [Authorize]
        public async Task<IActionResult> CreateView()
        {
            ViewBag.Categories = await _categoryService.GetAllAsync();
            return View("Create");
        }

        [HttpPost("/Articles/Create")]
        [Authorize]
        public async Task<IActionResult> CreateView([FromForm] ArticleDto articleDto)
        {
            
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var user = await _userService.GetByIdAsync(userId);
            articleDto.UserId = userId;
            articleDto.AuthorName = user.Username; 

            var category = await _categoryService.GetByIdAsync(articleDto.CategoryId);
            if (category == null)
            {
                ViewBag.Categories = await _categoryService.GetAllAsync();
                return View("Create", articleDto);
            }
            articleDto.CategoryName = category.Name;
            
            await _articleService.CreateAsync(articleDto);
            return RedirectToAction("List");
        }

        [HttpGet("/Articles/Edit/{id}")]
        [Authorize]
        public async Task<IActionResult> EditView(int id)
        {
            var article = await _articleService.GetByIdAsync(id);
            if (article == null) return NotFound();
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (article.UserId != userId) return Forbid();
            ViewBag.Categories = await _categoryService.GetAllAsync();
            return View("Edit", article);
        }

        [HttpPost("/Articles/Edit/{id}")]
        [Authorize]
        public async Task<IActionResult> EditView(int id, [FromForm] ArticleDto articleDto)
        {
            if (id != articleDto.Id) return BadRequest();
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var existingArticle = await _articleService.GetByIdAsync(id);
            if (existingArticle.UserId != userId) return Forbid();

            var user = await _userService.GetByIdAsync(userId);
            articleDto.AuthorName = user.Username;
            var category = await _categoryService.GetByIdAsync(articleDto.CategoryId);
            if (category == null) return BadRequest("Invalid CategoryId");
            articleDto.CategoryName = category.Name; 

            await _articleService.UpdateAsync(articleDto);
            return RedirectToAction("List");
        }

        [HttpGet("/Articles/Delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteView(int id)
        {
            var article = await _articleService.GetByIdAsync(id);
            if (article == null) return NotFound();
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (article.UserId != userId) return Forbid();
            return View("Delete", article);
        }

        [HttpPost("/Articles/Delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var existingArticle = await _articleService.GetByIdAsync(id);
            if (existingArticle.UserId != userId) return Forbid();
            await _articleService.DeleteAsync(id);
            return RedirectToAction("List");
        }
    }
}
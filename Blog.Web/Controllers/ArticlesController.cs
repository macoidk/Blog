using BlogSystem.BLL.DTO;
using BlogSystem.BLL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly ICommentService _commentService;
        private readonly ITagService _tagService;
        private readonly ICategoryService _categoryService;
        private readonly IUserService _userService;

        public ArticlesController(IArticleService articleService, ICommentService commentService, 
            ITagService tagService, ICategoryService categoryService, IUserService userService)
        {
            _articleService = articleService;
            _commentService = commentService;
            _tagService = tagService;
            _categoryService = categoryService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var articles = await _articleService.GetAllAsync();
            return View(articles);
        }

        public async Task<IActionResult> Details(int id)
        {
            var article = await _articleService.GetByIdLazyAsync(id);
            if (article == null) return NotFound();

            var comments = await _commentService.GetRootCommentsByArticleIdAsync(id);
            ViewBag.Comments = (comments ?? new List<CommentDto>()) as IEnumerable<CommentDto>;
            ViewBag.ArticleId = id;
            return View(article);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!HttpContext.Session.GetInt32("UserId").HasValue)
                return RedirectToAction("Login", "Users");

            ViewBag.Categories = await _categoryService.GetAllAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ArticleDto articleDto, string tags, string categoryName)
        {
            if (!HttpContext.Session.GetInt32("UserId").HasValue)
                return RedirectToAction("Login", "Users");

            var userId = HttpContext.Session.GetInt32("UserId").Value;
            var user = await _userService.GetByIdAsync(userId);
            if (user == null)
            {
                ViewBag.Error = "Користувач не знайдений. Увійдіть знову.";
                ViewBag.Categories = await _categoryService.GetAllAsync();
                return View(articleDto);
            }

            // Якщо CategoryId не передано або недійсний, створюємо або використовуємо рубрику за назвою
            if (articleDto.CategoryId == 0 && !string.IsNullOrEmpty(categoryName))
            {
                var existingCategory = (await _categoryService.GetAllAsync())
                    .FirstOrDefault(c => c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
                if (existingCategory != null)
                {
                    articleDto.CategoryId = existingCategory.Id;
                }
                else
                {
                    var newCategory = await _categoryService.CreateAsync(new CategoryDto { Name = categoryName });
                    articleDto.CategoryId = newCategory.Id;
                }
            }
            else if (articleDto.CategoryId != 0)
            {
                var category = await _categoryService.GetByIdAsync(articleDto.CategoryId);
                if (category == null)
                {
                    ViewBag.Error = "Обрана рубрика не існує.";
                    ViewBag.Categories = await _categoryService.GetAllAsync();
                    return View(articleDto);
                }
            }
            else
            {
                ViewBag.Error = "Виберіть рубрику зі списку або введіть її назву.";
                ViewBag.Categories = await _categoryService.GetAllAsync();
                return View(articleDto);
            }

            articleDto.UserId = userId;
            try
            {
                var createdArticle = await _articleService.CreateAsync(articleDto);

                if (!string.IsNullOrEmpty(tags))
                {
                    var tagNames = tags.Split(',').Select(t => t.Trim());
                    foreach (var tagName in tagNames)
                    {
                        await _articleService.AddTagAsync(createdArticle.Id, tagName);
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.Categories = await _categoryService.GetAllAsync();
                return View(articleDto);
            }
        }
    }
}
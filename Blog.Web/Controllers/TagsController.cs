using BlogSystem.BLL.DTO;
using BlogSystem.BLL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Controllers
{
    public class TagsController : Controller
    {
        private readonly ITagService _tagService;

        public TagsController(ITagService tagService)
        {
            _tagService = tagService;
        }

        // Список тегів
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var tags = await _tagService.GetAllAsync();
            return View(tags);
        }

        // Деталі тегу
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var tag = await _tagService.GetByIdAsync(id);
            if (tag == null) return NotFound();
            return View(tag);
        }

        // Створення тегу
        [HttpGet]
        public IActionResult Create()
        {
            if (!HttpContext.Session.GetInt32("UserId").HasValue)
                return RedirectToAction("Login", "Users");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            if (!HttpContext.Session.GetInt32("UserId").HasValue)
                return RedirectToAction("Login", "Users");

            try
            {
                await _tagService.CreateAsync(name);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(new TagDto { Name = name });
            }
        }

        // Редагування тегу
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!HttpContext.Session.GetInt32("UserId").HasValue)
                return RedirectToAction("Login", "Users");

            var tag = await _tagService.GetByIdAsync(id);
            if (tag == null) return NotFound();
            return View(tag);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TagDto tagDto)
        {
            if (!HttpContext.Session.GetInt32("UserId").HasValue)
                return RedirectToAction("Login", "Users");

            try
            {
                await _tagService.UpdateAsync(tagDto);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(tagDto);
            }
        }

        // Видалення тегу
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (!HttpContext.Session.GetInt32("UserId").HasValue)
                return RedirectToAction("Login", "Users");

            var tag = await _tagService.GetByIdAsync(id);
            if (tag == null) return NotFound();
            return View(tag);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!HttpContext.Session.GetInt32("UserId").HasValue)
                return RedirectToAction("Login", "Users");

            try
            {
                await _tagService.DeleteAsync(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return RedirectToAction("Delete", new { id });
            }
        }
    }
}
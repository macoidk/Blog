using BlogSystem.BLL.DTO;
using BlogSystem.BLL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // Список рубрик
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllAsync();
            return View(categories);
        }

        // Деталі рубрики
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        // Створення рубрики
        [HttpGet]
        public IActionResult Create()
        {
            if (!HttpContext.Session.GetInt32("UserId").HasValue)
                return RedirectToAction("Login", "Users");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryDto categoryDto)
        {
            if (!HttpContext.Session.GetInt32("UserId").HasValue)
                return RedirectToAction("Login", "Users");

            try
            {
                await _categoryService.CreateAsync(categoryDto);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(categoryDto);
            }
        }

        // Редагування рубрики
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!HttpContext.Session.GetInt32("UserId").HasValue)
                return RedirectToAction("Login", "Users");

            var category = await _categoryService.GetByIdAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryDto categoryDto)
        {
            if (!HttpContext.Session.GetInt32("UserId").HasValue)
                return RedirectToAction("Login", "Users");

            try
            {
                await _categoryService.UpdateAsync(categoryDto);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(categoryDto);
            }
        }

        // Видалення рубрики
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (!HttpContext.Session.GetInt32("UserId").HasValue)
                return RedirectToAction("Login", "Users");

            var category = await _categoryService.GetByIdAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!HttpContext.Session.GetInt32("UserId").HasValue)
                return RedirectToAction("Login", "Users");

            try
            {
                await _categoryService.DeleteAsync(id);
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
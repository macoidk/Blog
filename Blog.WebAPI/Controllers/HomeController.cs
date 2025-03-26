using Microsoft.AspNetCore.Mvc;

namespace Blog.WebAPI.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
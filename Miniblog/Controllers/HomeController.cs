using Microsoft.AspNetCore.Mvc;
using Repo.Interfaces;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        public IRepository Repository { get; private set; }
        public HomeController(IRepository repository)
        {
            Repository = repository;
        }
        public IActionResult Index()
        {
            return RedirectToAction("List", "Articles");
            //return View("~/Views/Articles/List.cs");
        }
    }
}

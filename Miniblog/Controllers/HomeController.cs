using Microsoft.AspNetCore.Mvc;
using Repo.Interfaces;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        public IRepository _repository { get; private set; }
        public HomeController(IRepository repository)
        {
            _repository = repository;
        }
        public IActionResult Index()
        {
            return RedirectToAction("List", "Articles");
            //return View("~/Views/Articles/List.cs");
        }
    }
}

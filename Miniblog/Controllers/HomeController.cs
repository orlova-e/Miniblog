using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Repo.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.App.Interfaces;
using Web.ViewModels;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        public IRepository Repository { get; private set; }
        public ICommon Common { get; set; }

        public HomeController(IRepository repository,
            ICommon common)
        {
            Repository = repository;
            Common = common;
        }
        public IActionResult Index()
        {
            return RedirectToAction("Lists", "Articles");
        }

        public async Task<IActionResult> Topics(int page = 1)
        {
            List<Topic> topics = (await Repository.Topics.GetAllAsync())
                .Where(t => t.Accepted is not false)
                .ToList();
            ListViewModel<Topic> listViewModel = new(page, topics, Common.Options.ListOptions);
            listViewModel.PageName = "Topics";
            if (page > 1 && !listViewModel.Entities.Any())
                return NotFound();
            return View(listViewModel);
        }

        [HttpGet]
        public IActionResult Topic([FromQuery] string name, int page = 1)
        {
            ListViewModel<Article> listViewModel;
            try
            {
                Topic topic = Repository.Topics.Find(t => t.Name == name && t.Accepted is not false).FirstOrDefault();
                if (topic is null)
                    throw new ArgumentNullException();
                var articles = Repository.Articles.Find(a => a.Topic?.Name == topic.Name).ToList();
                listViewModel = new(page, articles, Common.Options.ListOptions);
                listViewModel.PageName = "Topic: " + topic.Name;
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
            if (page > 1 && !listViewModel.Entities.Any())
                return NotFound();

            return View("~/Views/Articles/Lists.cshtml", listViewModel);
        }
    }
}

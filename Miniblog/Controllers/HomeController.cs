using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Repo.Interfaces;
using Services.FoundValues;
using Services.Interfaces;
using Services.Interfaces.Search;
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
        public IListCreator ListCreator { get; set; }
        public ICommon Common { get; set; }
        public IAggregateSearch<Article> ArticlesSearch { get; set; }
        public IAggregateSearch<User> UsersSearch { get; set; }

        public HomeController(IRepository repository,
            IListCreator listCreator,
            ICommon common,
            IAggregateSearch<Article> articlesSearch,
            IAggregateSearch<User> usersSearch)
        {
            Repository = repository;
            ListCreator = listCreator;
            ArticlesSearch = articlesSearch;
            UsersSearch = usersSearch;
            Common = common;
        }
        public IActionResult Index()
        {
            return RedirectToAction("Lists", "Articles");
        }

        public async Task<IActionResult> Search(string query, uint page = 1)
        {
            if (!Common.Options.WebsiteOptions.ShowSearchOption)
                return NotFound();
            List<FoundObject> items = new();
            if (!string.IsNullOrWhiteSpace(query))
            {
                items = await ArticlesSearch.FindAsync(query);
                items = items.Concat(await UsersSearch.FindAsync(query)).ToList();
            }
            ListViewModel<FoundObject> listViewModel = new(page, items, Common.Options.ListOptions);
            listViewModel.PageName = "Search";
            listViewModel.ItemName = query;
            if (page > 1 && !listViewModel.Entities.Any())
                return NotFound();
            return View(listViewModel);
        }

        public async Task<IActionResult> Topics(uint page = 1)
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
        public async Task<IActionResult> Topic([FromQuery] string name, uint page = 1)
        {
            ListViewModel<Article> listViewModel;
            Topic topic = Repository.Topics.Find(t => t.Name == name && t.Accepted is not false).FirstOrDefault();
            if (topic is null)
                return NotFound();
            var articles = (await ListCreator.FindArticlesAsync(a => a.Topic?.Name == topic.Name)).ToList();
            listViewModel = new(page, articles, Common.Options.ListOptions);
            listViewModel.PageName = "Topic";
            listViewModel.ItemName = topic.Name;
            if (page > 1 && !listViewModel.Entities.Any())
                return NotFound();

            return View("~/Views/Articles/Lists.cshtml", listViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Series(uint page = 1)
        {
            List<Series> series = (await Repository.Series.GetAllAsync())
                .Where(s => s.Accepted is not false)
                .ToList();
            ListViewModel<Series> listViewModel = new(page, series, Common.Options.ListOptions);
            listViewModel.PageName = "Series";
            if (page > 1 && !listViewModel.Entities.Any())
                return NotFound();
            return View(listViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> SeriesWith([FromQuery] string name, uint page = 1)
        {
            ListViewModel<Article> listViewModel;
            Series series = Repository.Series.Find(s => s.Link == name && s.Accepted is not false).FirstOrDefault();
            if (series is null)
                return NotFound();
            var articles = (await ListCreator.FindArticlesAsync(a => a.Series?.Name == series.Name)).ToList();
            listViewModel = new(page, articles, Common.Options.ListOptions);
            listViewModel.PageName = "Series";
            listViewModel.ItemName = series.Link;
            if (page > 1 && !listViewModel.Entities.Any())
                return NotFound();

            return View("~/Views/Articles/Lists.cshtml", listViewModel);
        }
    }
}

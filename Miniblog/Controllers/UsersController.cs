using Domain.Entities;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repo.Interfaces;
using Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.App.Interfaces;
using Web.ViewModels;

namespace Web.Controllers
{
    public class UsersController : Controller
    {
        private IArticleService ArticleService { get; set; }
        private IRepository Repository { get; }
        private IListCreator ListCreator { get; set; }
        private IUserService UserService { get; set; }
        private ICommon Common { get; }
        public UsersController(IArticleService articleService,
            IRepository repository,
            IListCreator listCreator,
            IUserService userService,
            ICommon common)
        {
            ArticleService = articleService;
            Repository = repository;
            ListCreator = listCreator;
            UserService = userService;
            Common = common;
        }

        [HttpGet]
        [Route("[controller]/[action]/{username}")]
        public async Task<IActionResult> Account(string username, uint page = 1, ListSorting sortBy = ListSorting.NewFirst)
        {
            User author = UserService.GetUserFromDb(u => u.Username == username);

            if (author is null)
                return NotFound();

            List<Article> articles = await ListCreator.FindArticlesAsync(a => a.User?.Id == author.Id);
            ListViewModel<Article> listViewModel = new(page, articles, Common.Options.ListOptions, sortBy);
            listViewModel.PageName = "Account";
            listViewModel.ItemName = author.Username;

            bool subscribed = false;
            if (User.Identity.IsAuthenticated)
            {
                User currentUser = UserService.FindByName(User.Identity.Name);
                if (author.Subscribers.Contains(currentUser))
                    subscribed = true;
            }

            ViewBag.Author = author;
            ViewBag.ListViewModel = listViewModel;
            ViewBag.Subscribed = subscribed;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Authors(uint page = 1)
        {
            var roles = (await Repository.Roles.GetAllAsync())
                .Where(r => r.WriteArticles);
            List<User> authors = (await Repository.Users.FindAsync(u => roles.Contains(u.Role) && u.Accepted is not false))?.ToList();
            ListViewModel<User> listViewModel = new(page, authors, Common.Options.ListOptions);
            listViewModel.PageName = "Authors";

            if (page > 1 && !listViewModel.Entities.Any())
                return NotFound();

            return View(listViewModel);
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> Subscriptions(uint page = 1)
        {
            User user = UserService.FindByName(User.Identity.Name);
            List<User> subscriptions = await Repository.Subscriptions.GetSubscriptionAsync(user.Id);
            ListViewModel<User> listViewModel = new(page, subscriptions, Common.Options.ListOptions);
            listViewModel.PageName = "Subscriptions";
            listViewModel.ItemName = user.Username;

            return View("Authors", listViewModel);
        }
    }
}

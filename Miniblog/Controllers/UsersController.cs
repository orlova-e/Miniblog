using Domain.Entities;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.App.Interfaces;
using Web.ViewModels;

namespace Web.Controllers
{
    public class UsersController : Controller
    {
        private IArticleService ArticleService { get; set; }
        private IListCreator ListCreator { get; set; }
        private IUserService UserService { get; set; }
        private ICommon Common { get; }
        public UsersController(IArticleService articleService,
            IListCreator listCreator,
            IUserService userService,
            ICommon common)
        {
            ArticleService = articleService;
            ListCreator = listCreator;
            UserService = userService;
            Common = common;
        }
        [HttpGet]
        [Route("[controller]/{username}")]
        public async Task<IActionResult> Account(string username, int page = 1, ListSorting sortBy = ListSorting.NewFirst)
        {
            User author = UserService.GetUserFromDb(u => u.Username == username);

            if (author is null)
                return NotFound();

            List<Article> articles = await ListCreator.FindArticlesAsync(a => a.User?.Id == author.Id);
            ListViewModel listViewModel = new(page, articles, Common.Options.ListOptions, sortBy);
            listViewModel.PageName = "Account";

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
    }
}

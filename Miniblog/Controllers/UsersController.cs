using Domain.Entities;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.App.Interfaces;
using Web.ViewModels;

namespace Web.Controllers
{
    public class UsersController : Controller
    {
        public IArticleService ArticleService { get; private set; }
        public IListPreparer ListPreparer { get; set; }
        public IListCreator ListCreator { get; private set; }
        public IUserService UserService { get; private set; }
        public UsersController(IArticleService articleService,
            IListPreparer listPreparer,
            IListCreator listCreator,
            IUserService userService)
        {
            ArticleService = articleService;
            ListPreparer = listPreparer;
            ListCreator = listCreator;
            UserService = userService;
        }
        [HttpGet]
        [Route("[controller]/{username}")]
        public async Task<IActionResult> Account([FromRoute] string username,
            [FromQuery] uint page = 1,
            [FromQuery] ListSorting sortby = ListSorting.NewFirst)
        {
            User author = UserService.GetUserFromDb(u => u.Username == username);

            if (author == null)
                return NotFound();

            List<Article> articles = await ListCreator.FindArticlesAsync(a => a.User.Id == author.Id);
            ListViewModel listViewModel = ListPreparer.GetListModel(articles, page, sortby);
            listViewModel.PageName = "Account";

            bool subscribed = false;
            if (User.Identity.IsAuthenticated)
            {
                Guid.TryParse(User.FindFirstValue("Id"), out Guid currentUserId);
                User currentUser = await UserService.GetFromDbAsync(currentUserId);
                if (author.Subscribers.Contains(currentUser))
                    subscribed = true;
            }

            ViewBag.Author = author;
            //ViewBag.Articles = articles;
            ViewBag.ListViewModel = listViewModel;
            ViewBag.Subscribed = subscribed;

            return View();
        }
    }
}

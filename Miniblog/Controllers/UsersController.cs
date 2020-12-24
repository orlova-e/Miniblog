using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Miniblog.ViewModels;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Miniblog.Controllers
{
    public class UsersController : Controller
    {
        public IArticleService articleService { get; private set; }
        public IListPreparer listPreparer { get; set; }
        public IListCreator listCreator { get; private set; }
        public IUserService userService { get; private set; }
        public UsersController(IArticleService articleService,
            IListPreparer listPreparer,
            IListCreator listCreator,
            IUserService userService)
        {
            this.articleService = articleService;
            this.listPreparer = listPreparer;
            this.listCreator = listCreator;
            this.userService = userService;
        }
        [HttpGet]
        [Route("[controller]/{username}")]
        public async Task<IActionResult> Account([FromRoute]string username, [FromQuery] uint page = 1, string sortby = "newfirst")
        {
            User author = userService.GetUserFromDb(u => u.Username == username);

            if (author == null)
                return NotFound();

            List<Article> articles = await listCreator.FindArticlesAsync(a => a.User.Id == author.Id);
            ListViewModel listViewModel = listPreparer.GetListModel(articles, page, sortby);
            listViewModel.PageName = "Account";

            bool subscribed = false;
            if (User.Identity.IsAuthenticated)
            {
                Guid.TryParse(User.FindFirstValue("Id"), out Guid currentUserId);
                User currentUser = await userService.GetFromDbAsync(currentUserId);
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

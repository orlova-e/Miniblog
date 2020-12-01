using Microsoft.AspNetCore.Mvc;
using Miniblog.Models.App.Interfaces;
using Miniblog.Models.Entities;
using Miniblog.Models.Services.Interfaces;
using Miniblog.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Miniblog.Controllers
{
    public class UsersController : Controller
    {
        public IRepository repository { get; private set; }
        public IArticlesService articleService { get; private set; }
        public IListService listService { get; set; }
        public UsersController(IRepository repository, IArticlesService articleService, IListService listService)
        {
            this.repository = repository;
            this.articleService = articleService;
            this.listService = listService;
        }
        [HttpGet]
        [Route("[controller]/{username}")]
        public async Task<IActionResult> Account([FromRoute]string username, [FromQuery] uint page = 1, string sortby = "newfirst")
        {
            User author = repository.Users.Find(u => u.Username == username).FirstOrDefault();

            if (author == null)
                return NotFound();

            List<Article> articles = await listService.FindArticlesAsync(a => a.User.Id == author.Id);
            ListViewModel listViewModel = listService.GetListModel(articles, page, sortby);
            listViewModel.PageName = "Account";

            bool subscribed = false;
            if (User.Identity.IsAuthenticated)
            {
                Guid.TryParse(User.FindFirstValue("Id"), out Guid currentUserId);
                User currentUser = await repository.Users.GetByIdAsync(currentUserId);
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

using Microsoft.AspNetCore.Mvc;
using Miniblog.Models.App.Interfaces;
using Miniblog.Models.Entities;
using Miniblog.Models.Services.Interfaces;
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
        public IArticlesService articlesService { get; private set; }
        public UsersController(IRepository repository, IArticlesService articlesService)
        {
            this.repository = repository;
            this.articlesService = articlesService;
        }
        [HttpGet]
        [Route("[controller]/{username}")]
        public async Task<IActionResult> Account([FromRoute]string username)
        {
            User author = repository.Users.Find(u => u.Username == username).FirstOrDefault();

            if (author == null)
                return NotFound();

            List<Article> articles = await articlesService.FindArticlesAsync(a => a.User.Id == author.Id);

            bool subscribed = false;
            if (User.Identity.IsAuthenticated)
            {
                Guid.TryParse(User.FindFirstValue("Id"), out Guid currentUserId);
                User currentUser = await repository.Users.GetByIdAsync(currentUserId);
                if (author.Subscribers.Contains(currentUser))
                    subscribed = true;
            }

            ViewBag.Author = author;
            ViewBag.Articles = articles;
            ViewBag.Subscribed = subscribed;

            return View();
        }
    }
}

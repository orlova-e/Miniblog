using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Miniblog.Filters;
using Miniblog.Models.App.Interfaces;
using Miniblog.Models.Entities;
using Miniblog.Models.Entities.Enums;
using Miniblog.Models.Services.Interfaces;
using Miniblog.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Miniblog.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class ArticlesController : Controller
    {
        public IRepository repository { get; private set; }
        public IArticlesService articlesService { get; private set; }

        public ArticlesController(IRepository repository, IArticlesService articlesService)
        {
            this.repository = repository;
            this.articlesService = articlesService;
        }

        [AllowAnonymous]
        [Route("[controller]/")]
        public async Task<IActionResult> Article([FromQuery] string title)
        {
            if (!articlesService.HasArticle(a => a.Link == title)) // ???????????? будет ли линк декодирована???
            {
                return NotFound();
            }

            Article article = await articlesService.GetArticleByLinkAsync(title);

            User currentUser = null;
            if (User.Identity.IsAuthenticated)
            {
                Guid.TryParse(User.FindFirstValue("Id"), out Guid userId);
                currentUser = await repository.Users.GetByIdAsync(userId);
                if(currentUser.Role == null)
                {
                    Role role = await repository.Roles.GetByIdAsync(currentUser.RoleId);
                    currentUser.Role = role;
                }
            }

            ViewBag.CurrentUser = currentUser;
            ViewBag.Article = article;

            //ArticleReadViewModel articleViewModel = new ArticleReadViewModel()
            //{
            //    Article = article,
            //    CurrentUser = currentUser,
            //    //CommentForm = new CommentViewModel(),
            //    //Comments 
            //};

            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> List([FromRoute] string name = "Recent", bool isSeries = false) // create method for series ?
        {
            List<Article> articles = (await articlesService
                .FindArticlesAsync(a => a.EntryType == EntryType.Article && a.Visibility == true))
                .OrderByDescending(a => a.DateTime.Ticks).ToList();

            return View(articles);
        }
        
        [HttpGet]
        [TypeFilter(typeof(AccessAttribute), Arguments = new[] { "WriteArticles" })]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [TypeFilter(typeof(AccessAttribute), Arguments = new[] { "WriteArticles" })]
        public async Task<IActionResult> Add(ArticleWriteViewModel articleViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(articleViewModel);
            }

            Guid.TryParse(User.FindFirstValue("Id"), out Guid userId);

            Article article = await articlesService.CreateArticleAsync(userId, articleViewModel);

            return RedirectToAction("Article", "Articles", new { title = article.Link });
        }
    }
}

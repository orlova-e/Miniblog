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
        [HttpGet]
        [Route("[controller]/")]
        public async Task<IActionResult> Article([FromQuery] string title)
        {
            if (!articlesService.HasArticle(a => a.Link == title))
            {
                return NotFound();
            }

            Article article = await articlesService.GetArticleByLinkAsync(title);
            ListDisplayOptions listOptions = await repository.ListDisplayOptions.FirstOrDefaultAsync();
            if (listOptions.OverrideForUserArticle)
            {
                article.DisplayOptions = (ArticleOptions)listOptions;
            }

            User user = null;
            if (User.Identity.IsAuthenticated)
            {
                Guid.TryParse(User.FindFirstValue("Id"), out Guid userId);
                user = await repository.Users.GetByIdAsync(userId);
                if(user?.Role == null)
                {
                    user.Role = await repository.Roles.GetByIdAsync(user.RoleId);
                }
            }

            CommentsOptions commentsOptions = await repository.CommentsOptions.FirstOrDefaultAsync();

            ArticleReadViewModel articleReadModel = new ArticleReadViewModel
            {
                Article = article,
                User = user
            };

            CommentsViewModel commentsViewModel = new CommentsViewModel
            {
                User = user,
                Depth = commentsOptions.AllowNesting ? commentsOptions.Depth : 0,
                WriteComments = user?.Role?.WriteComments ?? default,
                CommentsVisibility = article.DisplayOptions.Comments,
                Comments = repository.Comments.Find(c => c.ArticleId == article.Id).ToList()
            };

            ViewBag.ArticleReadModel = articleReadModel;
            ViewBag.CommentsViewModel = commentsViewModel;

            return View();
        }

        [AllowAnonymous]
        [HttpGet]
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

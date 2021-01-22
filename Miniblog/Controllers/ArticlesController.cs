using Domain.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Repo.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.App.Interfaces;
using Web.Configuration;
using Web.Filters;
using Web.ViewModels;

namespace Web.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class ArticlesController : Controller
    {
        public BlogOptions BlogOptions { get; set; }
        public IRepository Repository { get; private set; }
        public IArticleService ArticleService { get; private set; }
        public IListPreparer ListPreparer { get; private set; }
        public IListCreator ListCreator { get; private set; }
        public IUserService UserService { get; private set; }

        public ArticlesController(IRepository repository,
            IArticleService articleService,
            IListPreparer listPreparer,
            IListCreator listCreator,
            IUserService userService,
            IOptionsSnapshot<BlogOptions> optionsSnapshot)
        {
            Repository = repository;
            ArticleService = articleService;
            ListPreparer = listPreparer;
            ListCreator = listCreator;
            UserService = userService;
            BlogOptions = optionsSnapshot.Value;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("[controller]/")]
        public async Task<IActionResult> Article([FromQuery] string title)
        {
            if (!ArticleService.HasArticle(a => a.Link == title))
            {
                return NotFound();
            }

            Article article = await ArticleService.GetArticleByLinkAsync(title);
            ListOptions listOptions = BlogOptions.ListOptions;
            if (listOptions.OverrideForUserArticle)
            {
                article.DisplayOptions = (ArticleOptions)listOptions;
                article.DisplayOptions.ColorTheme = BlogOptions.WebsiteOptions.ColorTheme;
            }

            User user = null;
            if (User.Identity.IsAuthenticated)
            {
                Guid.TryParse(User.FindFirstValue("Id"), out Guid userId);
                user = await UserService.GetFromDbAsync(userId);
                if (user?.Role == null)
                {
                    user.Role = await Repository.Roles.GetByIdAsync(user.RoleId);
                }
            }

            CommentsOptions commentsOptions = BlogOptions.CommentsOptions;

            ArticleReadViewModel articleReadModel = new ArticleReadViewModel
            {
                Article = article,
                User = user
            };

            CommentsViewModel commentsViewModel = new CommentsViewModel
            {
                User = user,
                Depth = commentsOptions.AllowNesting ? commentsOptions.Depth.Value : 0,
                WriteComments = user?.Role?.WriteComments ?? default,
                CommentsVisibility = article.DisplayOptions.Comments,
                Comments = Repository.Comments.Find(c => c.ArticleId == article.Id).ToList()
            };

            ViewBag.ArticleReadModel = articleReadModel;
            ViewBag.CommentsViewModel = commentsViewModel;

            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] uint page = 1, string sortby = "newfirst") // create method for series ?
        {
            List<Article> articles = await ListCreator
                .FindArticlesAsync(a => true);
            ListViewModel listViewModel = ListPreparer.GetListModel(articles, page, sortby);
            listViewModel.PageName = "List";
            return View(listViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Favourites([FromQuery] uint page = 1, string sortby = "newfirst")
        {
            Guid.TryParse(User.FindFirstValue("Id"), out Guid userId);
            List<Article> articles = await ListCreator.GetFavouritesAsync(userId);
            ListViewModel listViewModel = ListPreparer.GetListModel(articles, page, sortby);
            listViewModel.PageName = "Favourites";
            return View("~/Views/Articles/Saved.cshtml", listViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Bookmarks([FromQuery] uint page = 1, string sortby = "newfisrt")
        {
            Guid.TryParse(User.FindFirstValue("Id"), out Guid userId);
            List<Article> articles = await ListCreator.GetBookmarkedAsync(userId);
            ListViewModel listViewModel = ListPreparer.GetListModel(articles, page, sortby);
            listViewModel.PageName = "Bookmarks";
            return View("~/Views/Articles/Saved.cshtml", listViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Commented([FromQuery] uint page = 1, string sortby = "newfisrt")
        {
            Guid.TryParse(User.FindFirstValue("Id"), out Guid userId);
            List<Article> articles = await ListCreator.GetCommentedAsync(userId);
            ListViewModel listViewModel = ListPreparer.GetListModel(articles, page, sortby);
            listViewModel.PageName = "Commented";
            return View("~/Views/Articles/Saved.cshtml", listViewModel);
        }

        [HttpGet]
        public IActionResult Drafts([FromQuery] uint page = 1, string sortby = "newfisrt")
        {
            Guid.TryParse(User.FindFirstValue("Id"), out Guid userId);
            List<Article> articles = ListCreator.FindDrafts(userId);
            ListViewModel listViewModel = ListPreparer.GetListModel(articles, page, sortby);
            listViewModel.PageName = "Drafts";
            return View("~/Views/Articles/Saved.cshtml", listViewModel);
        }

        [HttpGet]
        [TypeFilter(typeof(AccessAttribute), Arguments = new[] { "WriteArticles" })]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [TypeFilter(typeof(AccessAttribute), Arguments = new[] { "WriteArticles" })]
        public async Task<IActionResult> Add(NewArticle articleViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(articleViewModel);
            }

            Guid.TryParse(User.FindFirstValue("Id"), out Guid userId);

            Article article = await ArticleService.CreateArticleAsync(userId, articleViewModel);

            return RedirectToAction("Article", "Articles", new { title = article.Link });
        }
    }
}

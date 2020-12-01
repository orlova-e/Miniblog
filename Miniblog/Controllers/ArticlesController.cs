using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Miniblog.Configuration;
using Miniblog.Filters;
using Miniblog.Models.App.Interfaces;
using Miniblog.Models.Entities;
using Miniblog.Models.Services.Interfaces;
using Miniblog.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Miniblog.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class ArticlesController : Controller
    {
        public BlogOptions BlogOptions { get; set; }
        public IRepository repository { get; private set; }
        public IArticlesService articleService { get; private set; }
        public IListService listService { get; private set; }
        public IUserService userService { get; private set; }

        //public Configuration.WebsiteOptions WebsiteOptions { get; private set; }
        //public Roles Roles { get; private set; }
        //public ListOptions ListOptions { get; private set; }
        //public Configuration.CommentsOptions CommentsOptions { get; private set; }

        public ArticlesController(IRepository repository,
            IArticlesService articleService,
            IListService listService,
            IUserService userService,
            IOptionsSnapshot<BlogOptions> optionsSnapshot)
        {
            this.repository = repository;
            this.articleService = articleService;
            this.listService = listService;
            this.userService = userService;
            this.BlogOptions = optionsSnapshot.Value;
            //WebsiteOptions = websiteOptions.Value;
            //Roles = roles.Value;
            //ListOptions = listOptions.Value;
            //CommentsOptions = commentsOptions.Value;
            //CommentsOptions.Depth.Value = 2;

            //configuration[""]
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("[controller]/")]
        public async Task<IActionResult> Article([FromQuery] string title)
        {
            if (!articleService.HasArticle(a => a.Link == title))
            {
                return NotFound();
            }

            Article article = await articleService.GetArticleByLinkAsync(title);
            ListOptions listOptions = BlogOptions.ListOptions;
            if (listOptions.OverrideForUserArticle)
            {
                article.DisplayOptions = (ArticleOptions)listOptions;
            }

            User user = null;
            if (User.Identity.IsAuthenticated)
            {
                Guid.TryParse(User.FindFirstValue("Id"), out Guid userId);
                user = await userService.GetFromDbAsync(userId);
                if(user?.Role == null)
                {
                    user.Role = await repository.Roles.GetByIdAsync(user.RoleId);
                }
            }

            Configuration.CommentsOptions commentsOptions = BlogOptions.CommentsOptions;

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
                Comments = repository.Comments.Find(c => c.ArticleId == article.Id).ToList()
            };

            ViewBag.ArticleReadModel = articleReadModel;
            ViewBag.CommentsViewModel = commentsViewModel;

            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] uint page = 1, string sortby = "newfirst") // create method for series ?
        {
            List<Article> articles = await listService
                .FindArticlesAsync(a => true);
            ListViewModel listViewModel = listService.GetListModel(articles, page, sortby);
            listViewModel.PageName = "List";
            return View(listViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Favourites([FromQuery] uint page = 1, string sortby = "newfirst")
        {
            Guid.TryParse(User.FindFirstValue("Id"), out Guid userId);
            List<Article> articles = await listService.GetFavouritesAsync(userId);
            ListViewModel listViewModel = listService.GetListModel(articles, page, sortby);
            listViewModel.PageName = "Favourites";
            return View("~/Views/Articles/Saved.cshtml", listViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Bookmarks([FromQuery] uint page = 1, string sortby = "newfisrt")
        {
            Guid.TryParse(User.FindFirstValue("Id"), out Guid userId);
            List<Article> articles = await listService.GetBookmarkedAsync(userId);
            ListViewModel listViewModel = listService.GetListModel(articles, page, sortby);
            listViewModel.PageName = "Bookmarks";
            return View("~/Views/Articles/Saved.cshtml", listViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Commented([FromQuery] uint page = 1, string sortby = "newfisrt")
        {
            Guid.TryParse(User.FindFirstValue("Id"), out Guid userId);
            List<Article> articles = await listService.GetCommentedAsync(userId);
            ListViewModel listViewModel = listService.GetListModel(articles, page, sortby);
            listViewModel.PageName = "Commented";
            return View("~/Views/Articles/Saved.cshtml", listViewModel);
        }

        [HttpGet]
        public IActionResult Drafts([FromQuery] uint page = 1, string sortby = "newfisrt")
        {
            Guid.TryParse(User.FindFirstValue("Id"), out Guid userId);
            List<Article> articles = listService.FindDrafts(userId);
            ListViewModel listViewModel = listService.GetListModel(articles, page, sortby);
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
        public async Task<IActionResult> Add(ArticleWriteViewModel articleViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(articleViewModel);
            }

            Guid.TryParse(User.FindFirstValue("Id"), out Guid userId);

            Article article = await articleService.CreateArticleAsync(userId, articleViewModel);

            return RedirectToAction("Article", "Articles", new { title = article.Link });
        }
    }
}

using Domain.Entities;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.App.Interfaces;
using Web.Configuration;
using Web.ViewModels;

namespace Web.Controllers
{
    [Authorize]
    public class ArticlesController : Controller
    {
        private IArticleService ArticleService { get; }
        private IListCreator ListCreator { get; }
        private IUserService UserService { get; }
        private ICommon Common { get; }

        public ArticlesController(IArticleService articleService,
            IListCreator listCreator,
            IUserService userService,
            ICommon common)
        {
            ArticleService = articleService;
            ListCreator = listCreator;
            UserService = userService;
            Common = common;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("[controller]/")]
        public async Task<IActionResult> Article(string title)
        {
            if (!ArticleService.HasArticle(a => a.Link == title))
            {
                return NotFound();
            }

            Article article = await ArticleService.GetArticleByLinkAsync(title);
            ListOptions listOptions = Common.Options.ListOptions;
            if (!listOptions.OverrideForUserArticle)
            {
                article.DisplayOptions = (ArticleOptions)listOptions;
                article.DisplayOptions.ColorTheme = Common.Options.WebsiteOptions.ColorTheme;
            }

            User user = null;
            if (User.Identity.IsAuthenticated)
            {
                user = UserService.FindByName(User.Identity.Name);
            }

            CommentsOptions commentsOptions = Common.Options.CommentsOptions;

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
                Comments = article.Comments
            };

            ViewBag.ArticleReadModel = articleReadModel;
            ViewBag.CommentsViewModel = commentsViewModel;

            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("[controller]/[action]/{listName=default}")]
        public async Task<IActionResult> Lists(string listName, uint page = 1, ListSorting sortBy = ListSorting.NewFirst)
        {
            List<Article> articles = new();

            try
            {
                Guid.TryParse(User.FindFirstValue("Id"), out Guid userId);
                articles = listName.ToLower() switch
                {
                    "default" => await ListCreator.FindArticlesAsync(a => true),
                    "pages" => ListCreator.FindEntries(a => a.EntryType is EntryType.Page),
                    "favourites" => await ListCreator.GetFavouritesAsync(userId),
                    "bookmarks" => await ListCreator.GetBookmarkedAsync(userId),
                    "commented" => await ListCreator.GetCommentedAsync(userId),
                    "drafts" => ListCreator.FindDrafts(userId),
                    _ => throw new ArgumentException("Undefined list", nameof(listName))
                };
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
            catch (ArgumentException)
            {
                return NotFound();
            }

            ListViewModel<Article> listViewModel = new(page, articles, Common.Options.ListOptions, sortBy);
            listViewModel.PageName = listName;
            if (page > 1 && !listViewModel.Entities.Any())
                return NotFound();
            if (listName is "default" or "pages")
                return View(listViewModel);
            return View("~/Views/Articles/Saved.cshtml", listViewModel);
        }

        [HttpGet]
        public IActionResult Add(bool page = false)
        {
            Role role = Common.GetRole(User);
            if (!role.WriteArticles)
                return NotFound();
            ArticleData article = new();
            if (page)
            {
                if (role is ExtendedRole extended && extended.OverrideMenu)
                {
                    article.EntryType = EntryType.Page;
                }
                else
                {
                    return NotFound();
                }
            }
            return View(article);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ArticleData articleData)
        {
            if (!ModelState.IsValid)
            {
                return View(articleData);
            }

            User user = UserService.FindByName(User.Identity.Name);
            if (!user.Role.WriteArticles)
                return NotFound();
            articleData.User = user;
            Article article = await ArticleService.CreateArticleAsync(articleData);

            return RedirectToAction("Article", "Articles", new { title = article.Link });
        }
    }
}

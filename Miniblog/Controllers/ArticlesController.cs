using Domain.Entities;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
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

        [HttpGet]
        public IActionResult Add(bool page = false)
        {
            Role role = UserService.FindByName(User.Identity.Name).Role;
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

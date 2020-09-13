using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Miniblog.Filters;
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
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class ArticlesController : Controller
    {
        public IRepository repository { get; private set; }
        public ArticlesController(IRepository repository)
        {
            this.repository = repository;
        }

        [AllowAnonymous]
        [Route("[controller]/{link}")]
        public async Task<IActionResult> Article([FromRoute] string link)
        {
            Article article = repository.Articles.Find(a => a.Link == link).FirstOrDefault();
            
            if(article == null)
            {
                return NotFound();
            }            
            
            ListDisplayOptions globalArticleOptions = await repository.ListDisplayOptions.FirstOrDefaultAsync();
            if (!globalArticleOptions.OverrideForUserArticle)
                article.DisplayOptions = (ArticleOptions)globalArticleOptions;

            // ! test !     // SHOULD INCLUDE USERS' AVATARS (USERS' INSTANSES) !!!!!
            if (article.Comments == null)
            {
                List<Comment> comments = repository.Comments.Find(c => c.ArticleId == article.Id).ToList();
                article.Comments = comments;
            }
            if (article.Images == null)
            {

            }
            if (article.Topic == null && article.TopicId != null)
            {
                Topic topic = await repository.Topics.GetByIdAsync((Guid)article.TopicId);
                article.Topic = topic;
            }
            if (article.User == null)
            {
                User user = await repository.Users.GetByIdAsync(article.UserId);
                article.User = user;
            }

            article.Likes = await repository.ArticleLikes.GetAsync(article.Id);
            article.Bookmarks = await repository.ArticleBookmarks.GetAsync(article.Id);

            for (int i = 0; i < article.Comments.Count; i++)
            {
                article.Comments[i].Likes = await repository.CommentLikes.GetAsync(article.Comments[i].Id);
            }

            Guid.TryParse(User.FindFirstValue("Id"), out Guid userId);
            User currentUser = await repository.Users.GetByIdAsync(userId);

            ArticleReadViewModel articleViewModel = new ArticleReadViewModel()
            {
                Article = article,
                //CommentForm = new CommentViewModel(),
                CurrentUser = currentUser,
                //Comments 
            };

            return View(articleViewModel);
        }

        [AllowAnonymous]
        public async Task<IActionResult> List([FromRoute] string listName = "Recent")
        {
            ListDisplayOptions listOptions = await repository.ListDisplayOptions.FirstOrDefaultAsync();
            List<ArticleFromListViewModel> articlesModel = new List<ArticleFromListViewModel>();
            List<Article> articles = (await repository.Articles.GetAllAsync()).OrderByDescending(a => a.DateTime.Ticks).ToList();
            for (int i = 0; i < articles.Count; i++)
            {
                if (articles[i].User == null)
                    articles[i].User = await repository.Users.GetByIdAsync(articles[i].UserId);
                if (listOptions.OverrideForUserArticle && articles[i].DisplayOptions == null)
                    articles[i].DisplayOptions = repository.ArticleOptions.Find(d => d.ArticleId == articles[i].Id).FirstOrDefault();
                if (articles[i].Topic == null)
                    articles[i].Topic = await repository.Topics.GetByIdAsync((Guid)articles[i].TopicId);

                if (!articles[i].Comments.Any())
                    articles[i].Comments = repository.Comments.Find(c => c.ArticleId == articles[i].Id).ToList();
                if (!articles[i].Bookmarks.Any())
                    articles[i].Bookmarks = await repository.ArticleBookmarks.GetAsync(articles[i].Id);
                if (!articles[i].Likes.Any())
                    articles[i].Likes = await repository.ArticleLikes.GetAsync(articles[i].Id);
            }

            return View(articles);
        }
        
        [HttpGet]
        [TypeFilter(typeof(AccessAttribute), Arguments = new[] { "WriteArticles" })]
        public IActionResult Add()
        {
            //ViewBag.Article = new Article();
            //ViewBag.ArticleOptions = new UserArticleDisplayOptions();
            //return View(ViewBag);
            return View();
        }

        [HttpPost]
        [TypeFilter(typeof(AccessAttribute), Arguments = new[] { "WriteArticles" })]
        public IActionResult Add(ArticleWriteViewModel article)
        {
            if (!ModelState.IsValid)
            {
                return View(article);
            }

            return RedirectToAction("GetArticleAsync", article.Header);
        }
    }
}

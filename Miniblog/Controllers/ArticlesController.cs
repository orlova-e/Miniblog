using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Miniblog.Models.Entities;
using Miniblog.Models.Services.Interfaces;
using Miniblog.ViewModels;

namespace Miniblog.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class ArticlesController : Controller
    {
        public IRepository _repository { get; private set; }
        public ArticlesController(IRepository repository)
        {
            _repository = repository;
        }
        [AllowAnonymous]
        [Route("[controller]/{header}")]
        public async Task<IActionResult> GetArticleAsync(string header)
        {
            Article article = _repository.Articles.Find(a => a.Header == header).First();
            if (article.Comments == null)
            {
                List<Comment> comments = _repository.Comments.Find(c => c.ArticleId == article.Id).ToList();
                article.Comments = comments;
            }
            if (article.Images == null)
            {

            }
            if (article.Topic == null && article.TopicId != null)
            {
                Topic topic = await _repository.Topics.GetByIdAsync((Guid)article.TopicId);
                article.Topic = topic;
            }
            if (article.User == null)
            {
                User user = await _repository.Users.GetByIdAsync(article.UserId);
                article.User = user;
            }

            Guid.TryParse(User.FindFirstValue("Id"), out Guid userId);
            User currentUser = await _repository.Users.GetByIdAsync(userId);

            ArticleViewModel articleViewModel = new ArticleViewModel()
            {
                Article = article,
                //CommentForm = new CommentViewModel(),
                CurrentUser = currentUser,
                //Comments 
            };

            return View(articleViewModel);
        }
        // auth requirements
        [HttpGet]
        public IActionResult AddArticle()
        {
            //ViewBag.Article = new Article();
            //ViewBag.ArticleOptions = new UserArticleDisplayOptions();
            //return View(ViewBag);
            return View();
        }
        [HttpPost]
        public IActionResult AddArticle(Article article, ArticleOptions articleOptions = null)
        {
            if (!ModelState.IsValid)
            {
                //ViewBag.Article = article;
                //ViewBag.ArticleOptions = articleOptions;
                //return View(ViewBag);
                return View(article);
            }

            return RedirectToAction("GetArticleAsync", article.Header);
        }
    }
}

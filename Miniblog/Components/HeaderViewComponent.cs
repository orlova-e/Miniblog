using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Miniblog.Models.Entities;
using Miniblog.Models.Entities.Enums;
using Miniblog.Models.Services.Interfaces;
using Miniblog.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Miniblog.Configuration;

namespace Miniblog.Components
{
    public class HeaderViewComponent : ViewComponent
    {
        public BlogOptions BlogOptions { get; private set; }
        public IRepository _repository { get; set; }
        public HeaderViewComponent(IRepository repository,
            IOptionsSnapshot<BlogOptions> optionsSnapshot)
        {
            BlogOptions = optionsSnapshot.Value;
            _repository = repository;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            Configuration.WebsiteOptions websiteDisplayOptions = BlogOptions.WebsiteOptions;
            HeaderViewModel header = new HeaderViewModel()
            {
                Title = websiteDisplayOptions.Name,
                ShowSearch = websiteDisplayOptions.ShowSearchOption,
            };

            Dictionary<string, string> pages = new Dictionary<string, string>();

            ClaimsPrincipal principal;
            User user;
            Role role = null;

            if (User.Identity.IsAuthenticated)
            {
                principal = User as ClaimsPrincipal;
                Guid.TryParse(principal.FindFirstValue("Id"), out Guid id);
                user = await _repository.Users.GetByIdAsync(id);
                header.Username = user.Username;
                header.Avatar = user.Avatar;
                role = await _repository.Roles.GetByIdAsync(user.RoleId);
                if (role.WriteArticles)
                {
                    pages.Add("Add", Url.Action("Add", "Articles"));
                }
            }

            if (websiteDisplayOptions.ShowListOfPopularAndRecent)
            {
                pages.Add("Popular", Url.Action("List", "Articles", new { name = "Popular" }));
            }
            if (websiteDisplayOptions.ShowAuthors)
            {
                pages.Add("Authors", Url.Action("List", "Articles", new { name = "Authors" }));
            }
            if (websiteDisplayOptions.ShowTopics)
            {
                pages.Add("Topics", Url.Action("List", "Articles", new { name = "Topics" }));
            }

            List<Article> pagesDb = _repository.Articles
                .Find(page => page.EntryType == EntryType.Page && page.Visibility == true && page.MenuVisibility == true)
                .ToList();

            foreach(Article page in pagesDb)
            {
                pages.Add(page.Header, Url.Action("Article", "Articles", new { link = page.Link }));
            }

            if (User.Identity.IsAuthenticated)
            {
                pages.Add("Account", Url.Action("Account", "Options"));
                if(role.Discriminator == "ExtendedRole")
                {
                    pages.Add("Options", Url.Action("Main", "Options"));
                }
                pages.Add("Sign Out", Url.Action("SignOut", "Account"));
            }

            header.Pages = pages;
            return View(header);
        }
    }
}

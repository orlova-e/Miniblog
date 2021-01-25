using Domain.Entities;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Repo.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.App.Interfaces;
using Web.Configuration;
using Web.ViewModels;

namespace Web.Components
{
    public class HeaderViewComponent : ViewComponent
    {
        public BlogOptions BlogOptions { get; private set; }
        public IRepository Repository { get; set; }
        public IUserService UserService { get; private set; }
        public IListPreparer ListPreparer { get; private set; }
        public IListCreator ListCreator { get; private set; }
        public HeaderViewComponent(IRepository repository,
            IUserService userService,
            IListPreparer listPreparer,
            IListCreator listCreator,
            IOptionsSnapshot<BlogOptions> optionsSnapshot)
        {
            BlogOptions = optionsSnapshot.Value;
            UserService = userService;
            ListPreparer = listPreparer;
            ListCreator = listCreator;
            Repository = repository;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            WebsiteOptions websiteDisplayOptions = BlogOptions.WebsiteOptions;
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
                user = await UserService.GetFromDbAsync(id);
                header.User = user;
                role = await Repository.Roles.GetByIdAsync(user.RoleId);
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

            List<Article> pagesDb = ListCreator
                .FindEntries(page => page.EntryType == EntryType.Page && page.Visibility == true && page.MenuVisibility == true);


            foreach (Article page in pagesDb)
            {
                pages.Add(page.Header, Url.Action("Article", "Articles", new { link = page.Link }));
            }

            if (User.Identity.IsAuthenticated)
            {
                pages.Add("Account settings", Url.Action("Settings", "Account"));
                if (role.Discriminator == "ExtendedRole")
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

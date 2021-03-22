using Domain.Entities;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Repo.Interfaces;
using Services.Interfaces;
using System.Collections.Generic;
using Web.Configuration;
using Web.ViewModels;

namespace Web.Components
{
    public class HeaderViewComponent : ViewComponent
    {
        public BlogOptions BlogOptions { get; private set; }
        public IRepository Repository { get; set; }
        public IUserService UserService { get; private set; }
        public IListCreator ListCreator { get; private set; }
        public HeaderViewComponent(IRepository repository,
            IUserService userService,
            IListCreator listCreator,
            IOptionsSnapshot<BlogOptions> optionsSnapshot)
        {
            BlogOptions = optionsSnapshot.Value;
            UserService = userService;
            ListCreator = listCreator;
            Repository = repository;
        }
        public IViewComponentResult Invoke()
        {
            WebsiteOptions displayOptions = BlogOptions.WebsiteOptions;
            HeaderViewModel header = new HeaderViewModel()
            {
                Title = displayOptions.Name,
                ShowSearch = displayOptions.ShowSearchOption,
            };

            Dictionary<string, string> pages = new Dictionary<string, string>();

            Role role = null;

            if (User.Identity.IsAuthenticated)
            {
                User user = UserService.FindByName(User.Identity.Name);
                header.User = user;
                role = user.Role;
                if (role.WriteArticles)
                {
                    pages.Add("Add", Url.Action("Add", "Articles"));
                }
            }

            if (displayOptions.ShowListOfPopularAndRecent)
            {
                pages.Add("Popular", Url.Action("Lists", "Articles", new { listName = "Popular" }));
            }
            if (displayOptions.ShowAuthors)
            {
                pages.Add("Authors", Url.Action("Lists", "Articles", new { name = "Authors" }));
            }
            if (displayOptions.ShowTopics)
            {
                pages.Add("Topics", Url.Action("Lists", "Articles", new { name = "Topics" }));
            }

            List<Article> pagesDb = ListCreator
                .FindEntries(page => page.EntryType == EntryType.Page && page.Visibility == true && page.MenuVisibility == true);

            foreach (Article page in pagesDb)
            {
                pages.Add(page.Header, Url.Action("Article", "Articles", new { link = page.Link }));
            }

            if (User.Identity.IsAuthenticated)
            {
                pages.Add("Favourites", Url.Action("Lists", "Articles", new { listName = "Favourites"}));
                pages.Add("Account settings", Url.Action("Settings", "Account"));
                if (role.Type is RoleType.Administrator)
                {
                    pages.Add("Options", Url.Action("Main", "Options"));
                }
                else if (role.Type is RoleType.Editor)
                {
                    pages.Add("Verification list", Url.Action("List", "Verification", new { queueList = "users" }));
                }
                pages.Add("Sign Out", Url.Action("SignOut", "Account"));
            }

            header.Pages = pages;
            return View(header);
        }
    }
}

using Domain.Entities;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using Repo.Interfaces;
using Services.Interfaces;
using System.Collections.Generic;
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
        public IListCreator ListCreator { get; private set; }
        public HeaderViewComponent(IRepository repository,
            IUserService userService,
            IListCreator listCreator,
            ICommon common)
        {
            BlogOptions = common.Options;
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
                    pages.Add("Add", Url.Action("add", "articles"));
                }
            }

            if (displayOptions.ShowListOfPopularAndRecent)
            {
                pages.Add("Popular", Url.Action("index", "home", new { listName = "default", page = 1, sortBy = ListSorting.MostLiked }));
            }
            if (displayOptions.ShowAuthors)
            {
                pages.Add("Authors", Url.Action("authors", "users"));
            }
            if (displayOptions.ShowTopics)
            {
                pages.Add("Topics", Url.Action("topics", "home"));
            }

            List<Article> pagesDb = ListCreator
                .FindEntries(page => page.EntryType == EntryType.Page && page.Visibility == true && page.MenuVisibility == true);

            foreach (Article page in pagesDb)
            {
                pages.Add(page.Header, Url.Action("article", "articles", new { link = page.Link }));
            }

            if (User.Identity.IsAuthenticated)
            {
                pages.Add("Favourites", Url.Action("index", "home", new { listName = "favourites" }));
                pages.Add("Account settings", Url.Action("settings", "account"));
                if (role.Type is RoleType.Administrator)
                {
                    pages.Add("Options", Url.Action("main", "options"));
                }
                else if (role.Type is RoleType.Editor)
                {
                    pages.Add("Verification list", Url.Action("list", "verification", new { queueList = "users" }));
                }
                pages.Add("Sign Out", Url.Action("signout", "account"));
            }

            header.Pages = pages;
            return View(header);
        }
    }
}

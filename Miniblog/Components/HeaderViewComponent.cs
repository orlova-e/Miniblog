using Microsoft.AspNetCore.Mvc;
using Miniblog.Models.Entities;
using Miniblog.Models.Services.Interfaces;
using Miniblog.ViewModels;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Miniblog.Components
{
    public class HeaderViewComponent : ViewComponent
    {
        public IRepository _repository { get; set; }
        public HeaderViewComponent(IRepository repository)
        {
            _repository = repository;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            WebsiteOptions websiteDisplayOptions = await _repository.WebsiteOptions.FirstOrDefaultAsync();
            HeaderViewModel header = new HeaderViewModel()
            {
                Title = websiteDisplayOptions.Name,
                ShowSearch = websiteDisplayOptions.ShowSearchOption,

            };
            //await _repository.Pages.GetAllAsync();
            Dictionary<string, string> pages = new Dictionary<string, string>();

            if (websiteDisplayOptions.ShowListOfPopularAndRecent)
            {
                pages.Add("Popular", Url.Action("List", "Articles", new { listName = "Popular" }));
            }
            if (websiteDisplayOptions.ShowAuthors)
            {
                pages.Add("Authors", Url.Action("List", "Articles", new { listName = "Authors" }));
            }
            if (websiteDisplayOptions.ShowTopics)
            {
                pages.Add("Topics", Url.Action("List", "Articles", new { listName = "Topics" }));
            }
            //List<Page> madePagesList = new List<Page>();
            //foreach (Page page in madePagesList)
            //{
            //    if (page.Visibility)
            //    {
            //        pages.Add(page.Header, Url.Action("Show", "Pages", new { pageName = page.Header }));
            //    }
            //}
            ClaimsPrincipal claimsPrincipal = User as ClaimsPrincipal;
            
            if (User.Identity.IsAuthenticated)
            {
                pages.Add("Sign Out", Url.RouteUrl("default", new { controller = "Account", action = "SignOut" }));
                ClaimsPrincipal principal = User as ClaimsPrincipal;
                Guid.TryParse(principal.FindFirstValue("Id"), out Guid id);
                //Guid id = Guid.Parse(principal.FindFirstValue("Id"));
                User user = await _repository.Users.GetByIdAsync(id);
                header.Username = user.Username;
                header.Avatar = user.Avatar;
                //header.Link = Url.Action("User", "Account", new { username = user.Username });
            }
            header.Pages = pages;
            return View(header);
        }
    }
}

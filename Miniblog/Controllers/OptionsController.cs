using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Miniblog.Configuration;
using Miniblog.Models.App.Interfaces;
using Miniblog.Models.Entities;
using Miniblog.Models.Entities.Enums;
using Miniblog.ViewModels.Options;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Miniblog.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme,
               Roles = nameof(RoleType.Administrator))]
    public class OptionsController : Controller                 // +++ SURE that you added AUTHORIZATION requirements
    {
        public BlogOptions BlogOptions { get; private set; }
        public IUserService UserService { get; private set; }
        public OptionsController(IOptionsSnapshot<BlogOptions> optionsSnapshot,
            IUserService userService)
        {
            BlogOptions = optionsSnapshot.Value;
            UserService = userService;
        }

        [HttpGet]
        [Authorize(Roles = nameof(RoleType.Administrator) + "," + nameof(RoleType.Editor) + "," + nameof(RoleType.User))]
        public async Task<IActionResult> Account()            // SURE that you created VIEW
        {
            Guid.TryParse(User.FindFirstValue("Id"), out Guid userId);
            User user = await UserService.GetFromDbAsync(userId);
            return View(user);
        }

        //[Authorize(Roles = nameof(RoleType.Administrator))]
        [HttpGet]
        public IActionResult Main()
        {
            Configuration.WebsiteOptions websiteOptions = BlogOptions.WebsiteOptions;
            MainViewModel mainViewModel = new MainViewModel()
            {
                Title = websiteOptions.Name,
                Subtitle = websiteOptions.Subtitle,
                IconPath = websiteOptions.IconPath,
                DateFormat = websiteOptions.WebsiteDateFormat,
                Language = Enum.GetName(typeof(Languages), websiteOptions.WebsiteLanguage)
                //TimeFormat = options.Ti
            };
            return View(mainViewModel);
        }

        //[Route("Main")]
        //[HttpPost]
        ////[Authorize(Roles = nameof(RoleType.Administrator))]
        //public async Task<IActionResult> Main(MainViewModel mainViewModel)
        //{

        //}

        //[HttpGet]
        //public async Task<IActionResult> Writing()
        //{
        //    Role role 
        //}
    }
}

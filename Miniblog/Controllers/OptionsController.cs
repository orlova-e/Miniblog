using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Miniblog.Models.App.Interfaces;
using Miniblog.Models.Entities;
using Miniblog.Models.Entities.Enums;
using Miniblog.Models.Services.Interfaces;
using Miniblog.ViewModels.Options;

namespace Miniblog.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme,
               Roles = nameof(RoleType.Administrator))]
    public class OptionsController : Controller                 // +++ SURE that you added AUTHORIZATION requirements
    {
        public IUserService userService { get; private set; }
        public IRepository repository { get; set; }
        public OptionsController(IRepository repository, IUserService userService)
        {
            this.userService = userService;
            this.repository = repository;
        }

        [HttpGet]
        [Authorize(Roles = nameof(RoleType.Administrator) + "," + nameof(RoleType.Editor) + "," + nameof(RoleType.User))]
        public async Task<IActionResult> Account()            // SURE that you created VIEW
        {
            User user = await repository.Users.GetByIdAsync(Guid.Parse(User.FindFirstValue("Id")));
            return View(user);
        }

        [HttpGet]
        //[Authorize(Roles = nameof(RoleType.Administrator))]
        public async Task<IActionResult> Main()
        {
            WebsiteOptions options = await repository.WebsiteOptions.FirstOrDefaultAsync();
            MainViewModel mainViewModel = new MainViewModel()
            {
                Title = options.Name,
                Subtitle = options.Subtitle,
                Icon = options.Icon,
                DateFormat = options.WebsiteDateFormat,
                Language = Enum.GetName(typeof(Languages), options.WebsiteLanguage)
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

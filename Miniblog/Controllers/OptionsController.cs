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

namespace Miniblog.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme,
        Roles = nameof(RoleType.Administrator) + nameof(RoleType.Editor))]
    public class OptionsController : Controller                 // +++ SURE that you added AUTHORIZATION requirements
    {
        public IUserService _userService { get; private set; }
        public IRepository _repository { get; set; }
        public OptionsController(IRepository repository, IUserService userService)
        {
            _userService = userService;
            _repository = repository;
        }
        [Authorize(Roles = nameof(RoleType.User))]
        [Route("UserOptions")]
        [HttpGet]
        public async Task<IActionResult> UserOptionsAsync()
        {
            User user = await _repository.Users.GetByIdAsync(Guid.Parse(User.FindFirstValue("Id")));
            return View(user);                                          // SURE that you created VIEW
        }
        [Authorize(Roles = nameof(RoleType.Administrator))]
        [Route("Main")]
        [HttpGet]
        public async Task<IActionResult> MainAsync()
        {
            WebsiteOptions websiteDisplayOptions = await _repository.WebsiteOptions.FirstOrDefaultAsync();
            return View(websiteDisplayOptions);
        }

    }
}

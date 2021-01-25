using Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.ViewModels;

namespace Web.Controllers
{
    public class AccountController : Controller
    {
        public IUserService UserService { get; private set; }
        public AccountController(IUserService userService)
        {
            UserService = userService;
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignInAsync([FromForm] LoginViewModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return View(loginModel);
            }

            User user = UserService.GetFromDb((Account)loginModel);

            if (user == null)
            {
                ModelState.AddModelError("", "User is not found");
                return View("SignIn", loginModel);
            }

            await Authenticate(user);

            return RedirectToRoute("default", new { controller = "Home", action = "Index" });
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUpAsync([FromForm] RegisterViewModel registerModel)
        {
            if (!ModelState.IsValid)
            {
                return View(registerModel);
            }

            Dictionary<string, string> errors = UserService.CheckParameters((Account)registerModel);
            foreach (var error in errors)
            {
                ModelState.AddModelError(error.Key, error.Value);
            }

            if (!ModelState.IsValid)
            {
                return View("SignUp", registerModel);
            }

            User user = await UserService.CreateIntoDbAsync((Account)registerModel);

            await Authenticate(user);
            return RedirectToRoute("default", new { controller = "Home", action = "Index" });
        }

        private async Task Authenticate(User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim("Id", user.Id.ToString()),
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Username),
                new Claim("Email", user.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.Type.ToString())
            };

            ClaimsIdentity identity = new ClaimsIdentity(claims, "ApplicationCookie",
                ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
        }

        [Route("SignOut")]
        public async Task<IActionResult> SignOutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Miniblog.Models.App.Interfaces;
using Miniblog.Models.Entities;
using Miniblog.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Miniblog.Controllers
{
    public class AccountController : Controller
    {
        public IUserService _userService { get; private set; }
        public AccountController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async  Task<IActionResult> SignInAsync([FromForm]LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return View(loginModel);
            }

            User user = _userService.GetFromDb(loginModel);

            if(user == null)
            {
                ModelState.AddModelError("", "User is not found");
                return View("SignIn", loginModel);
            }

            await Authenticate(user);

            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUpAsync([FromForm]RegisterModel registerModel/*, string userId = null*/)
        {
            if(!ModelState.IsValid)
            {
                return View(registerModel);
            }

            Guid guid;
            bool alreadyRegisteredInJwtAccount = false;
            User user;
            bool guidParsed = Guid.TryParse(registerModel.userId, out guid);

            if (guidParsed)
            {
                alreadyRegisteredInJwtAccount = _userService.CheckForExistence(registerModel, guid);
            }

            if (alreadyRegisteredInJwtAccount)
            {
                user = await _userService.GetFromDbAsync(guid);
            }
            else // если JwtAccountController.SignUp не отработал
            {
                IEnumerable<string> errors = _userService.ParametersAlreadyExist(registerModel);
                if (errors.Any())
                {
                    if (errors.Contains("Username"))
                    {
                        ModelState.AddModelError(nameof(registerModel.Username), "A user with this name already exists");
                    }
                    if (errors.Contains("Email"))
                    {
                        ModelState.AddModelError(nameof(registerModel.Email), "A user with this email already exists");
                    }
                    return View("SignUp", registerModel);
                }
                user = await _userService.CreateIntoDbAsync(registerModel);
            }

            await Authenticate(user);

            return RedirectToAction("Index", "Home");
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

using Domain.Entities;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Repo.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.App.Interfaces;
using Web.Configuration;
using Web.Infrastructure.Extensions;
using Web.ViewModels.Options;

namespace Web.Controllers
{
    [Authorize(Roles = nameof(RoleType.Administrator))]
    public class OptionsController : Controller
    {
        public BlogOptions BlogOptions { get; private set; }
        public IConfigurationWriter ConfigurationWriter { get; private set; }
        public IWebHostEnvironment WebHostEnvironment { get; private set; }
        public IRepository Repository { get; private set; }
        public OptionsController(IOptionsSnapshot<BlogOptions> optionsSnapshot,
            IConfigurationWriter configurationWriter,
            IWebHostEnvironment webHostEnvironment,
            IRepository repository)
        {
            BlogOptions = optionsSnapshot.Value;
            ConfigurationWriter = configurationWriter;
            WebHostEnvironment = webHostEnvironment;
            Repository = repository;
        }

        [HttpGet]
        public IActionResult Main()
        {
            MainViewModel mainViewModel = (MainViewModel)BlogOptions.WebsiteOptions;
            return View(mainViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Main(MainViewModel mainViewModel)
        {
            mainViewModel.IconPath = BlogOptions.WebsiteOptions.IconPath;
            mainViewModel.AvatarPath = BlogOptions.WebsiteOptions.StandardAvatarPath;

            if (!ModelState.IsValid)
                return View(mainViewModel);

            string iconPath = await TryChangeImageAsync(mainViewModel.IconFile, mainViewModel.IconPath);
            mainViewModel.IconPath = !string.IsNullOrWhiteSpace(iconPath) ? iconPath : mainViewModel.IconPath;

            string avatarPath = await TryChangeImageAsync(mainViewModel.AvatarFile, mainViewModel.AvatarPath);
            mainViewModel.AvatarPath = !string.IsNullOrWhiteSpace(avatarPath) ? avatarPath : mainViewModel.AvatarPath;

            BlogOptions.WebsiteOptions += mainViewModel;
            await ConfigurationWriter.WriteAsync(BlogOptions);

            return RedirectToAction("main");
        }

        private async Task<string> TryChangeImageAsync(IFormFile formFile, string path)
        {
            if (formFile?.Length > 0)
            {
                string filePath = WebHostEnvironment.WebRootPath + path;
                filePath = await formFile.TryUpdateFileAsync(filePath);
                if (System.IO.File.Exists(filePath))
                {
                    filePath = filePath.Replace(WebHostEnvironment.WebRootPath, "");
                    string newPath = filePath;
                    return newPath;
                }
            }

            return string.Empty;
        }

        [HttpGet]
        public async Task<IActionResult> Writing()
        {
            List<RoleViewModel> rolesViewModels = new List<RoleViewModel>();
            List<Role> roles = await Repository.Roles.GetAllAsync();

            foreach (var role in roles)
            {
                if (role is ExtendedRole extendedRole)
                    rolesViewModels.Add((RoleViewModel)extendedRole);
                else
                    rolesViewModels.Add((RoleViewModel)role);
            }

            return View(rolesViewModels);
        }

        [HttpPost]
        public async Task<IActionResult> Writing(List<RoleViewModel> rolesViewModels)
        {
            RoleViewModel userViewModel = rolesViewModels.Where(r => r.Discriminator.Equals("Role")).First();
            if (userViewModel.ModerateTopics || userViewModel.ModerateTags)
            {
                userViewModel.ModerateTopics = false;
                userViewModel.ModerateTags = false;
                return View(rolesViewModels);
            }

            List<Role> roles = await Repository.Roles.GetAllAsync();

            foreach (var roleViewModel in rolesViewModels)
            {
                var role = roles.Where(r => Enum.GetName(typeof(RoleType), r.Type).Equals(roleViewModel.Type)).First();
                if (role is ExtendedRole extendedRole)
                {
                    extendedRole = extendedRole + roleViewModel;
                }
                else
                {
                    role += roleViewModel;
                }
                await Repository.Roles.UpdateAsync(role);
            }

            return RedirectToAction("writing");
        }
    }
}

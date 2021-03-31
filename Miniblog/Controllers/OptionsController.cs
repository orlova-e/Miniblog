using Domain.Entities;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repo.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.App.Interfaces;
using Web.Configuration;
using Web.Filters;
using Web.Infrastructure.Extensions;
using Web.ViewModels.Options;

namespace Web.Controllers
{
    [TypeFilter(typeof(AccessByRolesAttribute), Arguments = new object[] { new RoleType[] { RoleType.Administrator } })]
    public class OptionsController : Controller
    {
        private IChangeCommon Common { get; }
        private IWebHostEnvironment WebHostEnvironment { get; }
        private IRepository Repository { get; }
        public OptionsController(IChangeCommon common,
            IWebHostEnvironment webHostEnvironment,
            IRepository repository)
        {
            Common = common;
            WebHostEnvironment = webHostEnvironment;
            Repository = repository;
        }

        [HttpGet]
        public IActionResult Main()
        {
            MainViewModel mainViewModel = (MainViewModel)Common.Options.WebsiteOptions;
            return View(mainViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Main([FromForm] MainViewModel mainViewModel)
        {
            BlogOptions options = Common.Options;
            mainViewModel.IconPath = options.WebsiteOptions.IconPath;
            mainViewModel.AvatarPath = options.WebsiteOptions.StandardAvatarPath;

            if (!ModelState.IsValid)
                return View(mainViewModel);

            string iconPath = await TryChangeImageAsync(mainViewModel.IconFile, mainViewModel.IconPath);
            mainViewModel.IconPath = !string.IsNullOrWhiteSpace(iconPath) ? iconPath : mainViewModel.IconPath;

            string avatarPath = await TryChangeImageAsync(mainViewModel.AvatarFile, mainViewModel.AvatarPath);
            mainViewModel.AvatarPath = !string.IsNullOrWhiteSpace(avatarPath) ? avatarPath : mainViewModel.AvatarPath;

            options.WebsiteOptions += mainViewModel;
            await Common.UpdateOptionsAsync(options);

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
                    return filePath.Replace(WebHostEnvironment.WebRootPath, "");
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
        public async Task<IActionResult> Writing([FromForm] List<RoleViewModel> rolesViewModels)
        {
            if (!ModelState.IsValid)
                return View(rolesViewModels);

            List<Role> roles = await Repository.Roles.GetAllAsync();

            foreach (var roleViewModel in rolesViewModels)
            {
                var role = roles.Where(r => r.Type == roleViewModel.Type).First();
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

        [HttpGet]
        public async Task<IActionResult> Discussion()
        {
            DiscussionViewModel discussionViewModel = new()
            {
                CommentsOptions = Common.Options.CommentsOptions,
                DiscussionRoles = new List<DiscussionRoles>()
            };

            List<Role> roles = await Repository.Roles.GetAllAsync();
            foreach (var role in roles)
            {
                if (role is ExtendedRole extendedRole)
                    discussionViewModel.DiscussionRoles.Add((DiscussionRoles)extendedRole);
                else
                    discussionViewModel.DiscussionRoles.Add((DiscussionRoles)role);
            }

            return View(discussionViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Discussion([FromForm] DiscussionViewModel discussionViewModel)
        {
            if (!ModelState.IsValid)
                return View(discussionViewModel);

            BlogOptions options = Common.Options;
            discussionViewModel.CommentsOptions.Depth.Available ??= options.CommentsOptions.Depth.Available;
            if (!options.CommentsOptions.Equals(discussionViewModel.CommentsOptions))
            {
                options.CommentsOptions = discussionViewModel.CommentsOptions;
                await Common.UpdateOptionsAsync(options);
            }

            List<Role> roles = await Repository.Roles.GetAllAsync();

            foreach (var discussionRole in discussionViewModel.DiscussionRoles)
            {
                var role = roles.Where(r => r.Type == discussionRole.Type).First();
                if (role is ExtendedRole extendedRole)
                {
                    extendedRole = extendedRole + discussionRole;
                }
                else
                {
                    role += discussionRole;
                }
                await Repository.Roles.UpdateAsync(role);
            }

            return RedirectToAction("discussion");
        }

        [HttpGet]
        public IActionResult Reading()
        {
            ReadingViewModel readingViewModel = new ReadingViewModel
            {
                ListOptions = Common.Options.ListOptions,
                WebsiteOptionsPartially = (WebsiteOptionsPartially)Common.Options.WebsiteOptions
            };
            return View(readingViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Reading([FromForm] ReadingViewModel readingViewModel)
        {
            if (!ModelState.IsValid)
                return View(readingViewModel);

            BlogOptions options = Common.Options;
            readingViewModel.ListOptions.ArticlesPerPage.Available ??= options.ListOptions.ArticlesPerPage.Available;
            readingViewModel.ListOptions.WordsPerPreview.Available ??= options.ListOptions.WordsPerPreview.Available;

            options.ListOptions = readingViewModel.ListOptions;
            options.WebsiteOptions += readingViewModel.WebsiteOptionsPartially;
            await Common.UpdateOptionsAsync(options);

            return RedirectToAction("reading");
        }

        [HttpGet]
        public async Task<IActionResult> CheckLists()
        {
            List<CheckList> checkLists = await Repository.CheckLists.GetAllAsync();
            CheckListViewModel checkListView = new CheckListViewModel();
            checkListView.ListToCheck = checkLists
                .Where(c => c.CheckAction == CheckAction.Verify)
                .First()
                .VerifiableWords;
            checkListView.BlackList = checkLists
                .Where(c => c.CheckAction == CheckAction.Delete)
                .First()
                .VerifiableWords;

            return View(checkListView);
        }

        [HttpPost]
        public async Task<IActionResult> CheckLists([FromForm] CheckListViewModel checkListView)
        {
            List<CheckList> checkLists = await Repository.CheckLists.GetAllAsync();
            CheckList toCheck = checkLists
                .Where(c => c.CheckAction == CheckAction.Verify)
                .First();
            toCheck.VerifiableWords = checkListView.ListToCheck;
            await Repository.CheckLists.UpdateAsync(toCheck);
            CheckList toDelete = checkLists
                .Where(c => c.CheckAction == CheckAction.Delete)
                .First();
            toDelete.VerifiableWords = checkListView.BlackList;
            await Repository.CheckLists.UpdateAsync(toDelete);

            return RedirectToAction("checklists");
        }
    }
}

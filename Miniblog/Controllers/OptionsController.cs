using Domain.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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
        public OptionsController(IOptionsSnapshot<BlogOptions> optionsSnapshot,
            IConfigurationWriter configurationWriter,
            IWebHostEnvironment webHostEnvironment)
        {
            BlogOptions = optionsSnapshot.Value;
            ConfigurationWriter = configurationWriter;
            WebHostEnvironment = webHostEnvironment;
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
    }
}

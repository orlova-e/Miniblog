using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Miniblog.Configuration;
using Miniblog.Filters;
using Miniblog.Hubs;
using Miniblog.Models.App;
using Miniblog.Models.App.Interfaces;
using Miniblog.Models.Entities;
using Miniblog.Models.Services;
using Miniblog.Models.Services.Interfaces;
using System;
using System.Globalization;
using System.IO;

namespace Miniblog
{
    public class Startup
    {
        public IConfiguration Configuration { get; private set; }
        private string ConfigPath { get; }
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            //IConfigurationBuilder builder = new ConfigurationBuilder()
            //    .AddConfiguration(configuration);
            //Configuration = builder.Build();
            Configuration = configuration;
            ConfigPath = Path.Combine(env.ContentRootPath, "config.json");
        }
        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<MiniblogDb>(options => options.UseSqlServer(connectionString));

            services.Configure<BlogOptions>(Configuration);

            services.AddScoped<IRepository, Repository>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IArticlesService, ArticleService>();
            services.AddScoped<IListService, ListService>();
            services.AddScoped<IConfigurationWriter>(x => ActivatorUtilities.CreateInstance<ConfigurationWriter>(x, ConfigPath));

            services.AddTransient<ITextService, TextService>(); // to delete

            services.AddScoped<IOptionRepository<Role>, RolesRepository>();
            services.AddScoped<IOptionRepository<Models.Entities.WebsiteOptions>, WebsiteOptionsRepo>();
            services.AddScoped<IOptionRepository<ListDisplayOptions>, ListOptionsRepo>();

            services.AddScoped<IdAttribute>();

            services.AddSingleton<IUserIdProvider, UserNameProvider>();

            services.AddSignalR()
                .AddHubOptions<SubscriptionHub>(options =>
                {
                    options.ClientTimeoutInterval = TimeSpan.FromMinutes(15);
                    options.KeepAliveInterval = TimeSpan.FromMinutes(15);
                })
                .AddHubOptions<ArticleHub>(options =>
                {
                    options.ClientTimeoutInterval = TimeSpan.FromMinutes(30);
                    options.KeepAliveInterval = TimeSpan.FromMinutes(30);
                });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString("/Account/SignIn");
                    options.AccessDeniedPath = new PathString("/Account/SignIn");
                    //options.AccessDeniedPath = "/Account/Forbidden/";
                });

            services.AddAuthorization();

            services.AddLocalization(/*options => options.ResourcesPath = "Resources"*/);

            services.AddMvc()
                .AddMvcOptions(options =>
                {
                    options.Filters.AddService<IdAttribute>();
                })
                .AddViewLocalization();
            //services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IAntiforgery antiforgery)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseExceptionHandler("/errors");
                app.UseHsts();
            }

            CultureInfo[] supportedCultures = new[]
            {
                new CultureInfo("en"),
                new CultureInfo("ru")
            };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ArticleHub>("/articlehub");
                endpoints.MapHub<SubscriptionHub>("/subscription");

                //endpoints.MapHub<MessagesHub>("/messages");

                //endpoints.MapControllerRoute(
                //    name: "api",
                //    pattern: "api/{controller}/{action}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services;
using Services.Implementation;
using Services.Interfaces;
using System;
using System.Globalization;
using System.IO;
using Web.App.Implementation;
using Web.App.Interfaces;
using Web.Configuration;
using Web.Filters;
using Web.Hubs;

namespace Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; private set; }
        private string ConfigurationFilePath { get; }
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            ConfigurationFilePath = Path.Combine(env.ContentRootPath, "config.json");
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<BlogOptions>(Configuration);
            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddBLLServices(connectionString);
            services.AddScoped<IConfigurationWriter>(x => ActivatorUtilities.CreateInstance<ConfigurationWriter>(x, ConfigurationFilePath));

            services.AddScoped<IListPreparer, ListPreparer>();
            services.AddScoped<IListCreator, ListCreator>();

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
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Miniblog.Filters;
using Miniblog.Hubs;
using Miniblog.Models;
using Miniblog.Models.App;
using Miniblog.Models.App.Interfaces;
using Miniblog.Models.Entities;
using Miniblog.Models.Services;
using Miniblog.Models.Services.Interfaces;

namespace Miniblog
{
    public class Startup
    {
        public IConfiguration configuration { get; }
        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<MiniblogDb>(options => options.UseSqlServer(connectionString));

            services.AddScoped<IRepository, Repository>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IArticlesService, ArticleService>();

            services.AddTransient<ITextService, TextService>(); // to delete

            services.AddScoped<IOptionRepository<Role>, RolesRepository>();
            services.AddScoped<IOptionRepository<WebsiteOptions>, WebsiteOptionsRepo>();
            services.AddScoped<IOptionRepository<ListDisplayOptions>, ListOptionsRepo>();

            services.AddScoped<IdAttribute>();

            services.AddSignalR()
                .AddHubOptions<SubscriptionHub>(options =>
                {
                    options.ClientTimeoutInterval = TimeSpan.FromMinutes(15);
                    options.KeepAliveInterval = TimeSpan.FromMinutes(15);
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
                endpoints.MapHub<CommentsHub>("/comments");
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

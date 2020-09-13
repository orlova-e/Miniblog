using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        public IConfiguration _configuration { get; }
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<MiniblogDb>(options => options.UseSqlServer(connectionString));

            services.AddSignalR();

            services.AddScoped<IRepository, Repository>();
            services.AddScoped<IUserService, UserService>();
            services.AddTransient<ITextService, TextService>();
            services.AddScoped<IOptionRepository<Role>, RolesRepository>();
            services.AddScoped<IOptionRepository<WebsiteOptions>, WebsiteOptionsRepo>();
            //services.AddScoped<AccessAttribute>();
            //services.AddTransient(typeof(IOptionRepository<Role>), typeof(RolesRepository));

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString("/Account/SignIn");
                    //options.AccessDeniedPath = "/Account/Forbidden/";
                });

            services.AddAuthorization();

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IAntiforgery antiforgery)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<CommentsHub>("/comments");

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

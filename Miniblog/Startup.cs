using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Miniblog.Models;
using Miniblog.Models.App;
using Miniblog.Models.App.Interfaces;
using Miniblog.Models.Services;
using Miniblog.Models.Services.Interfaces;
using Miniblog.Token;

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
            services.AddAntiforgery(options => 
                options.HeaderName = "X-CSRF-TOKEN");

            const string signingSecurityKey = "K8eT3uYAsus7pKxU4e7uBHScpV4Vj1TBf3lN7YO6nAbWEyRj4XSUqw";
            var signingKey = new SigningSymmetricKey(signingSecurityKey);
            services.AddSingleton<IJwtSigningEncodingKey>(signingKey);

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<MiniblogDb>(options => options.UseSqlServer(connectionString));

            services.AddScoped<IRepository, Repository>();
            services.AddScoped<IUserService, UserService>();

            var signingDecodingKey = (IJwtSigningDecodingKey)signingKey;

            services.AddAuthentication()
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
                options =>
                {
                    options.LoginPath = new PathString("/Account/SignIn");
                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
                options =>
                {
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = ValidateTokenOptions.ISSUER,

                        ValidateAudience = true,
                        ValidAudience = ValidateTokenOptions.AUDIENCE,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = signingDecodingKey.GetSecurityKey()
                    };
                });

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

            //app.Use(next => context =>
            //{
            //    string path = context.Request.Path.Value;

            //    if (
            //        string.Equals(path, "/", StringComparison.OrdinalIgnoreCase) ||
            //        string.Equals(path, "/index.html", StringComparison.OrdinalIgnoreCase))
            //    {
            //        // The request token can be sent as a JavaScript-readable cookie, 
            //        // and Angular uses it by default.
            //        var tokens = antiforgery.GetAndStoreTokens(context);
            //        context.Response.Cookies.Append("CSRF-TOKEN", tokens.RequestToken,
            //            new CookieOptions() { HttpOnly = false });
            //    }

            //    return next(context);
            //});

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "api",
                    pattern: "api/{controller}/{action}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

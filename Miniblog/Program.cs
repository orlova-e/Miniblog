using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Repo;
using System;
using System.Diagnostics;
using Web.App.Implementation;

namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using(var scopeService = host.Services.CreateScope())
            {
                IServiceProvider provider = scopeService.ServiceProvider;
                try
                {
                    var dbContext = provider.GetRequiredService<MiniblogDb>();
                    var configuration = provider.GetRequiredService<IConfiguration>();
                    new UsersInitializer(configuration, dbContext).InitializeAndCheck();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.StackTrace);
                }
                finally
                {
                    host.Run();
                }
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(configure =>
                {
                    configure.AddJsonFile("config.json", optional: false, reloadOnChange: true)
                        .AddJsonFile("users.json", optional: true, reloadOnChange: false);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

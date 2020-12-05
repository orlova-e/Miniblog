using Microsoft.Extensions.DependencyInjection;
using Miniblog.Configuration;
using Services.Implementation;
using Services.Interfaces;
using Domain.Entities;
using Repo.Implementation;
using Repo.Interfaces;
using Repo;

namespace Services
{
    public static class ServicesServiceCollectionExtension
    {
        public static IServiceCollection AddBLLServices(this IServiceCollection services, string dbConnectionString, string configurationPath)
        {
            services.AddRepository(dbConnectionString);
            services.AddArticleService();
            services.AddUserService();
            services.AddListService();
            services.AddTextService();
            services.AddConfigurationWriter(configurationPath);
            services.AddRolesRepo();
            return services;
        }

        private static IServiceCollection AddArticleService(this IServiceCollection services)
        {
            services.AddScoped<IArticlesService, ArticleService>();
            return services;
        }

        private static IServiceCollection AddUserService(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            return services;
        }

        private static IServiceCollection AddTextService(this IServiceCollection services)
        {
            services.AddTransient<ITextService, TextService>();
            return services;
        }

        private static IServiceCollection AddListService(this IServiceCollection services)
        {
            services.AddScoped<IListService, ListService>();
            return services;
        }
        private static IServiceCollection AddConfigurationWriter(this IServiceCollection services, string configurationPath)
        {
            services.AddScoped<IConfigurationWriter>(x => ActivatorUtilities.CreateInstance<ConfigurationWriter>(x, configurationPath));
            return services;
        }

        private static IServiceCollection AddRolesRepo(this IServiceCollection services)
        {
            services.AddScoped<IOptionRepository<Role>, RolesRepository>();
            return services;
        }
    }
}

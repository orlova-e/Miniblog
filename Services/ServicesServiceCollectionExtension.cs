using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Repo;
using Repo.Implementation;
using Repo.Interfaces;
using Services.Implementation;
using Services.Implementation.Indexing;
using Services.Interfaces;
using Services.Interfaces.Indexing;

namespace Services
{
    public static class ServicesServiceCollectionExtension
    {
        public static IServiceCollection AddServicesLayer(this IServiceCollection services, string dbConnectionString)
        {
            services.AddRepository(dbConnectionString);
            services.AddArticleService();
            services.AddUserService();
            services.AddIndexedObjectsObserver();
            services.AddListCreator();
            services.AddTextService();
            services.AddRolesRepo();
            return services;
        }

        private static IServiceCollection AddArticleService(this IServiceCollection services)
        {
            services.AddScoped<IArticleService, ArticleService>();
            return services;
        }

        private static IServiceCollection AddUserService(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            return services;
        }

        private static IServiceCollection AddIndexedObjectsObserver(this IServiceCollection services)
        {
            services.AddScoped<IIndexedObjectsObserver, IndexedObjectsObserver>();
            return services;
        }

        private static IServiceCollection AddTextService(this IServiceCollection services)
        {
            services.AddTransient<ITextService, TextService>();
            return services;
        }

        private static IServiceCollection AddListCreator(this IServiceCollection services)
        {
            services.AddScoped<IListCreator, ListCreator>();
            return services;
        }

        private static IServiceCollection AddRolesRepo(this IServiceCollection services)
        {
            services.AddScoped<IOptionRepository<Role>, RolesRepository>();
            return services;
        }
    }
}

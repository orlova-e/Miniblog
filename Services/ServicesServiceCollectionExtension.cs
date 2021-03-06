using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Repo;
using Repo.Implementation;
using Repo.Interfaces;
using Services.Implementation;
using Services.Implementation.Search;
using Services.Interfaces;
using Services.Interfaces.Search;

namespace Services
{
    public static class ServicesServiceCollectionExtension
    {
        public static IServiceCollection AddServicesLayer(this IServiceCollection services, string dbConnectionString)
        {
            services.AddRepository(dbConnectionString);
            services.AddSearch();
            services.AddArticleService();
            services.AddUserService();
            services.AddCommentsService();
            services.AddEntitesObserver();
            services.AddListCreator();
            services.AddCheckPreparerBuilder();
            services.AddTextService();
            services.AddRolesRepo();
            return services;
        }

        private static IServiceCollection AddSearch(this IServiceCollection services)
        {
            services.AddScoped(typeof(IAggregateSearch<>), typeof(AggregateSearch<>));
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

        private static IServiceCollection AddCommentsService(this IServiceCollection services)
        {
            services.AddScoped<ICommentsService, CommentsService>();
            return services;
        }

        private static IServiceCollection AddEntitesObserver(this IServiceCollection services)
        {
            services.AddScoped<IEntityObserver, EntityObserver>();
            return services;
        }

        private static IServiceCollection AddCheckPreparerBuilder(this IServiceCollection services)
        {
            services.AddScoped<CheckPreparerBuilder>();
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

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Repo.Implementation;
using Repo.Interfaces;

namespace Repo
{
    public static class RepoServiceCollectionExtensions
    {
        public static IServiceCollection AddRepository(this IServiceCollection services, string connectionString)
        {
            services.AddDatabase(connectionString);
            services.AddScoped<IRepository, Repository>();
            return services;
        }

        private static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<MiniblogDb>(options => options.UseSqlServer(connectionString,
                options => options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));
            return services;
        }
    }
}

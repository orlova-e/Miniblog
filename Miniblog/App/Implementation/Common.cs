using Domain.Entities;
using Domain.Entities.Enums;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Repo.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Web.App.Interfaces;
using Web.Configuration;

namespace Web.App.Implementation
{
    public class Common : IChangeCommon
    {
        private readonly TimeSpan absoluteExpiration = TimeSpan.FromMinutes(30);
        private readonly SemaphoreSlim cacheLock = new(1);
        private IConfigurationWriter ConfigurationWriter { get; }
        private IMemoryCache MemoryCache { get; }
        private IRepository Repository { get; }

        public Common(IRepository repository,
            IOptionsSnapshot<BlogOptions> optionsSnapshot,
            IConfigurationWriter configurationWriter,
            IMemoryCache memoryCache)
        {
            ConfigurationWriter = configurationWriter;
            MemoryCache = memoryCache;
            Repository = repository;

            Initialize(optionsSnapshot.Value);
        }

        public List<Role> Roles => MemoryCache.Get(nameof(this.Roles)) as List<Role>;
        public List<ExtendedRole> ExtendedRoles => Roles.Where(r => r is ExtendedRole) as List<ExtendedRole>;
        public BlogOptions Options => MemoryCache.Get(nameof(this.Options)) as BlogOptions;

        private void Initialize(BlogOptions blogOptions)
        {
            if (!MemoryCache.TryGetValue(nameof(this.Options), out BlogOptions options))
            {
                MemoryCache.Set(nameof(this.Options), blogOptions, absoluteExpiration);
            }

            if (!MemoryCache.TryGetValue(nameof(this.Roles), out List<Role> roles))
            {
                roles = Repository.Roles.Find(r => true).ToList();
                MemoryCache.Set(nameof(this.Roles), roles, absoluteExpiration);
            }
        }

        public async Task UpdateRoleAsync(Role role)
        {
            await cacheLock.WaitAsync();
            int index = Roles.FindIndex(r => r.Type == role.Type);
            Roles.RemoveAt(index);
            Roles.Insert(index, role);
            MemoryCache.Remove(nameof(this.Roles));
            MemoryCache.Set(nameof(this.Roles), Roles, absoluteExpiration);
            cacheLock.Release();

            await Repository.Roles.UpdateAsync(role);
        }

        public async Task UpdateOptionsAsync(BlogOptions blogOptions)
        {
            await cacheLock.WaitAsync();
            MemoryCache.Remove(nameof(this.Options));
            MemoryCache.Set(nameof(this.Options), blogOptions, absoluteExpiration);
            cacheLock.Release();

            await ConfigurationWriter.WriteAsync(blogOptions);
        }

        public Role GetRole(ClaimsPrincipal user)
        {
            RoleType type = (RoleType)Enum.Parse(typeof(RoleType), user.FindFirst(ClaimsIdentity.DefaultRoleClaimType).Value);
            return Roles.Where(r => r.Type == type).FirstOrDefault();
        }
    }
}

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
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

        public Common(IOptionsSnapshot<BlogOptions> optionsSnapshot,
            IConfigurationWriter configurationWriter,
            IMemoryCache memoryCache)
        {
            ConfigurationWriter = configurationWriter;
            MemoryCache = memoryCache;

            Initialize(optionsSnapshot.Value);
        }

        public BlogOptions Options => MemoryCache.Get(nameof(this.Options)) as BlogOptions;

        private void Initialize(BlogOptions blogOptions)
        {
            if (!MemoryCache.TryGetValue(nameof(this.Options), out BlogOptions options))
            {
                MemoryCache.Set(nameof(this.Options), blogOptions, absoluteExpiration);
            }
        }

        public async Task UpdateOptionsAsync(BlogOptions blogOptions)
        {
            await cacheLock.WaitAsync();
            MemoryCache.Remove(nameof(this.Options));
            MemoryCache.Set(nameof(this.Options), blogOptions, absoluteExpiration);
            cacheLock.Release();

            await ConfigurationWriter.WriteAsync(blogOptions);
        }
    }
}

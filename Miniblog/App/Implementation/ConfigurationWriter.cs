using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Web.App.Interfaces;
using Web.Configuration;

namespace Web.App.Implementation
{
    public class ConfigurationWriter : IConfigurationWriter
    {
        private string ConfigPath { get; }
        public ConfigurationWriter(string configPath)
        {
            ConfigPath = configPath;
        }
        public async Task WriteAsync(BlogOptions blogOptions)
        {
            using (FileStream fileStream = new FileStream(ConfigPath, FileMode.Create, FileAccess.Write))
            {
                JsonSerializerOptions serializerOptions = new JsonSerializerOptions
                {
                    AllowTrailingCommas = false,
                    IgnoreNullValues = false,
                    WriteIndented = true
                };
                await JsonSerializer.SerializeAsync<BlogOptions>(fileStream, blogOptions, serializerOptions);
            }
        }
    }
}

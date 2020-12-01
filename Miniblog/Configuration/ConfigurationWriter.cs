using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Miniblog.Configuration
{
    public class ConfigurationWriter : IConfigurationWriter
    {
        private string configPath { get; }
        public ConfigurationWriter(string configPath)
        {
            this.configPath = configPath;
        }
        public async Task WriteAsync(BlogOptions blogOptions)
        {
            using (FileStream fileStream = new FileStream(configPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
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

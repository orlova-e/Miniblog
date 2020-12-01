using System.Threading.Tasks;

namespace Miniblog.Configuration
{
    public interface IConfigurationWriter
    {
        Task WriteAsync(BlogOptions blogOptions);
    }
}

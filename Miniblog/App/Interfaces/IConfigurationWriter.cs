using System.Threading.Tasks;
using Web.Configuration;

namespace Web.App.Interfaces
{
    public interface IConfigurationWriter
    {
        Task WriteAsync(BlogOptions blogOptions);
    }
}

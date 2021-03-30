using System.Threading.Tasks;
using Web.Configuration;

namespace Web.App.Interfaces
{
    public interface IChangeCommon : ICommon
    {
        Task UpdateOptionsAsync(BlogOptions blogOptions);
    }
}

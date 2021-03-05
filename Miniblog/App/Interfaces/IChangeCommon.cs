using Domain.Entities;
using System.Threading.Tasks;
using Web.Configuration;

namespace Web.App.Interfaces
{
    public interface IChangeCommon : ICommon
    {
        Task UpdateRoleAsync(Role role);
        Task UpdateOptionsAsync(BlogOptions blogOptions);
    }
}

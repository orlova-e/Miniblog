using Miniblog.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miniblog.Models.Services.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> GetByIdAsync(Guid id);
        Task<IEnumerable<RefreshToken>> GetAllAsync();
        IEnumerable<RefreshToken> Find(Func<RefreshToken, bool> predicate);
        Task CreateAsync(RefreshToken entity);
        Task UpdateAsync(RefreshToken entity);
        Task DeleteAsync(Guid id);
    }
}

using Microsoft.EntityFrameworkCore;
using Miniblog.Models.Entities;
using Miniblog.Models.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miniblog.Models.Services
{
    public class RefreshTokenRepository : IPlainRepository<RefreshToken>
    {
        public MiniblogDb Db { get; private set; }
        public RefreshTokenRepository(MiniblogDb db)
        {
            Db = db;
        }
        public async Task CreateAsync(RefreshToken entity)
        {
            Db.RefreshTokens.Add(entity);
            await Db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var refreshToken = await Db.RefreshTokens.FindAsync(id);
            Db.RefreshTokens.Remove(refreshToken);
            await Db.SaveChangesAsync();
        }

        public IEnumerable<RefreshToken> Find(Func<RefreshToken, bool> predicate)
        {
            return Db.RefreshTokens.Where(predicate).ToList();
        }

        public async Task<IEnumerable<RefreshToken>> GetAllAsync()
        {
            return await Db.RefreshTokens.ToListAsync();
        }

        public async Task<RefreshToken> GetByIdAsync(Guid id)
        {
            return await Db.RefreshTokens.FindAsync(id);
        }

        public async Task UpdateAsync(RefreshToken entity)
        {
            Db.RefreshTokens.Update(entity);
            await Db.SaveChangesAsync();
        }
    }
}

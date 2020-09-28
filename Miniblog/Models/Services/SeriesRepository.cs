using Microsoft.EntityFrameworkCore;
using Miniblog.Models.Entities;
using Miniblog.Models.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miniblog.Models.Services
{
    public class SeriesRepository : IPlainRepository<Series>
    {
        public MiniblogDb Db { get; private set; }
        public SeriesRepository(MiniblogDb db)
        {
            Db = db;
        }
        public async Task CreateAsync(Series entity)
        {
            Db.Series.Add(entity);
            await Db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var series = await Db.Series.FindAsync(id);
            Db.Series.Remove(series);
            await Db.SaveChangesAsync();
        }

        public IEnumerable<Series> Find(Func<Series, bool> predicate)
        {
            return Db.Series.Where(predicate).ToList();
        }

        public async Task<IEnumerable<Series>> GetAllAsync()
        {
            return await Db.Series.ToListAsync();
        }

        public async Task<Series> GetByIdAsync(Guid id)
        {
            var series = await Db.Series.FindAsync(id);
            await Db.Entry(series).Collection(s => s.Articles).LoadAsync();
            await Db.Entry(series).Reference(s => s.User).LoadAsync();
            return series;
        }

        public async Task UpdateAsync(Series entity)
        {
            Db.Series.Update(entity);
            await Db.SaveChangesAsync();
        }
    }
}

using Core.Contract;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class Repository<T>(HealthcareDb ctxt) : IGenericService<T> where T: class
    {
        private readonly HealthcareDb _ctxt = ctxt;
        private readonly DbSet<T> _dbset = ctxt.Set<T>();

        public async Task AddAsync(T entity)
        {
            await _dbset.AddAsync(entity);
            await _ctxt.SaveChangesAsync();
        }
        public async Task DeleteAsync(Guid id)
        {
            var e = await _dbset.FindAsync(id);
            if (e == null) return;
            _dbset.Remove(e);
            await _ctxt.SaveChangesAsync();
        }
        public async Task<T?> GetAsync(Guid id)
        {
            return await _dbset.FindAsync(id);
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbset.ToListAsync();
        }
        public async Task UpdateAsync(T entity)
        {
            _dbset.Update(entity);
            await _ctxt.SaveChangesAsync();
        }
    }
}

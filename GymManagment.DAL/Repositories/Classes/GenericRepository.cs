using GymManagment.DAL.Data.DbContexts;
using GymManagment.DAL.Data.Models;
using GymManagment.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace GymManagment.DAL.Repositories.Classes
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity, new()
    {
        private readonly GymManagmentDbContext _dbContext;
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepository(GymManagmentDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<TEntity>();
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
        {
            return await _dbSet.AnyAsync(predicate, ct);
        }

        public async Task<int> CreateAsync(TEntity entity, CancellationToken ct = default)
        {
            _dbSet.Add(entity);
            return await _dbContext.SaveChangesAsync(ct);
        }

        public async Task<int> DeleteAsync(TEntity entity, CancellationToken ct = default)
        {
            _dbSet.Remove(entity);
            return await _dbContext.SaveChangesAsync(ct);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(bool track = false, CancellationToken ct = default)
        {
            IQueryable<TEntity> query = track ? _dbSet : _dbSet.AsNoTracking();
            return await query.ToListAsync(ct);
        }

        public async Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _dbSet.FindAsync(id);

        public async Task<int> UpdateAsync(TEntity entity, CancellationToken ct = default)
        {
            _dbSet.Update(entity);
            return await _dbContext.SaveChangesAsync(ct);
        }
    }
}

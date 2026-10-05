using GymManagment.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagment.DAL.Repositories.Interfaces
{
    public interface IGenericRepository<TEntity>  where TEntity : BaseEntity , new()
    {
        Task<IEnumerable<TEntity>> GetAllAsync(bool track = false, CancellationToken ct = default);
        Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<int> CreateAsync(TEntity entity, CancellationToken ct = default);
        Task<int> UpdateAsync(TEntity entity, CancellationToken ct = default);
        Task<int> DeleteAsync(TEntity entity, CancellationToken ct = default);
    }
}

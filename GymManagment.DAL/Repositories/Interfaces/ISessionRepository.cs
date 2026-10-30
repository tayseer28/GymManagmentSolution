using GymManagment.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace GymManagment.DAL.Repositories.Interfaces
{
    public interface ISessionRepository : IGenericRepository<Session>
    {

        Task<IEnumerable<Session>> GetAllSessionsWithTrainerAndCategoryAsync(
       Expression<Func<Session, bool>>? predicate = null,
       CancellationToken ct = default);

        Task<Session?> GetSessionWithTrainerAndCategoryAsync(int sessionId, CancellationToken ct = default);

        Task<int> GetCountOfBookedSlotsAsync(int sessionId, CancellationToken ct = default);
    }

}

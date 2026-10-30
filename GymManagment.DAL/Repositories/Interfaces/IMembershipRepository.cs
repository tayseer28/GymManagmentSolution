using GymManagment.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace GymManagment.DAL.Repositories.Interfaces
{
    public interface IMembershipRepository : IGenericRepository<Membership>
    {
        Task<List<Membership>> GetAllMembershipsWithMemberAndPlanAsync(Expression<Func<Membership, bool>>? predicate = null, CancellationToken ct = default);

    }
}

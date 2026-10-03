using GymManagment.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagment.DAL.Repositories.Interfaces
{
    public interface IPlanRepository
    {
        Task<IEnumerable<Plan>> GetAllPlansAsync(bool track = false , CancellationToken ct = default);
        Task<Plan?> GetPlanByIdAsync(int id, CancellationToken ct = default);
        Task<int> CreatePlanAsync(Plan plan, CancellationToken ct = default);
        Task<int> UpdatePlanAsync(Plan plan, CancellationToken ct = default);
        Task<int> DeletePlanAsync(Plan plan, CancellationToken ct = default);
    }
}

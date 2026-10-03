using GymManagment.DAL.Repositories.Interfaces;
using GymManagment.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagment.DAL
{
    public class MockPlanRepository : IPlanRepository
    {
        public async Task<IEnumerable<Plan>> GetAllPlansAsync(bool track = false, CancellationToken ct = default)
        {
            var list = new List<Plan> {
                new Plan() { Name = "Test"}
            };
            return list;
        }
        public Task<int> CreatePlanAsync(Plan plan, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeletePlanAsync(Plan plan, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }


        public Task<Plan?> GetPlanByIdAsync(int id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdatePlanAsync(Plan plan, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}

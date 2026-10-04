using GymManagment.DAL.Data.DbContexts;
using GymManagment.DAL.Data.Models;
using GymManagment.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagment.DAL.Repositories.Classes
{
    public class PlanRepository : IPlanRepository
    {
        private readonly GymManagmentDbContext dbContext;
        public PlanRepository(GymManagmentDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<int> CreatePlanAsync(Plan plan, CancellationToken ct = default)
        {
            dbContext.Add(plan); // we use Add() not AddAsync() because Add just changes the state to be added [add in memory]
                                 // so there is no connection to the database yet to require async 
                                 // usin AddAsync() will not cause a problem it is just an overhead

            return await dbContext.SaveChangesAsync(ct); // here is we need the async because SaveChangesAsync() will connect to the database 
        }

        public async Task<int> DeletePlanAsync(Plan plan, CancellationToken ct = default)
        {
            dbContext.Remove(plan);
            return await dbContext.SaveChangesAsync(ct);
        }

        public async Task<IEnumerable<Plan>> GetAllPlansAsync(bool track = false, CancellationToken ct = default)
        {
            IQueryable<Plan> query = track ? dbContext.Plans : dbContext.Plans.AsNoTracking();
            return await query.ToListAsync(ct);
        }

        public async Task<Plan?> GetPlanByIdAsync(int id, CancellationToken ct = default)
        {
            return await dbContext.Plans.FindAsync(id, ct); 

        }

        public async Task<int> UpdatePlanAsync(Plan plan, CancellationToken ct = default)
        {
            dbContext.Update(plan);
            return await dbContext.SaveChangesAsync(ct);
        }
    }
}

using GymManagementBLL.Common;
using GymManagment.BLL.Services.Interfaces;
using GymManagment.BLL.ViewModel.PlanViewModels;
using GymManagment.DAL.Data.Models;
using GymManagment.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagment.BLL.Services.Classes
{
    public class PlanService : IPlanService
    {
        private readonly IGenericRepository<Plan> _planRepository;
        private readonly IGenericRepository<Membership> _membershipRepository;

        public PlanService(IGenericRepository<Plan> planReposityory,
            IGenericRepository<Membership> membershipRepository)
        {
            _planRepository = planReposityory;
            _membershipRepository = membershipRepository;
        }
        public async Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default)
        {
            var plans = await _planRepository.GetAllAsync(ct: ct);
            if (!plans.Any()) return [];
            var planViewModels = plans.Select(p => new PlanViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price.ToString(),
                Description = p.Description,
                Duration = p.DurationDays.ToString(),
                IsActive = p.IsActive 

            });
            return planViewModels;
        }


        public async Task<PlanViewModel?> GetPlanDetailsByIdAsync(int planId, CancellationToken ct = default)
        {
            var plan = await _planRepository.GetByIdAsync(planId, ct);
            if (plan is null ) return null;
            return new PlanViewModel
            {
                Name = plan.Name,
                Price = plan.Price.ToString(),
                Duration = plan.DurationDays.ToString(),
                Description = plan.Description,
                IsActive = plan.IsActive
            };

        }

        private async Task<bool> HasActiveMembershipsAsync(int planId, CancellationToken ct)
        {
             return await _membershipRepository.AnyAsync(M => M.PlanId == planId && M.EndDate > DateTime.Now, ct);
        }

        public async Task<UpdatePlanViewModel?> GetPlanToUpdateAsync(int Planid, CancellationToken ct = default)
        {
            var plan = await _planRepository.GetByIdAsync(Planid, ct);
            if (plan is null || !plan.IsActive) return null;
            if (await HasActiveMembershipsAsync(Planid, ct)) return null; //cannot edit or change in plans with active memberships 
            return new UpdatePlanViewModel
            {
                PlanName = plan.Name,
                Description = plan.Description,
                DurationDays = plan.DurationDays,
                Price = plan.Price
            };
        }

        public async Task<bool> UpdatePlanDetailsAsync(int planId, UpdatePlanViewModel plan, CancellationToken ct = default)
        {
            var model = await _planRepository.GetByIdAsync(planId, ct);
            if (model is null) return false;
            if (await HasActiveMembershipsAsync(planId, ct)) return false;
            model.DurationDays = plan.DurationDays;
            model.Price = plan.Price;
            model.Description = plan.Description;
            model.UpdatedAt = DateTime.Now;

            var result = await _planRepository.UpdateAsync(model, ct);
            return result > 0;
        }

        public async Task<bool> ToggleActivationAsync(int planId, CancellationToken ct = default)
        {
            var plan = await _planRepository.GetByIdAsync(planId, ct);
            if (plan is null) return false;
            if (plan.IsActive && await HasActiveMembershipsAsync(planId, ct)) return false;
            plan.IsActive = !plan.IsActive;
            plan.UpdatedAt = DateTime.Now;
            var result = await _planRepository.UpdateAsync(plan, ct);
            return result > 0;
        }
    }
}

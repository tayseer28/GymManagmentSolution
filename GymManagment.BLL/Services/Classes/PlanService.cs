using GymManagementBLL.Common;
using GymManagment.BLL.Services.Interfaces;
using GymManagment.BLL.ViewModel.PlanViewModels;
using GymManagment.DAL.Data.Models;
using GymManagment.DAL.Repositories.Classes;
using GymManagment.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagment.BLL.Services.Classes
{
    public class PlanService : IPlanService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PlanService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default)
        {
            var plans = await _unitOfWork.GetRepository<Plan>().GetAllAsync(ct: ct);
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
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(planId, ct);
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
             return await _unitOfWork.GetRepository<Membership>().AnyAsync(M => M.PlanId == planId && M.EndDate > DateTime.Now, ct);
        }

        public async Task<UpdatePlanViewModel?> GetPlanToUpdateAsync(int Planid, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(Planid, ct);
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
            var model = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(planId, ct);
            if (model is null) return false;
            if (await HasActiveMembershipsAsync(planId, ct)) return false;
            model.DurationDays = plan.DurationDays;
            model.Price = plan.Price;
            model.Description = plan.Description;
            model.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Plan>().Update(model);
            var result = await _unitOfWork.SaveChangesAsync(ct);

            return result > 0;
        }

        public async Task<bool> ToggleActivationAsync(int planId, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(planId, ct);
            if (plan is null) return false;
            if (plan.IsActive && await HasActiveMembershipsAsync(planId, ct)) return false;
            plan.IsActive = !plan.IsActive;
            plan.UpdatedAt = DateTime.Now;
            _unitOfWork.GetRepository<Plan>().Update(plan);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0;
        }
    }
}

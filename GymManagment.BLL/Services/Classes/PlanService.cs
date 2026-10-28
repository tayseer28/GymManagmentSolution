using AutoMapper;
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
        private readonly IMapper _mapper;

        public PlanService(IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default)
        {
            var plans = await _unitOfWork.GetRepository<Plan>().GetAllAsync(ct: ct);
            if (!plans.Any()) return [];
            //var planViewModels = plans.Select(p => new PlanViewModel
            //{
            //    Id = p.Id,
            //    Name = p.Name,
            //    Price = p.Price.ToString(),
            //    Description = p.Description,
            //    Duration = p.DurationDays.ToString(),
            //    IsActive = p.IsActive 

            //});
            var planViewModels = _mapper.Map <IEnumerable<Plan>, IEnumerable<PlanViewModel>>(plans);
            return planViewModels;
        }


        public async Task<Result<PlanViewModel>> GetPlanDetailsByIdAsync(int planId, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(planId, ct);
            if (plan is null ) return Result<PlanViewModel>.NotFound("Plan Not Found");
            //return new PlanViewModel
            //{
            //    Name = plan.Name,
            //    Price = plan.Price.ToString(),
            //    Duration = plan.DurationDays.ToString(),
            //    Description = plan.Description,
            //    IsActive = plan.IsActive
            //};
            return Result<PlanViewModel>.Ok(_mapper.Map<Plan, PlanViewModel>(plan));

        }

        private async Task<bool> HasActiveMembershipsAsync(int planId, CancellationToken ct)
        {
             return await _unitOfWork.GetRepository<Membership>().AnyAsync(M => M.PlanId == planId && M.EndDate > DateTime.Now, ct);
        }

        public async Task<Result<UpdatePlanViewModel>> GetPlanToUpdateAsync(int Planid, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(Planid, ct);
            if (plan is null || !plan.IsActive) return null;
            if (await HasActiveMembershipsAsync(Planid, ct)) return Result<UpdatePlanViewModel>.Fail("Cannot Edit Or Change in Plans With Active Membership"); //cannot edit or change in plans with active memberships 
            //return new UpdatePlanViewModel
            //{
            //    PlanName = plan.Name,
            //    Description = plan.Description,
            //    DurationDays = plan.DurationDays,
            //    Price = plan.Price
            //};
            return Result<UpdatePlanViewModel>.Ok( _mapper.Map<Plan, UpdatePlanViewModel>(plan));
        }

        public async Task<Result> UpdatePlanDetailsAsync(int planId, UpdatePlanViewModel plan, CancellationToken ct = default)
        {
            var model = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(planId, ct);
            if (model is null) return Result.NotFound("Plan Not Found");
            if (await HasActiveMembershipsAsync(planId, ct)) return Result.Fail("Cannot Update Plan With Active Membership");
            //model.DurationDays = plan.DurationDays;
            //model.Price = plan.Price;
            //model.Description = plan.Description;
            //model.UpdatedAt = DateTime.Now;
            _mapper.Map(plan, model);
            _unitOfWork.GetRepository<Plan>().Update(model);
            var result = await _unitOfWork.SaveChangesAsync(ct);

            return result > 0 ? Result.Ok() : Result.Fail("Update Plan Failed");
        }

        public async Task<Result> ToggleActivationAsync(int planId, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(planId, ct);
            if (plan is null) return Result.NotFound("Plan Not Found");
            if (plan.IsActive && await HasActiveMembershipsAsync(planId, ct)) return Result.Fail("Cannot Remove Active or Has Active Memberships Plan");
            plan.IsActive = !plan.IsActive;
            plan.UpdatedAt = DateTime.Now;
            _unitOfWork.GetRepository<Plan>().Update(plan);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Remove Plan Failed");
        }
    }
}

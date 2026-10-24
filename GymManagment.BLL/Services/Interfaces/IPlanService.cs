using GymManagementBLL.Common;
using GymManagment.BLL.ViewModel.PlanViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagment.BLL.Services.Interfaces
{
    public interface IPlanService
    {
        Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default);
        Task<PlanViewModel?> GetPlanDetailsByIdAsync(int planId, CancellationToken ct = default);
        Task<UpdatePlanViewModel?> GetPlanToUpdateAsync(int Planid, CancellationToken ct = default);
        Task<bool> UpdatePlanDetailsAsync(int planId, UpdatePlanViewModel plan, CancellationToken ct = default);
        Task<bool> ToggleActivationAsync(int planId, CancellationToken ct = default);



    }
}

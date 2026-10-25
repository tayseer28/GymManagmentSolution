using GymManagment.BLL.ViewModel.MemberViewModels;
using GymManagment.BLL.ViewModel.TrainerViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagment.BLL.Services.Interfaces
{
    public interface ITrainerService
    {
        Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default);
        Task<TrainerDetailsViewModel?> GetTrainerDetailsByIdAsync(int id, CancellationToken ct = default);
        Task<bool> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct = default);
        Task<TrainerToUpdateViewModel?> GetTrainerToUpdateAsync(int trainerId, CancellationToken ct = default);
        Task<bool> UpdateTrainerDetailsAsync(int trainerId, TrainerToUpdateViewModel model, CancellationToken ct = default);
        Task<bool> RemoveTrainerAsync(int trainerId, CancellationToken ct = default);
    }
}

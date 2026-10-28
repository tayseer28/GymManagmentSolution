using GymManagment.BLL.Services.Interfaces;
using GymManagment.BLL.ViewModel.AnalyticsViewModels;
using GymManagment.DAL.Data.Models;
using GymManagment.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagment.BLL.Services.Classes
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AnalyticsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<AnalyticsViewModel> GetDataAsync(CancellationToken ct = default)
        {
            var now = DateTime.Now;
            var upcomingSessions = await _unitOfWork.GetRepository<Session>().CountAsync(S => S.StartDate > now, ct);
            var ongoingSessions = await _unitOfWork.GetRepository<Session>().CountAsync(S => S.StartDate <= now && S.EndDate >= now, ct);
            var completedSessions = await _unitOfWork.GetRepository<Session>().CountAsync(S => S.EndDate < now, ct);
            var totalMembers = await _unitOfWork.GetRepository<Member>().CountAsync(ct: ct);
            var totalTrainers = await _unitOfWork.GetRepository<Trainer>().CountAsync(ct: ct);
            var activeMembers = await _unitOfWork.GetRepository<Membership>().CountAsync( X => X.EndDate > now , ct);

            return new AnalyticsViewModel
            {
                TotalMembers = totalMembers,
                ActiveMembers = activeMembers,
                TotalTrainers = totalMembers,
                CompletedSessions = completedSessions,
                OngoingSessions = ongoingSessions,
                UpcomingSessions = upcomingSessions
            };
        }
    }
}

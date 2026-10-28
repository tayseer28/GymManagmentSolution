using GymManagment.BLL.ViewModel.AnalyticsViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagment.BLL.Services.Interfaces
{
    public interface IAnalyticsService
    {
        Task<AnalyticsViewModel> GetDataAsync(CancellationToken ct = default);

    }
}

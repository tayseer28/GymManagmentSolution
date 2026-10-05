using GymManagment.BLL.ViewModel.MemberViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagment.BLL.Services.Interfaces
{
    public interface IMemberService
    {
        Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default);
    }
}

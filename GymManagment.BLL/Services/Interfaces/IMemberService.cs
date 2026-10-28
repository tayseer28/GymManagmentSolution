using GymManagementBLL.Common;
using GymManagment.BLL.ViewModel.MemberViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagment.BLL.Services.Interfaces
{
    public interface IMemberService
    {
        Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default);
        Task<Result> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default);
        Task<Result<MemberDetailsViewModel>> GetMemberDetailsByIdAsync(int id, CancellationToken ct = default);
        Task<Result<HealthRecordViewModel>> GetMemberHealthRecordDetailsAsync(int memberId, CancellationToken ct = default);
        Task<Result< MemberToUpdateViewModel>> GetMemberToUpdateAsync(int memberId, CancellationToken ct = default);
        Task<Result> UpdateMemberDetailsAsync(int memberId, MemberToUpdateViewModel member, CancellationToken ct = default);
        Task<Result> RemoveMember(int memberId, CancellationToken ct = default);
        
    }
}

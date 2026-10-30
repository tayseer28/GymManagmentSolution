using AutoMapper;
using GymManagment.BLL.Common;
using GymManagment.BLL.Attachment;
using GymManagment.BLL.Services.Interfaces;
using GymManagment.BLL.ViewModel.MemberViewModels;
using GymManagment.DAL.Data.Models;
using GymManagment.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagment.BLL.Services.Classes
{
    public class MemberService : IMemberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAttachmentService _attachmentService;

        public MemberService(IUnitOfWork unitOfWork,
            IMapper mapper,
            IAttachmentService attachmentService)

        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _attachmentService = attachmentService;
        }

        public async Task<Result> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {
            // if email or phone exist return false 
            var emailExist = await _unitOfWork.GetRepository<Member>().AnyAsync(M => M.Email == model.Email, ct);
            var phoneExist = await _unitOfWork.GetRepository<Member>().AnyAsync(M => M.Phone == model.Phone, ct);
            if (emailExist || phoneExist) return Result.Fail("A member with this phone or email already exist");

            // Upload Photo
            var storedPhotoName = await _attachmentService.UploadAsync(model.PhotoFile.OpenReadStream(), model.PhotoFile.FileName, "MembersPhoto", ct);
            if (string.IsNullOrWhiteSpace(storedPhotoName.Value)) return Result.Validation("Invalid Profile Image");
            // else trnasform the model to member and save it to the database
            var member = _mapper.Map<CreateMemberViewModel, Member>(model);
            member.Photo = storedPhotoName.Value;
            //var result = await _memberRepository.CreateAsync(member, ct);
            _unitOfWork.GetRepository<Member>().Create(member);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            if (result > 0)
                return Result.Ok();
            else
            {
                // Delete the Uploaded photo 
                _attachmentService.Delete(storedPhotoName.Value, "MembersPhoto");
                return Result.Fail("Failed To Create Member");
            }
        }

        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default)
        {
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync(ct: ct);
            if (!members.Any()) return [];
            var memberViewModels = _mapper.Map<IEnumerable<Member>, IEnumerable<MemberViewModel>>(members);
            return memberViewModels;
        }

        public async Task<Result<MemberDetailsViewModel>> GetMemberDetailsByIdAsync(int id, CancellationToken ct = default)
        {
            //check if member is exist 
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct);
            if (member is null) return Result<MemberDetailsViewModel>.NotFound("Member Not Found");
            var memberDetails = _mapper.Map<Member, MemberDetailsViewModel>(member);

            // realted data : 
            var activeMembership = await _unitOfWork.GetRepository<Membership>().FirstOrDefaultAsync(M => M.MemberId == member.Id && M.EndDate > DateTime.Now);
            if (activeMembership is not null)
            {
                var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(activeMembership.PlanId);
                memberDetails.PlanName = plan?.Name ?? "No Plane";
                memberDetails.MembershipStartDate = activeMembership.CreatedAt.ToString();
                memberDetails.MembershipEndtDate = activeMembership.EndDate.ToString();
            }

            return Result<MemberDetailsViewModel>.Ok(memberDetails);
        }

        public async Task<Result<HealthRecordViewModel>> GetMemberHealthRecordDetailsAsync(int memberId, CancellationToken ct = default)
        {
            var healthRecord = await _unitOfWork.GetRepository<HealthRecord>().FirstOrDefaultAsync(HR => HR.MemberId == memberId);
            if (healthRecord is null) return Result<HealthRecordViewModel>.NotFound("Health Record Not Found");
            else
            {
                return Result<HealthRecordViewModel>.Ok(_mapper.Map<HealthRecord, HealthRecordViewModel>(healthRecord));
            }

        }


        public async Task<Result> UpdateMemberDetailsAsync(int memberId, MemberToUpdateViewModel member, CancellationToken ct = default)
        {
            var repo = _unitOfWork.GetRepository<Member>();
            var model = await repo.GetByIdAsync(memberId, ct);
            if (model is null) return Result.NotFound("Member Not Found");

            // Self-exclusion: check if email/phone exists on a DIFFERENT member.
            if (await repo.AnyAsync(m => m.Email == member.Email && m.Id != memberId, ct))
                return Result.Fail("Another member is already using this email.");
            if (await repo.AnyAsync(m => m.Phone == member.Phone && m.Id != memberId, ct))
                return Result.Fail("Another member is already using this phone number.");

            _mapper.Map(member, model);
            model.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Member>().Update(model);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed To update Member");

        }

        async Task<Result<MemberToUpdateViewModel>> IMemberService.GetMemberToUpdateAsync(int memberId, CancellationToken ct)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberId, ct);
            if (member == null) return Result<MemberToUpdateViewModel>.NotFound("Member Not Found");
            else
            {
                return Result<MemberToUpdateViewModel>.Ok(_mapper.Map<Member, MemberToUpdateViewModel>(member));
            }

        }
        public async Task<Result> RemoveMember(int memberId, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberId, ct);
            if (member is null) return Result.NotFound("Member Not Found");
            var hasFutureBookings = await _unitOfWork.GetRepository<Booking>().AnyAsync(B => B.MemberId == memberId && B.Session.StartDate > DateTime.Now, ct);
            if (hasFutureBookings) return Result.Fail("Cannot delete a member with upcoming sessions."); //if member has future bookings then we cannot delete him

            _unitOfWork.GetRepository<Member>().Delete(member);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            //return result > 0 ? Result.Ok() : Result.Fail("Failed To Delete Member");
            if (result > 0)
            {
                if (!string.IsNullOrEmpty(member.Photo))
                    _attachmentService.Delete(member.Photo, "members");

                return Result.Ok();
            }
            return Result.Fail("Failed To Delete Member");
        }
    }
}

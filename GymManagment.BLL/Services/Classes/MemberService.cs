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

        public MemberService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {
            // if email or phone exist return false 
            var emailExist = await _unitOfWork.GetRepository<Member>().AnyAsync(M => M.Email == model.Email, ct);
            var phoneExist = await _unitOfWork.GetRepository<Member>().AnyAsync(M => M.Phone == model.Phone, ct);
            if(emailExist || phoneExist) return false;

            // else trnasform the model to member and save it to the database
            var member = new Member
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                Address = new Address
                {
                    City = model.City,
                    Street = model.Street,
                    BuildingNo = model.BuildingNumber
                },
                HealthRecord = new HealthRecord
                {
                    BloodType = model.HealthRecordViewModel.BloodType,
                    Weight = model.HealthRecordViewModel.Weight,
                    Height = model.HealthRecordViewModel.Height,
                    Note = model.HealthRecordViewModel.Note

                }

            };
            //var result = await _memberRepository.CreateAsync(member, ct);
            _unitOfWork.GetRepository<Member>().Create(member);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0;
        }

        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default)
        {
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync(ct : ct);
            if (!members.Any()) return [];

            var memberViewModels = members.Select(m => new MemberViewModel
            {
                Id = m.Id,
                Photo = m.Photo,
                Name = m.Name,
                Email = m.Email,
                Gender = m.Gender.ToString(),
                Phone = m.Phone

            });
            return memberViewModels;
        }

        public async Task<MemberDetailsViewModel?> GetMemberDetailsByIdAsync(int id, CancellationToken ct = default)
        {
            //check if member is exist 
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct);
            if(member is null) return null;

            var memberDetails = new MemberDetailsViewModel
            {
                Name = member.Name,
                Email = member.Email,
                Phone = member.Phone,
                Address = $"{member.Address.Street} St. , {member.Address.City} , Building No. {member.Address.BuildingNo}",
                DateOfBirth = member.DateOfBirth.ToShortDateString(),
                Gender = member.Gender.ToString()
                
            };

            // realted data : 
            var activeMembership = await _unitOfWork.GetRepository<Membership>().FirstOrDefaultAsync(M => M.MemberId == member.Id && M.EndDate > DateTime.Now);
           if(activeMembership is not null)
            {
                var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(activeMembership.PlanId);
                memberDetails.PlanName = plan?.Name ?? "No Plane";
                memberDetails.MembershipStartDate = activeMembership.CreatedAt.ToString();
                memberDetails.MembershipEndtDate = activeMembership.EndDate.ToString();
            }

            return memberDetails;
        }

        public async Task<HealthRecordViewModel?> GetMemberHealthRecordDetailsAsync(int memberId, CancellationToken ct = default)
        {
            var healthRecord = await _unitOfWork.GetRepository<HealthRecord>().FirstOrDefaultAsync(HR => HR.MemberId == memberId);
            if (healthRecord is null) return null;
            else
            {
               return new HealthRecordViewModel
                {
                    Height = healthRecord.Height,
                    Weight = healthRecord.Weight,
                    BloodType = healthRecord.BloodType,
                    Note = healthRecord.Note
                    
                };
            }
           
        }


        public async Task<bool> UpdateMemberDetailsAsync(int memberId, MemberToUpdateViewModel member, CancellationToken ct = default)
        {
            var model = await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberId, ct);
            if (model is null) return false;
            // if email or phone exist return false 
            var emailExist = await _unitOfWork.GetRepository<Member>().AnyAsync(M => M.Email == member.Email && M.Id != memberId);
            var phoneExist = await _unitOfWork.GetRepository<Member>().AnyAsync(M => M.Phone == member.Phone && M.Id != memberId);
            if (emailExist || phoneExist) return false;

            model.Email = member.Email;
            model.Phone = member.Phone;
            model.Address.BuildingNo = member.BuildingNumber;
            model.Address.City = member.City;
            model.Address.Street = member.Street;
            model.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Member>().Update(model);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0;

  
        }

        async Task<MemberToUpdateViewModel?> IMemberService.GetMemberToUpdateAsync(int memberId, CancellationToken ct)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberId, ct);
            if (member == null) return null;
            else
            {
                return new MemberToUpdateViewModel
                {
                    Name = member.Name,
                    Photo = member.Photo,
                    Email = member.Email,
                    Phone = member.Phone,
                    BuildingNumber = member.Address.BuildingNo,
                    City = member.Address.City,
                    Street = member.Address.Street,
                };
            }
                  
        }
        public async Task<bool> RemoveMember(int memberId, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberId, ct);
            if (member is null) return false;
            var hasFutureBookings = await _unitOfWork.GetRepository<Booking>().AnyAsync(B => B.MemberId == memberId && B.Session.StartDate > DateTime.Now, ct);
            if (hasFutureBookings) return false; //if member has future bookings then we cannot delete him

            _unitOfWork.GetRepository<Member>().Delete(member);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0;
        }
    }
}

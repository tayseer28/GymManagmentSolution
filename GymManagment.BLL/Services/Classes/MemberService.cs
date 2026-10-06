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
        private readonly IGenericRepository<Member> _memberRepository;
        public MemberService(IGenericRepository<Member> memberRepository)
        {
            _memberRepository = memberRepository;
        }

        public async Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {
            // if email or phone exist return false 
            var emailExist = await _memberRepository.AnyAsync(M => M.Email == model.Email, ct);
            var phoneExist = await _memberRepository.AnyAsync(M => M.Phone == model.Phone, ct);
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
            var result = await _memberRepository.CreateAsync(member, ct);
            return result > 0;
        }

        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default)
        {
            var members = await _memberRepository.GetAllAsync(ct : ct);
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
    }
}

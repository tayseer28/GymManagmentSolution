using AutoMapper;
using AutoMapper.Execution;
using GymManagment.BLL.Common;
using GymManagment.BLL.Services.Interfaces;
using GymManagment.BLL.ViewModel.TrainerViewModels;
using GymManagment.DAL.Data.Models;
using GymManagment.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagment.BLL.Services.Classes
{
    public class TrainerService : ITrainerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TrainerService(IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default)
        {
            var trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct: ct);
            if (trainers is null) return [];
            //return trainers.Select(T => new TrainerViewModel()
            //{
            //    Id = T.Id,
            //    Name = T.Name,
            //    Email = T.Email,
            //    Phone = T.Phone,
            //    Speciality = T.Speciality.ToString()
            //});
            return _mapper.Map<IEnumerable<Trainer>, IEnumerable<TrainerViewModel>>(trainers);
        }

        public async Task<Result> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct = default)
        {
            var repo = _unitOfWork.GetRepository<Trainer>();

            //var isEmailExist = await _unitOfWork.GetRepository<Trainer>().AnyAsync(T => T.Email == model.Email, ct);
            //var isPhoneExist = await _unitOfWork.GetRepository<Trainer>().AnyAsync(T => T.Phone == model.Phone, ct);
            //if (isEmailExist || isPhoneExist) return false;

            if (await repo.AnyAsync(m => m.Email == model.Email , ct))
                return Result.Fail("Another member is already using this email.");
            if (await repo.AnyAsync(m => m.Phone == model.Phone , ct))
                return Result.Fail("Another member is already using this phone number.");
            //var trainer = new Trainer()
            //{
            //    Name = model.Name,
            //    Phone = model.Phone,
            //    Email = model.Email,
            //    Gender = model.Gender,
            //    DateOfBirth = model.DateOfBirth,
            //    Speciality = model.Specialties,
            //    Address = new Address
            //    {
            //        BuildingNo = model.BuildingNumber,
            //        City = model.City,
            //        Street = model.Street
            //    }

            //};
            var trainer = _mapper.Map<CreateTrainerViewModel, Trainer>(model);
            _unitOfWork.GetRepository<Trainer>().Create(trainer);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Create Trainer");
        }
        public async Task<Result <TrainerDetailsViewModel>> GetTrainerDetailsByIdAsync(int id, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
            if (trainer is null) return Result<TrainerDetailsViewModel>.NotFound("Trainer Not Found");
            //return new TrainerDetailsViewModel
            //{
            //    Name = trainer.Name,
            //    Email = trainer.Email,
            //    Phone = trainer.Phone,
            //    Speciality = trainer.Speciality.ToString(),
            //    DateOfBirth = trainer.DateOfBirth.ToShortDateString(),
            //    Address = $"{trainer.Address.Street} St. , {trainer.Address.City} , Building No. {trainer.Address.BuildingNo}",
            //};
            return Result<TrainerDetailsViewModel>.Ok(_mapper.Map<Trainer, TrainerDetailsViewModel>(trainer));
        }

        public async Task<Result<TrainerToUpdateViewModel>> GetTrainerToUpdateAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(trainerId, ct);
            if (trainer is null) return Result<TrainerToUpdateViewModel>.NotFound("Trainer Not Found");
            //return new TrainerToUpdateViewModel
            //{
            //    Name = trainer.Name,
            //    Email = trainer.Email,
            //    Phone = trainer.Phone,
            //    BuildingNumber = trainer.Address.BuildingNo,
            //    City = trainer.Address.City,
            //    Street = trainer.Address.Street,
            //    Specialties = trainer.Speciality
            //};
            return Result<TrainerToUpdateViewModel>.Ok( _mapper.Map<Trainer, TrainerToUpdateViewModel>(trainer));
        }

        public async Task<Result> UpdateTrainerDetailsAsync(int trainerId, TrainerToUpdateViewModel model, CancellationToken ct = default)
        {
            var repo = _unitOfWork.GetRepository<Trainer>();

            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(trainerId, ct);
            if (trainer is null) return Result.NotFound("Trainer Not Found");
            //if (await _unitOfWork.GetRepository<Trainer>().AnyAsync(T => T.Email == model.Email && T.Id != trainerId)) return false;
            //if (await _unitOfWork.GetRepository<Trainer>().AnyAsync(T => T.Phone == model.Phone && T.Id != trainerId)) return false;
            if (await repo.AnyAsync(m => m.Email == model.Email && m.Id != trainerId, ct))
                return Result.Fail("Another member is already using this email.");
            if (await repo.AnyAsync(m => m.Phone == model.Phone && m.Id != trainerId, ct))
                return Result.Fail("Another member is already using this phone number.");
            //trainer.Email = model.Email;
            //trainer.Phone = model.Phone;
            //trainer.Speciality = model.Specialties;
            //trainer.Address.Street = model.Street;
            //trainer.Address.BuildingNo = model.BuildingNumber;
            //trainer.Address.City = model.City;
             _mapper.Map(model, trainer);
            trainer.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Trainer>().Update(trainer);
            var result = await _unitOfWork.SaveChangesAsync( ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed To update Trainer");
        }

        public async Task<Result> RemoveTrainerAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(trainerId, ct);
            if (trainer is null) return Result.NotFound("Trainer Not Found");
            bool hasFutureSessions = await _unitOfWork.GetRepository<Session>().AnyAsync(s => s.TrainerId == trainerId && s.StartDate > DateTime.Now);
            if (hasFutureSessions) return Result.Fail("Cannot Delete Trainer That Has Future Sessions");

            _unitOfWork.GetRepository<Trainer>().Delete(trainer);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Remove Trainer");
        }
    }
}

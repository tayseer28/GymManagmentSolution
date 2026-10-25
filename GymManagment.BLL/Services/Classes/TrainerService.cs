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
        private readonly IGenericRepository<Trainer> _trainerRepository;
        private readonly IGenericRepository<Session> _sessionRepository;
        public TrainerService(IGenericRepository<Trainer> trainerRepository,
            IGenericRepository<Session> sessionRepository)
        {
            _trainerRepository = trainerRepository;
            _sessionRepository = sessionRepository;
        }

        public async Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default)
        {
            var trainers = await _trainerRepository.GetAllAsync(ct: ct);
            if (trainers is null) return [];
            return trainers.Select(T => new TrainerViewModel()
            {
                Id = T.Id,
                Name = T.Name,
                Email = T.Email,
                Phone = T.Phone,
                Speciality = T.Speciality.ToString()
            });
        }

        public async Task<bool> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct = default)
        {
            var isEmailExist = await _trainerRepository.AnyAsync(T => T.Email == model.Email, ct);
            var isPhoneExist = await _trainerRepository.AnyAsync(T => T.Phone == model.Phone, ct);
            if (isEmailExist || isPhoneExist) return false;
            var trainer = new Trainer()
            {
                Name = model.Name,
                Phone = model.Phone,
                Email = model.Email,
                Gender = model.Gender,
                DateOfBirth = model.DateOfBirth,
                Speciality = model.Specialties,
                Address = new Address
                {
                    BuildingNo = model.BuildingNumber,
                    City = model.City,
                    Street = model.Street
                }
                
            };
            var result = await _trainerRepository.CreateAsync(trainer, ct);
            return result > 0;
        }
        public async Task<TrainerDetailsViewModel?> GetTrainerDetailsByIdAsync(int id, CancellationToken ct = default)
        {
            var trainer = await _trainerRepository.GetByIdAsync(id, ct);
            if (trainer is null) return null;
            return new TrainerDetailsViewModel
            {
                Name = trainer.Name,
                Email = trainer.Email,
                Phone = trainer.Phone,
                Speciality = trainer.Speciality.ToString(),
                DateOfBirth = trainer.DateOfBirth.ToShortDateString(),
                Address = $"{trainer.Address.Street} St. , {trainer.Address.City} , Building No. {trainer.Address.BuildingNo}",
            };
        }

        public async Task<TrainerToUpdateViewModel?> GetTrainerToUpdateAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer = await _trainerRepository.GetByIdAsync(trainerId, ct);
            if (trainer is null) return null;
            return new TrainerToUpdateViewModel
            {
                Name = trainer.Name,
                Email = trainer.Email,
                Phone = trainer.Phone,
                BuildingNumber = trainer.Address.BuildingNo,
                City = trainer.Address.City,
                Street = trainer.Address.Street,
                Specialties = trainer.Speciality
            };
        }

        public async Task<bool> UpdateTrainerDetailsAsync(int trainerId, TrainerToUpdateViewModel model, CancellationToken ct = default)
        {
            var trainer = await _trainerRepository.GetByIdAsync(trainerId, ct);
            if (trainer is null) return false;
            if (await _trainerRepository.AnyAsync(T => T.Email == model.Email && T.Id != trainerId)) return false;
            if (await _trainerRepository.AnyAsync(T => T.Phone == model.Phone && T.Id != trainerId)) return false;
            trainer.Email = model.Email;
            trainer.Phone = model.Phone;
            trainer.Speciality = model.Specialties;
            trainer.Address.Street = model.Street;
            trainer.Address.BuildingNo = model.BuildingNumber;
            trainer.Address.City = model.City;
            trainer.UpdatedAt = DateTime.Now;

            var result = await _trainerRepository.UpdateAsync(trainer, ct);
            return result > 0;
        }

        public async Task<bool> RemoveTrainerAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer = await _trainerRepository.GetByIdAsync(trainerId, ct);
            if (trainer is null) return false;
            bool hasFutureSessions = await _sessionRepository.AnyAsync(s => s.TrainerId == trainerId && s.StartDate > DateTime.Now);
            if (hasFutureSessions) return false;

            var result = await _trainerRepository.DeleteAsync(trainer, ct);
            return result > 0;
        }
    }
}

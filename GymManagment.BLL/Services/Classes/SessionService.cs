using AutoMapper;
using GymManagementBLL.Common;
using GymManagment.BLL.Services.Interfaces;
using GymManagment.BLL.ViewModel.SessionViewModel;
using GymManagment.DAL.Data.Models;
using GymManagment.DAL.Data.Models.Enums;
using GymManagment.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagment.BLL.Services.Classes
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public SessionService(IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<IEnumerable<SessionViewModel>?> GetAllSessionsAsync(CancellationToken ct = default)
        {
            var sessionRepo = _unitOfWork.SessionRepository;
            var sessions = await _unitOfWork.SessionRepository.GetAllSessionsWithTrainerAndCategoryAsync(ct: ct); //get related data
            if (sessions == null || !sessions.Any()) return null;
            sessions = sessions.OrderByDescending(s => s.EndDate);
            //var mappedSessions = sessions.Select(s => new SessionViewModel
            //{
            //    Id = s.Id,
            //    Capacity = s.Capacity,
            //    Description = s.Description,
            //    TrainerName = s.Trainer.Name,
            //    CategoryName = s.Category.CategoryName,
            //    StartDate = s.StartDate,
            //    EndDate = s.EndDate,
            //});
            var mappedSessions = _mapper.Map<IEnumerable<Session>, IEnumerable<SessionViewModel>>(sessions);
            
            foreach (var session in mappedSessions)
            {
                session.AvailableSlots = session.Capacity - await sessionRepo.GetCountOfBookedSlotsAsync(session.Id, ct);
                // N + ! problem 
            }
            //return mappedSessions;
            return mappedSessions;
        }
        public async Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct = default)
        {
            if (model.EndDate <= model.StartDate) return Result.Validation("EndDate Must be After StartDate");
            if (model.StartDate <= DateTime.Now) return Result.Validation("StartDate Must Be In The Future");
            if (model.Capacity < 1 || model.Capacity > 25) return Result.Validation("Capacity Must be Between 1 and 25");

            // to get the matching trainer speciality with category name
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId);
            if (trainer is null) return Result.NotFound("Trainer Not Fount");
            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(model.CategoryId);
            if (category is null) return Result.NotFound("Category Not Fount");
            var isValidSpeciality = Enum.TryParse<Speciality>(category.CategoryName, true, out var CategorySpeciality);
            if (!isValidSpeciality || trainer.Speciality != CategorySpeciality) return Result.Validation("Cannot Session To This Trainer");

            var session = _mapper.Map<CreateSessionViewModel, Session>(model);
            _unitOfWork.GetRepository<Session>().Create(session);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Create Session");
        }

        async Task<IEnumerable<TrainerSelectViewModel>> ISessionService.GetTrainersForDropDownAsync(CancellationToken ct)
        {
            var result = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct : ct);
            return _mapper.Map<IEnumerable<Trainer>, IEnumerable<TrainerSelectViewModel>>(result);
        }

        async Task<IEnumerable<CategorySelectViewModel>> ISessionService.GetCategoriesForDropDownAsync(CancellationToken ct)
        {
            var result = await _unitOfWork.GetRepository<Category>().GetAllAsync(ct: ct);
            return _mapper.Map<IEnumerable<Category>, IEnumerable<CategorySelectViewModel>>(result);
        }

        public async Task<Result<SessionViewModel>> GetSessionByIdAsync(int sessionId, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetSessionWithTrainerAndCategoryAsync(sessionId, ct);
            if (session is null) return Result<SessionViewModel>.NotFound("Session Not Found");
            var MappedSession = _mapper.Map<Session, SessionViewModel>(session);
            MappedSession.AvailableSlots = session.Capacity - await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(sessionId, ct);
            return Result<SessionViewModel>.Ok(MappedSession);
        }

        public async Task<Result<UpdateSessionViewModel>> GetSessionToUpdateAsync(int sessionId, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(sessionId, ct);
            if (session is null) return Result<UpdateSessionViewModel>.NotFound("Session Not Found");
            if (session.StartDate <= DateTime.Now) return Result<UpdateSessionViewModel>.Fail("Cannot Update Session that Has Already Started");
            var bookingCount = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(sessionId);
            if (bookingCount > 0) return Result<UpdateSessionViewModel>.Fail("Cannot Update Session That Has been Booked");
            var mappedSession = _mapper.Map<Session, UpdateSessionViewModel>(session);
            return Result<UpdateSessionViewModel>.Ok(mappedSession);
        }

        public async Task<Result> UpdateSessionAsync(int id, UpdateSessionViewModel model, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(id, ct);
            if (session is null) return Result.NotFound("Session Not Found");

            if (session.StartDate <= DateTime.Now) return Result.Validation("Cannot Update Sesiso That Has Already Started");

            if (model.EndDate <= model.StartDate) return Result.Validation("EndDate Must be After StartDate");
            
            var bookingCount = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(id, ct);
            if (bookingCount > 0) return Result.Fail("Cannot Update Session That Has been Booked");

            if (model.StartDate <= DateTime.Now) return Result.Fail("Start Date Must Be in the Future");
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId);
            if (trainer is null) return Result.NotFound("Trainer Not Fount");
            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(session.CategoryId);

            var isValidSpeciality = Enum.TryParse<Speciality>(category?.CategoryName, true, out var CategorySpeciality);
            if (!isValidSpeciality || trainer.Speciality != CategorySpeciality) return Result.Validation("Cannot Session To This Trainer");

            _mapper.Map(model, session);
            session.UpdatedAt = DateTime.Now;
            _unitOfWork.SessionRepository.Update(session);
            var result = await _unitOfWork.SaveChangesAsync(ct);
             return result > 0 ? Result.Ok() : Result.Fail("Fail to Update Session");
        }

        public async Task<Result> RemoveSessionAsync(int sessionId, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(sessionId);
            if (session is null) return Result.NotFound("Sesison Not Found");
            if (session.EndDate >= DateTime.Now) return Result.Fail("Cannot Delete Ongoing Session");
            var bookingCount = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(sessionId, ct);
            if (bookingCount > 0) return Result.Fail("Cannot Delete Sessions That Has Bookings");

            _unitOfWork.SessionRepository.Delete(session);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Delete Session");
        }
    }
}



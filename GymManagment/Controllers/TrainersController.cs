using GymManagment.BLL.Services.Interfaces;
using GymManagment.BLL.ViewModel.TrainerViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagment.PL.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class TrainersController : Controller
    {
        private readonly ITrainerService _trainerService;
        public TrainersController(ITrainerService trainerService)
        {
            _trainerService = trainerService;
        }

        public async Task<IActionResult> Index()
            => View(await _trainerService.GetAllTrainersAsync());

        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var trainer = await _trainerService.GetTrainerDetailsByIdAsync(id, ct);
            if (trainer.Success)
                return View(trainer.Value);
            else
            {
                TempData["ErrorMessage"] = trainer.Error;
                return RedirectToAction(nameof(Index));
            }

        }


        #region Create Trainer
        [HttpGet]
        public async Task<IActionResult> Create()
            => View();

        [HttpPost]
        public async Task<IActionResult> Create(CreateTrainerViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model); //return user to the form again
            var result = await _trainerService.CreateTrainerAsync(model, ct);
            if (result.Success)
            {
                TempData["SuccessMessage"] = "Trainer created successfully.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = result.Error;
                return View(model);

            }
        }
        #endregion


        #region Edit Trainer
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var trainer = await _trainerService.GetTrainerToUpdateAsync(id, ct);
            if (trainer.Success)
                return View(trainer.Value);
            else
            {
                TempData["ErrorMessage"] = trainer.Error;
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, TrainerToUpdateViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _trainerService.UpdateTrainerDetailsAsync(id, model, ct);
            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Success ? "Trainer Updated successfully!" : result.Error;
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Delete Trainer
        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var result = await _trainerService.GetTrainerDetailsByIdAsync(id, ct);
            if (result.Success)
                return View();
            else
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromRoute] int id, CancellationToken ct)
        {
            var result = await _trainerService.RemoveTrainerAsync(id, ct);
            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Success ? "Trainer Deleted Succesfully!" : result.Error;
            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}

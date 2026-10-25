using GymManagment.BLL.Services.Classes;
using GymManagment.BLL.Services.Interfaces;
using GymManagment.BLL.ViewModel;
using GymManagment.BLL.ViewModel.TrainerViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GymManagment.PL.Controllers
{
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
            if(trainer is null)
            {
                TempData["ErrorMessage"] = "Trainer Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
                
        }


        #region Create Trainer
        [HttpGet]
        public async Task<IActionResult> Create()
            => View();

        [HttpPost]
        public async Task<IActionResult> Create(CreateTrainerViewModel model , CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model); //return user to the form again
            var result = await _trainerService.CreateTrainerAsync(model, ct);
            if(result)
            {
                TempData["SuccessMessage"] = "Trainer created successfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = "Failed to Add Trainer";
            return View(model);
        }
        #endregion


        #region Edit Trainer
        [HttpGet]
        public async Task<IActionResult> Edit(int id , CancellationToken ct)
        {
            var trainer = await _trainerService.GetTrainerToUpdateAsync(id, ct);
            if(trainer is null)
            {
                TempData["ErrorMessage"] = "Trainer Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id , TrainerToUpdateViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _trainerService.UpdateTrainerDetailsAsync(id, model, ct);
            if (result)
                TempData["SuccessMessage"] = "Trainer Updated successfully!";
            else
                TempData["ErrorMessage"] = "Failed to Update Trainer";

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Delete Trainer
        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var result = await _trainerService.GetTrainerDetailsByIdAsync(id, ct);
            if (result == null)
            {
                TempData["ErrorMessage"] = "User Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromRoute] int id, CancellationToken ct)
        {
            var result = await _trainerService.RemoveTrainerAsync(id, ct);
            if (!result)
                TempData["ErrorMessage"] = "Cannot Delete Trainer";
            else
                TempData["SuccessMessage"] = "Trainer Deleted Succesfully!";

            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}

using GymManagment.DAL.Data.Models;
using GymManagment.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymManagment.BLL.Services.Interfaces;
using GymManagment.BLL.ViewModel.PlanViewModels;
using Microsoft.AspNetCore.Authorization;

namespace GymManagment.PL.Controllers
{
    [Authorize]
    public class PlanController : Controller
    {

        private readonly IPlanService _planService;
        public PlanController(IPlanService planService)
        {
            _planService = planService;
        }

        // Index Action
        // baseUrl/Plan/Index
        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
            => View(await _planService.GetAllPlansAsync(ct));

        // Details Action
        // baseUrl/Plan/Details/{id}
        public async Task<IActionResult> Details(int id, CancellationToken ct = default)
        {
            var plan = await _planService.GetPlanDetailsByIdAsync(id, ct);
            if (plan.Success) // we check on null in case the user enter id thorugh the url not through the button
                return View(plan.Value);
            else
            {
                TempData["ErrorMessage"] = plan.Error;
                return RedirectToAction(nameof(Index));
            }
        }

        #region Edit
        // baseUrl/Plan/Edit/{id}
        // Edit Action - display the form 
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var result = await _planService.GetPlanToUpdateAsync(id, ct);
            if(result.Success)
                return View(result.Value);
            else
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction(nameof(Index));
            }

        }

        // Post baseUrl/Plan/Edit/{Id}
        // submit the form 
        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdatePlanViewModel model, CancellationToken ct)
        {
            var result = await _planService.UpdatePlanDetailsAsync(id, model, ct);
            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Success ? "Plan Updated Seccessfully!" : result.Error;
            return RedirectToAction(nameof(Index));
        }
        #endregion

        [HttpPost]
        public async Task<IActionResult> Activate(int id , CancellationToken ct)
        {
            var result = await _planService.ToggleActivationAsync(id, ct);
            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Success ? "Plan status changed!" : result.Error;

            //if (result.)
            //    TempData["SuccessMessage"] = "Plan status changed";
            //else
            //    TempData["ErrorMessage"] = "Failed to activate";
            return RedirectToAction(nameof(Index));
        }
    }
}

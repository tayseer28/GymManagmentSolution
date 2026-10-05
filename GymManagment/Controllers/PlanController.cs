using GymManagment.DAL.Data.Models;
using GymManagment.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagment.Controllers
{
    public class PlanController : Controller
    {
        private readonly IGenericRepository<Plan> _planRepository;
        public PlanController(IGenericRepository<Plan> planRepository)
        {
            this._planRepository = planRepository;
        }

        // Index Action
        // baseUrl/Plan/Index
        public async Task<IActionResult> Index(CancellationToken ct = default)
        {
            var plans = await _planRepository.GetAllAsync(ct: ct);
            return View(plans);
        }

        // Details Action
        // baseUrl/Plan/Details/{id}
        public async Task<IActionResult> Details(int id, CancellationToken ct = default)
        {
            var plan = await _planRepository.GetByIdAsync(id, ct); 
            if (plan == null) // we check on null in case the user enter id thorugh the url not through the button
            {
                //return NotFound();
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }
    }
}

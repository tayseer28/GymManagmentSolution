using GymManagment.DAL.Repositories.Interfaces;
using GymManagment.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagment.Controllers
{
    public class PlanController : Controller
    {
        private readonly IPlanRepository planRepository;
        public PlanController(IPlanRepository planRepository)
        {
            this.planRepository = planRepository;
        }

        // Index Action
        // baseUrl/Plan/Index
        public async Task<IActionResult> Index(CancellationToken ct = default)
        {
            var plans = await planRepository.GetAllPlansAsync(ct: ct);
            return View(plans);
        }

        // Details Action
        // baseUrl/Plan/Details/{id}
        public async Task<IActionResult> Details(int id, CancellationToken ct = default)
        {
            var plan = await planRepository.GetPlanByIdAsync(id, ct); // Find search locally first before sending request to the database 
            if (plan == null) // we check on null in case the user enter id thorugh the url not through the button
            {
                //return NotFound();
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }
    }
}

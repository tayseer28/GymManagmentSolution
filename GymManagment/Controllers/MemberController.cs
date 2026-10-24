using GymManagment.BLL.Services.Interfaces;
using GymManagment.BLL.ViewModel.MemberViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GymManagment.PL.Controllers
{
    public class MemberController : Controller
    {
        private readonly IMemberService _memberService;
        public MemberController(IMemberService memberService)
        {
            _memberService = memberService;
        }
        // GET BaseUrl/Member/Index
        //Index - List All members
        public async Task<IActionResult> Index(CancellationToken ct )
        {
            // Implementation to list all members
            var members = await _memberService.GetAllMembersAsync(ct);
            return View(members);
        }


        // GET BaseUrl/Member/MemberDetails/{id}
        // MemberDetails - Show details of a specific member by ID
        public async Task<IActionResult> MemberDetails(int id, CancellationToken ct)
        {
            // Implementation to show member details by ID
            var memberDetails=  await _memberService.GetMemberDetailsByIdAsync(id, ct);
            if(memberDetails is null)
            {
                TempData["ErrorMessage"] = "Member Fetch Failed";
                return RedirectToAction(nameof(Index));
            }
            return View(memberDetails);
        }

        // GET BaseUrl/Member/HealthRecordDetails/{id}
        // HealthRecordDetails - Show health records of a specific member by ID
        public async Task<IActionResult> HealthRecordDetails(int id, CancellationToken ct)
        {
            var result = await _memberService.GetMemberHealthRecordDetailsAsync(id, ct);
            if(result is null)
            {
                TempData["ErrorMessage"] = "Member Health Record Fetch Failed";
                return RedirectToAction(nameof(Index));
            }
            return View(result);
           
        } 

        #region Create Member

        // GET BaseUrl/Member/Create
        // Create - Show empty form 
        [HttpGet]
        public IActionResult Create() =>  View();

        // POST BaseUrl/Member/Create  {member}
        // CreateMember - submit the form 

        [HttpPost]
        public async Task<IActionResult> CreateMember(CreateMemberViewModel model, CancellationToken ct)
        {
            if(!ModelState.IsValid) return View(nameof(Create), model);
            var isCreated = await _memberService.CreateMemberAsync(model, ct);
            if(isCreated)
                TempData["SuccessMessage"] = "Member created successfully!";
            else
                TempData["ErrorMessage"] = "Failed to create member. Please try again.";
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Edit Member

        // GET BaseUrl/Member/Edit/{id}
        // MemberEdit - Show form with pre-filled data to edit
        [HttpGet]
        public async Task<IActionResult> EditMember(int id , CancellationToken ct)
        {
            var result = await _memberService.GetMemberToUpdateAsync(id, ct);
            if(result == null)
            {
                TempData["ErrorMessage"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(result);
        }

        // PUT BaseUrl/Member/Edit  {member}
        // MemberEdit - Submit form
        [HttpPost]
        public async Task<IActionResult> EditMember(int id , MemberToUpdateViewModel member, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(member);
            var result = await _memberService.UpdateMemberDetailsAsync(id, member, ct);
            if (result)
                TempData["SuccessMessage"] = "Member Updated successfully!";
            else
                TempData["ErrorMessage"] = "Failed to Update Member";

            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Delete

        // GET BaseUrl/Member/Delete/{id}
        // Delete - Show confirmation page for deletion

        // POST BaseUrl/Member/Delete/{id}
        // DeleteConfirmed - Delete after confirmation
        #endregion
    }
}

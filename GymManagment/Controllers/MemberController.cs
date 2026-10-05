using GymManagment.BLL.Services.Interfaces;
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

        // GET BaseUrl/Member/HealthRecordDetails/{id}
        // HealthRecordDetails - Show health records of a specific member by ID

        #region Create Member

        // GET BaseUrl/Member/Create
        // Create - Show empty form 

        // POST BaseUrl/Member/Create  {member}
        // CreateMember - submit the form 

        #endregion

        #region Edit Member

        // GET BaseUrl/Member/Edit/{id}
        // MemberEdit - Show form with pre-filled data to edit

        // PUT BaseUrl/Member/Edit  {member}
        // MemberEdit - Submit form

        #endregion

        #region Delete

        // GET BaseUrl/Member/Delete/{id}
        // Delete - Show confirmation page for deletion

        // POST BaseUrl/Member/Delete/{id}
        // DeleteConfirmed - Delete after confirmation
        #endregion
    }
}

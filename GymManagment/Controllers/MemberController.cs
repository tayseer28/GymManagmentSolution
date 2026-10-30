using GymManagementBLL.Common;
using GymManagment.BLL.Attachment;
using GymManagment.BLL.Services.Interfaces;
using GymManagment.BLL.ViewModel.MemberViewModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace GymManagment.PL.Controllers
{
    public class MemberController : Controller
    {
        private readonly IMemberService _memberService;
        private readonly IAttachmentService _attachmentService;

        public MemberController(IMemberService memberService,
            IAttachmentService attachmentService)
        {
            _memberService = memberService;
            _attachmentService = attachmentService;
        }
        // GET BaseUrl/Member/Index
        //Index - List All members
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            // Implementation to list all members
            var members = await _memberService.GetAllMembersAsync(ct);
            return View(members);
        }

        #region Get Member Photo
        [HttpGet]
        public async Task<IActionResult> Picture(int id)
        {
            var member = await _memberService.GetMemberDetailsByIdAsync(id);
            if (member.Value is null || string.IsNullOrEmpty(member.Value.Photo))
                return NotFound();

            var result = _attachmentService.GetFile(member.Value.Photo, "MembersPictures");
            if (result is null) return NotFound();

            return File(result.Value.Stream, result.Value.ContentType);
        }
        #endregion

        #region Member Details

        // GET BaseUrl/Member/MemberDetails/{id}
        // MemberDetails - Show details of a specific member by ID
        public async Task<IActionResult> MemberDetails(int id, CancellationToken ct)
        {
            // Implementation to show member details by ID
            var memberDetails = await _memberService.GetMemberDetailsByIdAsync(id, ct);

            if (memberDetails.Success)
            {
                return View(memberDetails.Value);
            }
            else
            {
                TempData["ErrorMessage"] = memberDetails.Error;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET BaseUrl/Member/HealthRecordDetails/{id}
        // HealthRecordDetails - Show health records of a specific member by ID
        public async Task<IActionResult> HealthRecordDetails(int id, CancellationToken ct)
        {
            var result = await _memberService.GetMemberHealthRecordDetailsAsync(id, ct);
            if (result.Success)
                return View(result);

            else
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction(nameof(Index));
            }
        }
        #endregion

        #region Create Member

        // GET BaseUrl/Member/Create
        // Create - Show empty form 
        [HttpGet]
        public IActionResult Create() => View();

        // POST BaseUrl/Member/Create  {member}
        // CreateMember - submit the form 

        [HttpPost]
        public async Task<IActionResult> CreateMember(CreateMemberViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(nameof(Create), model);

            var result = await _memberService.CreateMemberAsync(model, ct);
            if (result.Success)
                TempData["SuccessMessage"] = "Member created successfully.";
            else
                TempData["ErrorMessage"] = result.Error;

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Edit Member

        // GET BaseUrl/Member/Edit/{id}
        // MemberEdit - Show form with pre-filled data to edit
        [HttpGet]
        public async Task<IActionResult> EditMember(int id, CancellationToken ct)
        {
            var result = await _memberService.GetMemberToUpdateAsync(id, ct);
            if (result.Success)
                return View(result.Value);
            else
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction(nameof(Index));
            }
        }

        // PUT BaseUrl/Member/Edit  {member}
        // MemberEdit - Submit form
        [HttpPost]
        public async Task<IActionResult> EditMember([FromRoute] int id, MemberToUpdateViewModel member, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(member);
            var result = await _memberService.UpdateMemberDetailsAsync(id, member, ct);
            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Success ? "Member Updated successfully!" : result.Error;
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Delete

        // GET BaseUrl/Member/Delete/{id}
        // Delete - Show confirmation page for deletion
        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var result = await _memberService.GetMemberDetailsByIdAsync(id, ct);
            if (result.Success)
                return View();
            else
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST BaseUrl/Member/Delete/{id}
        // DeleteConfirmed - Delete after confirmation
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromRoute] int id, CancellationToken ct)
        {
            var result = await _memberService.RemoveMember(id, ct);
            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Success ? "Session Deleted!" : result.Error;
            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}

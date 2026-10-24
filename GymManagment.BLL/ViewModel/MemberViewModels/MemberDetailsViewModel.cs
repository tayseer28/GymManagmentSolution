namespace GymManagment.BLL.ViewModel.MemberViewModels
{
    public class MemberDetailsViewModel : MemberViewModel
    {
        // add remaining member details required 
        public string DateOfBirth { get; set; }
        public string Address { get; set; }
        public string PlanName { get; set; }
        public string MembershipStartDate { get; set; }
        public string MembershipEndtDate { get; set; }

    }
}
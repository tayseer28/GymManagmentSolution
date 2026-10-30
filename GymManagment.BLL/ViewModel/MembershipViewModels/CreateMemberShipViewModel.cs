using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagment.BLL.ViewModel.MembershipViewModels
{
    public class CreateMemberShipViewModel
    {
        public int PlanId { get; set; }
        public int MemberId { get; set; }
        public DateTime? StartDate { get; set; }
    }
}

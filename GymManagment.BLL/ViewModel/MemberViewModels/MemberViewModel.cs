using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagment.BLL.ViewModel.MemberViewModels
{
    public class MemberViewModel
    {
        public int Id { get; set; }
        public string? Photo { get; set; } = default!;
        public string Name { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
    }
}

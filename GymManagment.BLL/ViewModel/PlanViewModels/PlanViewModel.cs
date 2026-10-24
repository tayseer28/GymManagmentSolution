using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagment.BLL.ViewModel.PlanViewModels
{
    public class PlanViewModel
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
        public string Duration { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}

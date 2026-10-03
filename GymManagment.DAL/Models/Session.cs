using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagment.DAL.Models
{
    internal class Session : BaseEntity
    {
        public string Description { get; set; }
        public int Capacity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}

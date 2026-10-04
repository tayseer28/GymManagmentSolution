using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagment.DAL.Data.Models
{
    public class Membership : BaseEntity
    {
        #region Relationships
        public Member Member { get; set; }
        public int MemberId { get; set; }

        public Plan Plan { get; set; }
        public int PlanId { get; set; } 
        #endregion

        // StartDate is the CreatedAt of the BaseEntity so we will handle this in the configuration class
        public DateTime EndDate { get; set; }

        public string Status  => EndDate < DateTime.Now ? "Expired" : "Active"; // Computed property (will not mapped)
        public bool IsActive => EndDate > DateTime.Now; // computed property (will not mapped)
    }
}

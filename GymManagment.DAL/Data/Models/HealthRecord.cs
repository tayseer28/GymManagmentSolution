using GymManagment.DAL.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagment.DAL.Data.Models
{
    public class HealthRecord : BaseEntity
    {
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public BloodType BloodType { get; set; }
        public string? Note { get; set; }
        //LastUpdate is the UpdatedAt of the BaseEntity so we will handle this in the configuration class 

        #region Relationships
        public Member Member { get; set; } = default!; // Each HealthRecord belongs to one Member
        public int MemberId { get; set; } // FK 
        #endregion


    }
}

using GymManagment.DAL.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagment.DAL.Data.Models
{
    public class Trainer : GymUser
    {
        public Speciality Speciality { get; set; }
        // HireDate is the CreatedAt of the BaseEntity so we will handle this in the configuration class

        #region Relationships
        public ICollection<Session> Sessions { get; set; } = default!; // each trainer can conduct to many sessions

        #endregion 
    }
}

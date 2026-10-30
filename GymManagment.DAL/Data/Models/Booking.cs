using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagment.DAL.Data.Models
{
    public class Booking : BaseEntity
    {
        #region Relationship
        public Member Member { get; set; }
        public int MemberId { get; set; }

        public Session Session { get; set; }
        public int SessionId { get; set; } 
        #endregion

        // BookingDate is the CreatedAt of the BaseEntity so we will handle this in the configuration class
        public bool IsAttended { get; set; }
    }
}

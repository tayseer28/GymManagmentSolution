using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagment.DAL.Data.Models
{
    public class Member : GymUser
    {
        public string? Photo { get; set; }
        // JoinDate is the CreatedAt of the BaseEntity so we will handle this in the configuration class

        #region Relationships
        public HealthRecord HealthRecord { get; set; } = default!; // Each Member has one HealthRecord

        public ICollection<Booking> Bookings { get; set; } = default!; // each member has many bookings 

        public ICollection<Membership> Memberships { get; set; } = default!; // each member can subscribe many plans

        #endregion

    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagment.DAL.Data.Models
{
    public class Session : BaseEntity
    {
        public string Description { get; set; }
        public int Capacity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        #region Relationships
        public Trainer Trainer { get; set; } = default!; // each session has one trainer
        public int TrainerId { get; set; } // FK 

        public Category Category { get; set; } = default!; // each session belongs to one category
        public int CategoryId { get; set; } // FK

        public ICollection<Booking> SessionMembers { get; set; } = default!; // each session has many books 
        #endregion
    }
}

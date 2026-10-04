using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagment.DAL.Data.Models
{
    public class Category : BaseEntity
    {
        public string CategoryName { get; set; }

        #region Relationships
        public ICollection<Session> Sessions { get; set; } = default!;  // each category can have many sessions
        #endregion

    }
}

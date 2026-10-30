namespace GymManagment.DAL.Data.Models
{
    public class Plan : BaseEntity
    {
       
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int DurationDays { get; set; }
        public bool IsActive { get; set; }

        #region Relationships
        public ICollection<Membership> PlanMemberships { get; set; } = default!; // each Plan can have many members 
        #endregion


    }
}

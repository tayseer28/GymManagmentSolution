using GymManagment.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagment.Configurations
{
    public class PlanConfiguration : IEntityTypeConfiguration<Plan>
    {
        public void Configure(EntityTypeBuilder<Plan> builder)
        {
            builder.Property(p => p.Name)
                    .HasColumnType("varchar") // we can make it "varchar(50)" but the ef core will not know the length of that prop
                    .HasMaxLength(50);        // only the database knows it , but by using hasMaxLength() then both(database, ef core) know it is varchar(50)


            builder.Property(p => p.Description)
                    .HasMaxLength(200);

            builder.Property(p => p.Price)
                    .HasPrecision(10, 2);

            builder.Property(p => p.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");

            builder.ToTable(TB =>
            {
                TB.HasCheckConstraint("PlanDurationCheck", "DurationDays between 1 and 365");
            });
        }
    }
}

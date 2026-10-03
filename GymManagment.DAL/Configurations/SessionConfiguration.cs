using GymManagment.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagment.DAL.Configurations
{
    internal class SessionConfiguration : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            //builder.Property(s => s.CreatedAt)
            //        .HasDefaultValueSql("GETDATE()");
            builder.ToTable(tb =>
            {
                tb.HasCheckConstraint("CapacityCheck", "Capacity between 1 and 25");
                tb.HasCheckConstraint("DateCheck", "StartDate < EndDate");
            });
        }
    }
}

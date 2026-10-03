using GymManagment.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace GymManagment.DAL.Configurations
{
    internal class MemberConfiguration : GymUserConfiguration<Member> , IEntityTypeConfiguration<Member>
    {
        public new void Configure(EntityTypeBuilder<Member> builder)
        {
            base.Configure(builder);
            builder.Property(m => m.CreatedAt)
                    .HasColumnName("JoinDate")
                    .HasDefaultValueSql("GETDATE()");
            
        }
    }
}

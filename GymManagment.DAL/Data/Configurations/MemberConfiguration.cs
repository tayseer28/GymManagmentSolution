using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GymManagment.DAL.Data.Models;


namespace GymManagment.DAL.Data.Configurations
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

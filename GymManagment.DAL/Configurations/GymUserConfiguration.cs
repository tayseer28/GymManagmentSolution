using GymManagment.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagment.DAL.Configurations
{
    public class GymUserConfiguration<T> : IEntityTypeConfiguration<T> where T : GymUser
    {
        public void Configure(EntityTypeBuilder<T> builder)
        {
            builder.Property(gu => gu.Name)
                    .HasColumnType("varchar")
                    .HasMaxLength(50);

            builder.Property(gu => gu.Email)
                    .HasColumnType("varchar")
                    .HasMaxLength(100);

            builder.HasIndex(gu => gu.Email)
                    .IsUnique();

            builder.Property(gu => gu.Phone)
                    .HasColumnType("varchar")
                    .HasMaxLength(11);

            builder.HasIndex(gu => gu.Phone)
                    .IsUnique();

            builder.OwnsOne(gu => gu.Address, AddressBuilder =>
            {
                AddressBuilder.Property(a => a.Street)
                              .HasColumnType("varchar")
                              .HasColumnName("Street")
                              .HasMaxLength(30);

                AddressBuilder.Property(a => a.City)
                              .HasColumnType("varchar")
                              .HasColumnName("City")
                              .HasMaxLength(30);
            });

            builder.ToTable(b =>
            {
                // Simple email format check: must contain an '@' with at least one character before it and a '.' after the '@'
                // This is intentionally simple; full RFC email regex is not suitable for a SQL check constraint.
                b.HasCheckConstraint("GymUserEmailFormatCheck",
                    """
                    Email IS NULL OR 
                    (CHARINDEX('@', Email) > 1 AND CHARINDEX('.', Email, CHARINDEX('@', Email) + 2) > CHARINDEX('@', Email) + 1 AND 
                    Email NOT LIKE '% %' AND LEN(Email) <= 254)
                    """
                    );

                // Ensure phone is either null or an Egyptian mobile number: 11 digits, starts with 010/011/012/015 and contains only digits
                b.HasCheckConstraint("CK_GymUser_Phone_Format",
                    "Phone IS NULL OR (LEN(Phone) = 11 AND Phone LIKE '01[0-5]%' AND Phone NOT LIKE '%[^0-9]%')");
            });


        }
    }
}

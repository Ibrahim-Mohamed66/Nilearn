using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nilearn.Domain.Entities;

namespace Nilearn.Infrastructure.Configurations;

public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.ProfileImageUrl)
            .HasMaxLength(200);

        builder.HasOne(u => u.StudentProfile)
            .WithOne(s => s.AppUser!)
            .HasForeignKey<Student>(s => s.AppUserId);

        builder.HasOne(u => u.InstructorProfile)
            .WithOne(i => i.User!)
            .HasForeignKey<Instructor>(i => i.AppUserId);
    }
}

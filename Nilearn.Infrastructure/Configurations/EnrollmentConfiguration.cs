using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nilearn.Domain.Entities;
using Nilearn.Domain.Enums;

namespace Nilearn.Infrastructure.Configurations
{
    internal class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            builder.HasKey(e => e.Id);


            builder.HasOne(e => e.Student)
               .WithMany(s => s.Enrollments)
               .HasForeignKey(e => e.StudentId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Course)
                   .WithMany(c => c.Enrollments)
                   .HasForeignKey(e => e.CourseId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.Payments)
                   .WithOne(p => p.Enrollment)
                   .HasForeignKey(p => p.EnrollmentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(e => new { e.StudentId, e.CourseId })
                   .IsUnique();
                  

            builder.HasIndex(e => e.StudentId);
            builder.HasIndex(e => e.CourseId);
            builder.HasIndex(e => e.Status);

            
            builder.Property(e => e.Status)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

           

        }
    }
}
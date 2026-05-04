using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nilearn.Domain.Entities;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        // Table name
        builder.ToTable("Reviews");

        // Properties
        builder.Property(r => r.Rating)
            .IsRequired();

        builder.Property(r => r.Comment)
            .HasMaxLength(1000)
            .IsRequired();  

        builder.HasOne(r => r.Course)
            .WithMany(c => c.Reviews)
            .HasForeignKey(r => r.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Student)
            .WithMany(s => s.Reviews)
            .HasForeignKey(r => r.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        // ⭐ Important business rule:
        // One review per student per course
        builder.HasIndex(r => new { r.CourseId, r.StudentId })
            .IsUnique();

        // Index for performance (course reviews queries)
        builder.HasIndex(r => r.CourseId);
        builder.HasIndex(r => r.StudentId);
    }
}
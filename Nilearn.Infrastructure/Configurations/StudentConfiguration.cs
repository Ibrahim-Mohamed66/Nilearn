using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nilearn.Domain.Entities;

namespace Nilearn.Infrastructure.Configuration
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.StudentNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(s => s.StudentNumber)
                .IsUnique();

            builder.Property(s => s.CurrentLevel)
                .IsRequired();

            ;
        }
    }
}
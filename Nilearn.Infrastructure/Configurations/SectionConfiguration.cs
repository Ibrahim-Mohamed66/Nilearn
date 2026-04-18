using Microsoft.EntityFrameworkCore;
using Nilearn.Domain.Entities;
using System.Security.Cryptography.X509Certificates;

namespace Nilearn.Infrastructure.Configurations
{
    internal class SectionConfiguration : IEntityTypeConfiguration<Section>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Section> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Title).IsRequired().HasMaxLength(200);
            builder.Property(s => s.Description).HasMaxLength(1000);
            builder.Property(s => s.Order).IsRequired();
            builder.HasOne(s => s.Course)
                   .WithMany(c => c.Sections)
                   .HasForeignKey(s => s.CourseId)
                    .OnDelete(DeleteBehavior.Restrict);
          
        }
    }
}
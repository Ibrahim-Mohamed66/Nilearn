using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nilearn.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nilearn.Infrastructure.Configurations
{
    internal class LessonConfiguration : IEntityTypeConfiguration<Lesson>
    {
        public void Configure(EntityTypeBuilder<Lesson> builder)
        {
            builder.HasKey(l => l.Id);

            // Properties
            builder.Property(l => l.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(l => l.Description)
                .HasMaxLength(1000);

            builder.Property(l => l.Order)
                .IsRequired();

            builder.Property(l => l.SectionId)
                .IsRequired();

            builder.Property(l => l.CloudinaryPublicId)
                .HasMaxLength(500);

            
            builder.Property(l => l.Format)
                .HasMaxLength(50);

            builder.Property(l => l.Bytes);

            builder.Property(l => l.DurationInSeconds);

            builder.Property(l => l.Content)
                 .HasMaxLength(1000);

            // Relationship
            builder.HasOne(l => l.Section)
                .WithMany(s => s.Lessons)
                .HasForeignKey(l => l.SectionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nilearn.Domain.Entities;

namespace Nilearn.Infrastructure.Configuration
{
    public class InstructorConfiguration : IEntityTypeConfiguration<Instructor>
    {
        public void Configure(EntityTypeBuilder<Instructor> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.Bio)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(i => i.Headline)
                .HasMaxLength(200);

            builder.Property(i => i.WebsiteUrl)
                .HasMaxLength(200);


           
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nilearn.Domain.Entities;

namespace Nilearn.Infrastructure.Configurations
{
    internal class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            
            builder.HasKey(p => p.Id);

            builder.HasOne(p => p.Enrollment)
                   .WithMany(e => e.Payments)
                   .HasForeignKey(p => p.EnrollmentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => p.EnrollmentId);

            
            builder.Property(p => p.Amount)
                   .HasPrecision(18, 2)
                   .IsRequired();

            

            builder.Property(p => p.Currency)
               .HasConversion<string>()
               .HasMaxLength(3)
               .IsRequired();

            
            builder.Property(p => p.Status)
                   .IsRequired();

           
            builder.Property(p => p.PaymobTransactionId)
                   .HasMaxLength(255);

            builder.Property(p => p.PaymobOrderId)
                   .HasMaxLength(255);

            builder.Property(p => p.PaymobIntentionId)
                   .HasMaxLength(255);

           
            builder.Property(p => p.MerchantReferenceId)
                   .HasMaxLength(150)
                   .IsRequired();

            builder.HasIndex(p => p.MerchantReferenceId)
                   .IsUnique();

          
            
            builder.Property(p => p.Version)
                   .IsRowVersion()
                   .IsConcurrencyToken();

          
            builder.HasIndex(p => p.PaymobTransactionId)
                   .IsUnique()
                   .HasFilter("\"PaymobTransactionId\" IS NOT NULL");

            builder.HasIndex(p => p.PaymobOrderId);

            builder.HasIndex(p => p.PaymobIntentionId);

            builder.HasIndex(p => p.Status);

        }
    }
}
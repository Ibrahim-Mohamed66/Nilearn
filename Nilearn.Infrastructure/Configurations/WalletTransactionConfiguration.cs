using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nilearn.Domain.Entities;
namespace Nilearn.Infrastructure.Configurations;

internal class WalletTransactionConfiguration : IEntityTypeConfiguration<WalletTransaction>
{
    public void Configure(EntityTypeBuilder<WalletTransaction> builder)
    {
        builder.HasKey(w => w.Id);

        builder.Property(w => w.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(w => w.Description)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(w => w.CreatedAt)
            .IsRequired();

        builder.Property(w => w.Type)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(w => w.PaymentId)
            .IsRequired();

        builder.HasIndex(w => new { w.PaymentId, w.InstructorId })
              .IsUnique();


        builder.HasIndex(w => w.PaymentId);

        



        builder.HasOne(w => w.Payment)
            .WithMany(p => p.Transactions)
            .HasForeignKey(w => w.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

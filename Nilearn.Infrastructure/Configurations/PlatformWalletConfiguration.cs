using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nilearn.Domain.Entities;
namespace Nilearn.Infrastructure.Configurations;

internal class PlatformWalletConfiguration : IEntityTypeConfiguration<PlatformWallet>
{
    public void Configure(EntityTypeBuilder<PlatformWallet> builder)
    {

        builder.ToTable("PlatformWallets");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Balance)
            .HasPrecision(18, 2)
            .IsRequired();
    }
}

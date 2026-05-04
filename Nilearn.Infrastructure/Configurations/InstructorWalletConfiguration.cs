using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nilearn.Domain.Entities;

namespace Nilearn.Infrastructure.Configurations;

internal class InstructorWalletConfiguration : IEntityTypeConfiguration<InstructorWallet>
{
    public void Configure(EntityTypeBuilder<InstructorWallet> builder)
    {

        builder.ToTable("InstructorWallets");
        builder.HasKey(ins => ins.Id);

        builder.HasIndex(ins => ins.InstructorId)
            .IsUnique();

        builder.Property(ins => ins.Balance)
            .HasPrecision(18,2)
            .IsRequired();
    }
}

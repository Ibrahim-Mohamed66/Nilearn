namespace Nilearn.Domain.Entities;

public class PlatformWallet : BaseEntity
{
    public decimal Balance { get; set; } = 0;
    public void Credit(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be positive.", nameof(amount));
        Balance += amount;
    }
    public void Debit(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be positive.", nameof(amount));
        if (Balance < amount)
            throw new InvalidOperationException("Insufficient balance.");
        Balance -= amount;
    }
}

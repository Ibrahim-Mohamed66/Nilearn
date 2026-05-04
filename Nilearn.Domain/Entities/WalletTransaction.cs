using Nilearn.Domain.Enums;
namespace Nilearn.Domain.Entities;

public class WalletTransaction
{
    public Guid Id { get; set; }

    public int? InstructorId { get; set; }
    public Instructor? Instructor { get; set; } 

    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }

    public int PaymentId { get; set; } 
    public Payment? Payment { get; private set; } 

    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }

    public void Credit(int? instructorId, decimal amount, string description, int paymentId)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be positive.", nameof(amount));

        InstructorId = instructorId;
        Amount = amount;
        Type = TransactionType.Credit;
        Description = description;
        PaymentId = paymentId;
        CreatedAt = DateTime.UtcNow;
    }
}

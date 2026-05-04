using System;
using System.Collections.Generic;
using System.Text;

namespace Nilearn.Domain.Entities;

public class InstructorWallet : BaseEntity
{
    public int InstructorId { get; set; }

    public decimal Balance { get; set; } = 0;

    public Instructor? Instructor { get; private set; }

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

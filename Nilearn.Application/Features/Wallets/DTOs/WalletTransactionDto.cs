using Nilearn.Domain.Enums;

namespace Nilearn.Application.Features.Wallets.DTOs;

public class WalletTransactionDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? CourseTitle { get; set; }
    public int? PaymentId { get; set; }
}

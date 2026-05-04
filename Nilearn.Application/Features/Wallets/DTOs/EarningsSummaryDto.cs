namespace Nilearn.Application.Features.Wallets.DTOs;

public class EarningsSummaryDto
{
    public decimal TotalEarnings { get; set; }
    public decimal CurrentBalance { get; set; }
    public decimal ThisMonthEarnings { get; set; }
    public int TotalTransactions { get; set; }
}

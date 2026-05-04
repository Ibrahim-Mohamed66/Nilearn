namespace Nilearn.Application.Features.Wallets.DTOs;

public class PlatformRevenueDto
{
    public decimal TotalRevenue { get; set; }
    public decimal CurrentBalance { get; set; }
    public decimal TotalInstructorPayouts { get; set; }
    public int TotalTransactions { get; set; }
}

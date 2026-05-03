using Nilearn.Domain.Enums;

namespace Nilearn.Application.Features.Payments.DTOs;

public class PaymentDto
{
    public int PaymentId { get; set; }
    public int EnrollmentId { get; set; }
    public decimal Amount { get; set; }
    public Currency Currency { get; set; }
    public PaymentStatus Status { get; set; }
    public string TransactionId { get; set; } 
    public DateTime? PaidAt { get; set; }

    // Course info
    public string? CourseTitle { get; set; }
    public string? CourseThumbnailUrl { get; set; }
}

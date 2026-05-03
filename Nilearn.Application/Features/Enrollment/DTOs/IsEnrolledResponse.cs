namespace Nilearn.Application.Features.Enrollment.DTOs;

public class IsEnrolledResponse
{
    public bool Enrolled { get; set; }
    public string? Status { get; set; }
    public int? EnrollmentId { get; set; }
}

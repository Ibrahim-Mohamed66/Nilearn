using Nilearn.Domain.Enums;

namespace Nilearn.Application.Features.Enrollment.DTOs;

public class EnrollmentDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public EnrollmentStatus Status { get; set; }
    public DateTime? ActivatedAt { get; set; }
    public DateTime CreatedAt { get; set; }

    // Student info (populated for course enrollments)
    public string? StudentName { get; set; }
    public string? StudentEmail { get; set; }

    // Course info (populated for student enrollments)
    public string? CourseTitle { get; set; }
    public string? CourseThumbnailUrl { get; set; }
    public string? InstructorName { get; set; }
}

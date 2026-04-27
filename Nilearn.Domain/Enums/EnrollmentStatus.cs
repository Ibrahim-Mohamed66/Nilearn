namespace Nilearn.Domain.Enums;

public enum EnrollmentStatus
{
    Pending,    // Payment initiated, awaiting completion
    Active,     // Paid and enrolled
    Cancelled   // User/admin cancelled
}

using Nilearn.Domain.Enums;

namespace Nilearn.Domain.Entities;
public class Enrollment : BaseEntity
{
    public int StudentId { get; private set; }
    public int CourseId { get; private set; }

    public EnrollmentStatus Status { get; private set; } = EnrollmentStatus.Pending;
    public DateTime? ActivatedAt { get; private set; }  // Set when Status → Active
                                               
    public Student? Student { get; private set; }
    public Course? Course { get; private set; }
   
    public ICollection<Payment> Payments { get; private set; } = new List<Payment>();

    public Enrollment(int studentId, int courseId)
    {
        StudentId = studentId;
        CourseId = courseId;
    }

    protected Enrollment() { } // For EF Core
    public void Activate()
    {
        if (IsActive)
            return;

        if(IsCancelled)
            throw new InvalidOperationException("Cannot activate a cancelled enrollment.");

        Status = EnrollmentStatus.Active;
        ActivatedAt = DateTime.UtcNow;

        AddDomainEvent(new Events.EnrollmentActivatedEvent(Id, StudentId, CourseId));
    }

    public void Cancel()
    {
        if (IsCancelled)
            return;
        if (IsActive)
            throw new InvalidOperationException("Cannot cancel an active enrollment directly. Process a refund first.");

        Status = EnrollmentStatus.Cancelled;
    }
    public void Reactivate()
    {
        if (Status != EnrollmentStatus.Cancelled)
            throw new InvalidOperationException("Only cancelled enrollments can be reactivated.");

        Status = EnrollmentStatus.Pending;
        ActivatedAt = null;
    }
    public bool IsActive => Status == EnrollmentStatus.Active;
    public bool IsPending => Status == EnrollmentStatus.Pending;
    public bool IsCancelled => Status == EnrollmentStatus.Cancelled;

}

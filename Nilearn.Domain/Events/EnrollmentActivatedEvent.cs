using Nilearn.Domain.Interfaces;

namespace Nilearn.Domain.Events;

public record EnrollmentActivatedEvent(int EnrollmentId, int StudentId, int CourseId) : IDomainEvent;

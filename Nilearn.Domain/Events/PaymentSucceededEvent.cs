using Nilearn.Domain.Interfaces;
namespace Nilearn.Domain.Events;

public record PaymentSucceededEvent(int PaymentId) : IDomainEvent;
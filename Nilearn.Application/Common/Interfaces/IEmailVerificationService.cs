using Nilearn.Application.Common.Enums;
using Nilearn.Domain.Entities;

namespace Nilearn.Application.Common.Interfaces
{
    public interface IEmailVerificationService
    {
        Task SendVerificationEmailAsync(AppUser user,CancellationToken cancellationToken);
        Task<EmailConfirmationResult>  ConfirmEmailAsync(string userId, string token,CancellationToken cancellationToken);
    }
}
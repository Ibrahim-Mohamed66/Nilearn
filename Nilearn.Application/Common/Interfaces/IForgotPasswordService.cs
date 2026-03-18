using Nilearn.Application.Common.Enums;
using Nilearn.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nilearn.Application.Common.Interfaces
{
    internal interface IForgotPasswordService
    {
        Task SendResetPasswordEmailAsync(AppUser user, CancellationToken cancellationToken);
        Task<ResetPasswordResult> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken);
    }
}

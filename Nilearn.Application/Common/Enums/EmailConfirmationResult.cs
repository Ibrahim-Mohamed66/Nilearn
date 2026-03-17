using System;
using System.Collections.Generic;
using System.Text;

namespace Nilearn.Application.Common.Enums
{
    public enum EmailConfirmationResult
    {
        Success,
        UserNotFound,
        InvalidToken,
        AlreadyConfirmed
    }
}

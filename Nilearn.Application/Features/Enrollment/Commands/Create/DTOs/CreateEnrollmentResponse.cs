using System;
using System.Collections.Generic;
using System.Text;

namespace Nilearn.Application.Features.Enrollment.Commands.Create.DTOs
{
    public record CreateEnrollmentResponse(bool RequiresPayment, string? PaymentUrl);

}

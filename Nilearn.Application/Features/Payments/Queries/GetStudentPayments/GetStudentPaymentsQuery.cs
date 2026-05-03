using MediatR;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Payments.DTOs;
using Nilearn.Domain.Enums;
using Nilearn.Shared.Models;

namespace Nilearn.Application.Features.Payments.Queries.GetStudentPayments;

public sealed record GetStudentPaymentsQuery(
    string UserId,
    int PageNumber,
    int PageSize,
    PaymentStatus? Status = null) : IRequest<Result<PagedResponse<PaymentDto>>>;

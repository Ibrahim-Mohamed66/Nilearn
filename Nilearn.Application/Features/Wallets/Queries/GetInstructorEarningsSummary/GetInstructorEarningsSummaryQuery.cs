using MediatR;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Wallets.DTOs;

namespace Nilearn.Application.Features.Wallets.Queries.GetInstructorEarningsSummary;

public record GetInstructorEarningsSummaryQuery(string UserId) : IRequest<Result<EarningsSummaryDto>>;

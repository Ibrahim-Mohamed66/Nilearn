using MediatR;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Wallets.DTOs;

namespace Nilearn.Application.Features.Wallets.Queries.GetPlatformRevenue;

public record GetPlatformRevenueQuery() : IRequest<Result<PlatformRevenueDto>>;

using MediatR;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Wallets.DTOs;

namespace Nilearn.Application.Features.Wallets.Queries.GetInstructorWallet;

public record GetInstructorWalletQuery(string UserId) : IRequest<Result<InstructorWalletDto>>;

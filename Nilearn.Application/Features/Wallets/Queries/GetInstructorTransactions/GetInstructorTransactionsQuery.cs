using MediatR;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Wallets.DTOs;
using Nilearn.Shared.Models;

namespace Nilearn.Application.Features.Wallets.Queries.GetInstructorTransactions;

public record GetInstructorTransactionsQuery(
    string UserId,
    int PageNumber = 1,
    int PageSize = 10,
    DateTime? StartDate = null,
    DateTime? EndDate = null) : IRequest<Result<PagedResponse<WalletTransactionDto>>>;

using MediatR;
using Microsoft.EntityFrameworkCore;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Wallets.DTOs;
using Nilearn.Domain.Enums;
using Nilearn.Domain.Interfaces;

namespace Nilearn.Application.Features.Wallets.Queries.GetPlatformRevenue;

public class GetPlatformRevenueQueryHandler : IRequestHandler<GetPlatformRevenueQuery, Result<PlatformRevenueDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPlatformRevenueQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PlatformRevenueDto>> Handle(GetPlatformRevenueQuery request, CancellationToken cancellationToken)
    {
        var platformWallet = await _unitOfWork.PlatformWalletRepository.GetAsync(cancellationToken);
        if (platformWallet == null)
        {
            var platformRevenueDto = new PlatformRevenueDto
            {
                TotalRevenue = 0m,
                CurrentBalance = 0m,
                TotalInstructorPayouts = 0m,
                TotalTransactions = 0
            };
            return Result<PlatformRevenueDto>.SuccessResponse(platformRevenueDto, message: "No platform revenue found.");
        }

        var transactionsQuery = _unitOfWork.WalletTransactionRepository.QueryAll();

        // Total revenue is the sum of all credits to the platform (InstructorId is null)
        var totalRevenue = await transactionsQuery
            .Where(t => t.Type == TransactionType.Credit && t.InstructorId == null)
            .SumAsync(t => t.Amount, cancellationToken);

        // Total instructor payouts (total earnings credited to instructors)
        var totalInstructorPayouts = await transactionsQuery
            .Where(t => t.Type == TransactionType.Credit && t.InstructorId != null)
            .SumAsync(t => t.Amount, cancellationToken);

        var totalTransactions = await transactionsQuery.CountAsync(cancellationToken);

        return Result<PlatformRevenueDto>.SuccessResponse(new PlatformRevenueDto
        {
            TotalRevenue = totalRevenue,
            CurrentBalance = platformWallet.Balance,
            TotalInstructorPayouts = totalInstructorPayouts,
            TotalTransactions = totalTransactions
        }, "Platform revenue fetched successfully");
    }
}

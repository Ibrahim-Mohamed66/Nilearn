using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Wallets.DTOs;
using Nilearn.Domain.Enums;
using Nilearn.Domain.Interfaces;

namespace Nilearn.Application.Features.Wallets.Queries.GetInstructorEarningsSummary;

public class GetInstructorEarningsSummaryQueryHandler : IRequestHandler<GetInstructorEarningsSummaryQuery, Result<EarningsSummaryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetInstructorEarningsSummaryQueryHandler> _logger;

    public GetInstructorEarningsSummaryQueryHandler(IUnitOfWork unitOfWork, ILogger<GetInstructorEarningsSummaryQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<EarningsSummaryDto>> Handle(GetInstructorEarningsSummaryQuery request, CancellationToken cancellationToken)
    {
        var instructorId = await _unitOfWork.InstructorRepository.GetIdByUserIdAsync(request.UserId, cancellationToken);
        if(instructorId is null)
        {
            _logger.LogInformation("Instructor not found for user ID: {UserId}", request.UserId);
            return Result<EarningsSummaryDto>.FailureResponse(message: "Instructor not found.");
        }
        var wallet = await _unitOfWork.InstructorWalletRepository.GetByInstructorIdAsync(instructorId.Value, cancellationToken);
        if (wallet == null)
        {
            var newEarningSummary = new EarningsSummaryDto
            {
                TotalEarnings = 0m,
                CurrentBalance = 0m,
                ThisMonthEarnings = 0m,
                TotalTransactions = 0
            };
            return Result<EarningsSummaryDto>.SuccessResponse(newEarningSummary, message: "No earnings found.");
        }

        var firstDayOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

        var transactionsQuery = _unitOfWork.WalletTransactionRepository.QueryByInstructorId(instructorId.Value);

        var totalEarnings = await transactionsQuery
            .Where(t => t.Type == TransactionType.Credit)
            .SumAsync(t => t.Amount, cancellationToken);

        var thisMonthEarnings = await transactionsQuery
            .Where(t => t.Type == TransactionType.Credit && t.CreatedAt >= firstDayOfMonth)
            .SumAsync(t => t.Amount, cancellationToken);

        var totalTransactions = await transactionsQuery.CountAsync(cancellationToken);
        _logger.LogInformation("Earnings summary calculated for instructor ID: {InstructorId} - Total Earnings: {TotalEarnings}, " +
            "This Month Earnings: {ThisMonthEarnings}, Total Transactions: {TotalTransactions}",
                instructorId, totalEarnings, thisMonthEarnings, totalTransactions);
        return Result<EarningsSummaryDto>.SuccessResponse(new EarningsSummaryDto
        {
            TotalEarnings = totalEarnings,
            CurrentBalance = wallet.Balance,
            ThisMonthEarnings = thisMonthEarnings,
            TotalTransactions = totalTransactions
        }, "Earnings summary fetched successfully");
    }
}

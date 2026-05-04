using MediatR;
using System.Linq;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Extensions;
using Nilearn.Application.Features.Wallets.DTOs;
using Nilearn.Domain.Interfaces;
using Nilearn.Shared.Models;
using Microsoft.Extensions.Logging;

namespace Nilearn.Application.Features.Wallets.Queries.GetInstructorTransactions;

public class GetInstructorTransactionsQueryHandler : IRequestHandler<GetInstructorTransactionsQuery, Result<PagedResponse<WalletTransactionDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetInstructorTransactionsQueryHandler> _logger;

    public GetInstructorTransactionsQueryHandler(IUnitOfWork unitOfWork, ILogger<GetInstructorTransactionsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<PagedResponse<WalletTransactionDto>>> Handle(GetInstructorTransactionsQuery request, CancellationToken cancellationToken)
    {
        var instructorId = await _unitOfWork.InstructorRepository.GetIdByUserIdAsync(request.UserId, cancellationToken);
        if (instructorId is null)
        {
            _logger.LogInformation("Instructor not found for user ID: {UserId}", request.UserId);
            return Result<PagedResponse<WalletTransactionDto>>.FailureResponse(message: "Instructor not found.");
        }
        var query = _unitOfWork.WalletTransactionRepository.QueryByInstructorId(instructorId.Value, request.StartDate, request.EndDate);

        var projectedQuery = query.Select(t => new WalletTransactionDto
        {
            Id = t.Id,
            Amount = t.Amount,
            Type = t.Type,
            Description = t.Description,
            CreatedAt = t.CreatedAt,
            PaymentId = t.PaymentId,
            CourseTitle = t.Payment != null && t.Payment.Enrollment != null && t.Payment.Enrollment.Course != null 
                ? t.Payment.Enrollment.Course.Title 
                : null
        });

        var pagedTransactions = await projectedQuery.ToPagedAsync(request.PageNumber, request.PageSize, cancellationToken);
        _logger.LogInformation("Fetched {Count} transactions for instructor ID: {InstructorId}", pagedTransactions.Items.Count, instructorId);

        return Result<PagedResponse<WalletTransactionDto>>.SuccessResponse(pagedTransactions, "Instructor transactions fetched successfully");
    }
}

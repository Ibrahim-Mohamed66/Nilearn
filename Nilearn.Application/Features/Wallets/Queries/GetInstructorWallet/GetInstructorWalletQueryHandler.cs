using MediatR;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Wallets.DTOs;
using Nilearn.Domain.Interfaces;

namespace Nilearn.Application.Features.Wallets.Queries.GetInstructorWallet;

public class GetInstructorWalletQueryHandler : IRequestHandler<GetInstructorWalletQuery, Result<InstructorWalletDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetInstructorWalletQueryHandler> _logger;
    public GetInstructorWalletQueryHandler(IUnitOfWork unitOfWork, ILogger<GetInstructorWalletQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<InstructorWalletDto>> Handle(GetInstructorWalletQuery request, CancellationToken cancellationToken)
    {
        var instructorId = await _unitOfWork.InstructorRepository.GetIdByUserIdAsync(request.UserId, cancellationToken);
        if (instructorId is null)
        {
            _logger.LogInformation("Instructor not found for user ID: {UserId}", request.UserId);
            return Result<InstructorWalletDto>.FailureResponse(message: "Instructor not found.");
        }
        var wallet = await _unitOfWork.InstructorWalletRepository.GetByInstructorIdAsync(instructorId.Value, cancellationToken);

        if (wallet == null)
        {
            return Result<InstructorWalletDto>.SuccessResponse(
                new InstructorWalletDto
                {
                    InstructorId = instructorId.Value,
                    Balance = 0m
                },
                "Wallet not found (no earnings yet)."
            );
        }

        return Result<InstructorWalletDto>.SuccessResponse(new InstructorWalletDto
        {
            InstructorId = wallet.InstructorId,
            Balance = wallet.Balance
        }, "Wallet fetched successfully");
    }
}

using MediatR;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Domain.Interfaces;


namespace Nilearn.Application.Features.Auth.Logout.Commands;

public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result<string>>
{
    private readonly ILogger<LogoutCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    public LogoutCommandHandler(ILogger<LogoutCommandHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<string>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
       
        await _unitOfWork.RefreshTokenRepository.RevokeAsync(request.token,cancellationToken);
        _logger.LogInformation("Refresh token {Token} revoked", request.token);
        return Result<string>.SuccessResponse(message: "Logout successful");
    }
}

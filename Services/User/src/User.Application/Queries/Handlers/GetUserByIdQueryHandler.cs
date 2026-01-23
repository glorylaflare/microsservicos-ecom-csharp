using FluentResults;
using MediatR;
using Serilog;
using User.Application.Interfaces;
using User.Application.Responses;
namespace User.Application.Queries.Handlers;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<GetUserResponse>>
{
    private readonly IUserReadService _userService;
    private readonly ILogger _logger;
    public GetUserByIdQueryHandler(IUserReadService userService)
    {
        _userService = userService;
        _logger = Log.ForContext<GetUserByIdQueryHandler>();
    }
    public async Task<Result<GetUserResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.Information("[INFO] Handling {EventName} for UserId: {UserId}", nameof(GetUserByIdQuery), request.Id);
            var user = await _userService.GetByIdAsync(request.Id);
            if (user is null)
            {
                _logger.Warning("[WARN] User with ID {UserId} not found in the repository", request.Id);
                return Result.Fail<GetUserResponse>($"User with ID {request.Id} not found.");
            }
            var response = new GetUserResponse(
                user.Id,
                user.Username,
                user.Email,
                user.Status,
                user.CreatedAt,
                user.UpdatedAt
            );
            _logger.Information("[INFO] Successfully fetched and mapped user with ID {UserId}", request.Id);
            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "[ERROR] Error occurred while fetching user with ID {UserId}", request.Id);
            return Result.Fail<GetUserResponse>("An error occurred while processing your request.");
        }
    }
}
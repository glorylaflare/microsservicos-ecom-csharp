using FluentResults;
using MediatR;
using Serilog;
using User.Application.Interfaces;
using User.Application.Responses;

namespace User.Application.Queries.Handlers;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result<IEnumerable<GetUserResponse>>>
{
    private readonly IUserReadService _userService;
    private readonly ILogger _logger;

    public GetAllUsersQueryHandler(IUserReadService userService)
    {
        _userService = userService;
        _logger = Log.ForContext<GetAllUsersQueryHandler>();
    }

    public async Task<Result<IEnumerable<GetUserResponse>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.Information("[INFO] Handling {EventName}", nameof(GetAllUsersQuery));

            var users = await _userService.GetAllAsync();
            if (users is null || !users.Any())
            {
                _logger.Warning("[WARN] No users found in the repository");
                return Result.Fail<IEnumerable<GetUserResponse>>("No users found.");
            }

            var response = users.Select(user => new GetUserResponse(
                user.Id,
                user.Username,
                user.Email,
                user.Status,
                user.CreatedAt, 
                user.UpdatedAt
            ));

            _logger.Information("[INFO] Successfully fetched and mapped {UserCount} users", response.Count());
            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "[ERROR] Error occurred while fetching all users");
            return Result.Fail<IEnumerable<GetUserResponse>>("An error occurred while processing your request.");
        }
    }
}

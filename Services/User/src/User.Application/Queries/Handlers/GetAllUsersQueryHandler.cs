using FluentResults;
using MediatR;
using Serilog;
using User.Application.Responses;
using User.Domain.Interfaces;

namespace User.Application.Queries.Handlers;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result<IEnumerable<GetUserResponse>>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger _logger;

    public GetAllUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
        _logger = Log.ForContext<GetAllUsersQueryHandler>();
    }

    public async Task<Result<IEnumerable<GetUserResponse>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.Information("Handling {EventName}", nameof(GetAllUsersQuery));

            _logger.Debug("Fetching all users from the repository");
            var users = await _userRepository.GetAllUsersAsync();
            if (users is null || !users.Any())
            {
                _logger.Warning("No users found in the repository");
                return Result.Fail<IEnumerable<GetUserResponse>>("No users found.");
            }

            var response = users.Select(user => new GetUserResponse(
                user.Id,
                user.Username,
                user.Email,
                user.CreatedAt,
                user.UpdatedAt
            ));

            _logger.Information("Successfully fetched and mapped {UserCount} users", response.Count());
            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while fetching all users");
            return Result.Fail<IEnumerable<GetUserResponse>>("An error occurred while processing your request.");
        }
    }
}

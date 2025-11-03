using FluentResults;
using MediatR;
using Serilog;
using User.Application.Responses;
using User.Domain.Interfaces;

namespace User.Application.Queries.Handlers;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<GetUserResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger _logger;

    public GetUserByIdQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
        _logger = Log.ForContext<GetUserByIdQueryHandler>();
    }

    public async Task<Result<GetUserResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.Information("Handling {EventName} for UserId: {UserId}", nameof(GetUserByIdQuery), request.Id);

            _logger.Debug("Fetching user with ID {UserId} from the repository", request.Id);
            var user = await _userRepository.GetUserByIdAsync(request.Id);
            if (user is null)
            {
                _logger.Warning("User with ID {UserId} not found in the repository", request.Id);
                return Result.Fail<GetUserResponse>($"User with ID {request.Id} not found.");
            }

            var response = new GetUserResponse(
                user.Id,
                user.Username,
                user.Email,
                user.CreatedAt,
                user.UpdatedAt
            );

            _logger.Information("Successfully fetched and mapped user with ID {UserId}", request.Id);
            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while fetching user with ID {UserId}", request.Id);
            return Result.Fail<GetUserResponse>("An error occurred while processing your request.");
        }
    }
}

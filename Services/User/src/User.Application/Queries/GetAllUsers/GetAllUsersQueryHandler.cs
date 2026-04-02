using BuildingBlocks.SharedKernel.Common;
using FluentResults;
using MediatR;
using Serilog;
using User.Application.Interfaces;
using User.Application.Responses;
using User.Application.Specifications;

namespace User.Application.Queries.GetAllUsers;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result<PageResult<UserResponse>>>
{
    private readonly IUserReadService _userService;
    private readonly ILogger _logger;
    public GetAllUsersQueryHandler(IUserReadService userService)
    {
        _userService = userService;
        _logger = Log.ForContext<GetAllUsersQueryHandler>();
    }
    public async Task<Result<PageResult<UserResponse>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.Information("[INFO] Handling {EventName}", nameof(GetAllUsersQuery));

            var users = await _userService.WhereAsync(new AllUsersSpec(request.Skip, request.Take));
            if (users is null || !users.Any())
            {
                _logger.Warning("[WARN] No users found in the repository");
                return Result.Fail("No users found.");
            }
            
            var response = users.Select(user => new UserResponse(
                user.Id,
                user.Username,
                user.Email,
                user.Status,
                user.CreatedAt,
                user.UpdatedAt
            ));
            
            _logger.Information("[INFO] Successfully fetched and mapped {UserCount} users", response.Count());
            
            return Result.Ok(new PageResult<UserResponse>
            {
                Items = response,
                Total = response.Count(),
                Skip = request.Skip,
                Take = request.Take
            });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "[ERROR] Error occurred while fetching all users");
            return Result.Fail("An error occurred while processing your request.");
        }
    }
}
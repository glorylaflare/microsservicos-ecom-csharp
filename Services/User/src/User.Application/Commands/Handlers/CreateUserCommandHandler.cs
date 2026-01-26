using Auth0.Core.Exceptions;
using BuildingBlocks.Messaging;
using BuildingBlocks.Contracts.Datas;
using FluentResults;
using FluentValidation;
using MediatR;
using Serilog;
using User.Application.Interfaces;
using User.Domain.Interfaces;
using BuildingBlocks.Contracts.MongoEvents;
namespace User.Application.Commands.Handlers;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<int>>
{
    private readonly IUserRepository _userRepository;
    private readonly IValidator<CreateUserCommand> _validator;
    private readonly IAuthService _authService;
    private readonly ILogger _logger;
    private readonly IEventBus _eventBus;

    public CreateUserCommandHandler(IUserRepository userRepository, IValidator<CreateUserCommand> validator, IAuthService authService, IEventBus eventBus)
    {
        _userRepository = userRepository;
        _validator = validator;
        _authService = authService;
        _eventBus = eventBus;
        _logger = Log.ForContext<CreateUserCommandHandler>();
    }
    
    public async Task<Result<int>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        _logger.Information("[INFO] Handling {EventName} for user: {UserName}", nameof(CreateUserCommand), request.Username);

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new Error(e.ErrorMessage));
            _logger.Warning("[WARN] Validation failed for {EventName}: {Errors}", nameof(CreateUserCommand), errors);
            return Result.Fail(errors);
        }
        
        try
        {
            var createdUser = await _authService.SignupUserAsync(request.Email, request.Password);
            
            var user = new Domain.Models.User(
                createdUser.Id,
                request.Username,
                request.Email
            );

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            #region MongoDB Event Sourcing
            _logger.Information("[INFO] Publishing UserCreatedEvent for user ID {UserId}", user.Id);
            var data = new UserUpdatedData(
                user.Auth0UserId, 
                user.Username, 
                user.Email
            );
            var mongoEvnt = new UserUpdatedEvent(data);
            await _eventBus.PublishAsync(mongoEvnt);
            #endregion

            _logger.Information("[INFO] User {UserName} created successfully with ID {UserId}", request.Username, user.Id);
            return Result.Ok(user.Id);
        }
        catch (ApiException ex)
        {
            _logger.Error(ex, "[ERROR] Auth0 API error occurred while creating user {UserName}: {Message}", request.Username, ex.Message);
            return Result.Fail("An error occurred while creating the user in the authentication service.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "[ERROR] Error occurred while creating user {UserName}", request.Username);
            return Result.Fail("An error occurred while creating the user.");
        }
    }
}
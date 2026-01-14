using FluentResults;
using FluentValidation;
using MediatR;
using Serilog;
using User.Domain.Interfaces;

namespace User.Application.Commands.Handlers;

public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IValidator<DeactivateUserCommand> _validator;
    private readonly ILogger _logger;

    public DeactivateUserCommandHandler(IUserRepository userRepository, IValidator<DeactivateUserCommand> validator)
    {
        _userRepository = userRepository;
        _validator = validator;
        _logger = Log.ForContext<DeactivateUserCommandHandler>();
    }

    public async Task<Result> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        _logger.Information("[INFO] Handling {EventName} for user: {Email}", nameof(DeactivateUserCommand), request.Email);

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new Error(e.ErrorMessage));
            _logger.Warning("[WARN] Validation failed for {EventName}: {Errors}", nameof(DeactivateUserCommand), errors);
            return Result.Fail(errors);
        }

        try
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.Warning("[WARN] User with email {Email} not found", request.Email);
                return Result.Fail("User not found.");
            }

            user.Deactivate();
            await _userRepository.SaveChangesAsync();
            _logger.Information("[INFO] User with email {Email} deactivated successfully", request.Email);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "[ERROR] An error occurred while deactivating user with email {Email}: {Message}", request.Email, ex.Message);
            return Result.Fail("An unexpected error occurred.");
        }
    }
}

using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BuildingBlocks.Security.Context;

public sealed class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _context;

    public UserContext(IHttpContextAccessor context)
    {
        _context = context;
    }

    public string UserId
    {
        get
        {
            var httpContext = _context.HttpContext;
            var user = httpContext?.User;
            
            if (user?.Identity == null || !user.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User ID claim not found in the current context.");
            }

            return userId;
        }
    }

    public bool IsAuthenticated => _context.HttpContext?
        .User?
        .Identity?
        .IsAuthenticated ?? false;
}

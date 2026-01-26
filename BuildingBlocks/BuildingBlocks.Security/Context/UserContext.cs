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

    public string UserId => _context.HttpContext?
            .User?
            .FindFirst(ClaimTypes.NameIdentifier)?
            .Value ?? throw new UnauthorizedAccessException("Authenticated user without UserId claim");

    public bool IsAuthenticated => _context.HttpContext?
        .User?
        .Identity?
        .IsAuthenticated ?? false;
}

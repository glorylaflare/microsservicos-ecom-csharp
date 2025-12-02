using BuildingBlocks.SharedKernel.Common;

namespace User.Domain.Models;

public class User : EntityBase
{
    public string Auth0UserId { get; private set; }
    public string Username { get; private set; }
    public string Email { get; private set; }
    public Status Status { get; private set; }

    protected User() { }

    public User(string auth0UserId, string username, string email)
    {
        Auth0UserId = auth0UserId;
        Username = username;
        Email = email;
        Status = Status.Active;
    }

    public void Deactivate()
    {
        Status = Status.Inactive;
        UpdatedAt = DateTime.UtcNow;
    }
}

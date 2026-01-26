namespace BuildingBlocks.Security.Context;

public interface IUserContext
{
    string UserId { get; }
    bool IsAuthenticated { get; }
}

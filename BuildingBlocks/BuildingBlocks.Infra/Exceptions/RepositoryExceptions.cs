namespace BuildingBlocks.Infra.Exceptions;

public class RepositoryException : Exception
{
    public RepositoryException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

public sealed class RepositoryQueryException : RepositoryException
{
    public RepositoryQueryException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

public sealed class RepositorySpecificationException : RepositoryException
{
    public RepositorySpecificationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
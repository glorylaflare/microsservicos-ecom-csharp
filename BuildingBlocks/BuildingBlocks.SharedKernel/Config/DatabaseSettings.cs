namespace BuildingBlocks.SharedKernel.Config;

public class DatabaseSettings
{
    public required string Host { get; set; }
    public int Port { get; } = 1433;
    public required string Database { get; set; }
    public bool TrustServerCertificate { get; } = true;
    public bool EnableConnectionPooling { get; } = true;
    public int MinPoolSize { get; } = 0;
    public int MaxPoolSize { get; } = 50;
    public string ToConnectionString()
    {
        var pooling = EnableConnectionPooling ?
            $"Min Pool Size={MinPoolSize};Max Pool Size={MaxPoolSize};"
            : string.Empty;
        return
            $"Server={Host},{Port};" +
            $"Database={Database};" +
            $"TrustServerCertificate={TrustServerCertificate};" +
            $"{pooling}" +
            "MultipleActiveResultSets=true;" +
            "Integrated Security=True;";
    }
    public string ToConnectionStringWithoutPooling()
    {
        return
            $"Server={Host},{Port};" +
            $"Database={Database};" +
            $"TrustServerCertificate={TrustServerCertificate};" +
            "MultipleActiveResultSets=true;" +
            "Integrated Security=True;";
    }
}
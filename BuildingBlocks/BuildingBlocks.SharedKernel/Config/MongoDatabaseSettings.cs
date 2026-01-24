namespace BuildingBlocks.SharedKernel.Config;

public class MongoDatabaseSettings
{
    public string ConnectionString { get; set; } = default!;
    public string DatabaseName { get; set; } = default!;
}

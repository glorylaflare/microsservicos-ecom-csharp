namespace BuildingBlocks.SharedKernel.Common;

public class PageResult<T>
{
    public IEnumerable<T> Items { get; init; }
    public int Total { get; init; }
    public int Skip { get; init; }
    public int Take { get; init; }
}

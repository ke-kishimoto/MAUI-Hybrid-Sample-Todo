namespace Frontend.Sync;

public sealed class TodoApiItem
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public bool IsCompleted { get; init; }

    public DateTimeOffset CreatedAt { get; init; }

    public DateTimeOffset UpdatedAt { get; init; }

    public DateTimeOffset? DeletedAt { get; init; }

    public long Version { get; init; }
}

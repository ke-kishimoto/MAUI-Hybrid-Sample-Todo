namespace Frontend.Data;

public sealed class LocalTodoListItem
{
    public string Id { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;

    public bool IsCompleted { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime UpdatedAt { get; init; }

    public long Version { get; init; }

    public bool IsPendingSync { get; init; }
}

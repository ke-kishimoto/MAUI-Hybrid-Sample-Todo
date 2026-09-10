namespace Frontend.Data;

public sealed class PendingCreateOperation
{
    public string OperationId { get; init; } = string.Empty;

    public string TodoItemId { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;
}

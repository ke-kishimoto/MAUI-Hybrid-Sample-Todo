namespace Frontend.Sync;

public sealed class CreateTodoApiRequest
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;
}

using Frontend.Data;

namespace Frontend.Sync;

public sealed class TodoSyncService
{
    private readonly SemaphoreSlim syncLock = new(1, 1);
    private readonly LocalTodoService localTodos;
    private readonly TodoApiClient apiClient;

    public TodoSyncService(LocalTodoService localTodos, TodoApiClient apiClient)
    {
        this.localTodos = localTodos;
        this.apiClient = apiClient;
    }

    public async Task<SyncResult> SyncAsync(CancellationToken cancellationToken = default)
    {
        await syncLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            var succeededCount = 0;
            var failedCount = 0;
            var operations = await localTodos.GetPendingCreatesAsync().ConfigureAwait(false);

            foreach (var operation in operations)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    var serverItem = await apiClient.CreateAsync(
                        Guid.Parse(operation.TodoItemId),
                        operation.Title,
                        cancellationToken).ConfigureAwait(false);
                    await localTodos.CompleteCreateSyncAsync(operation, serverItem).ConfigureAwait(false);
                    succeededCount++;
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    await localTodos.RecordSyncFailureAsync(operation.OperationId, exception.Message)
                        .ConfigureAwait(false);
                    failedCount++;
                }
            }

            return new SyncResult(succeededCount, failedCount);
        }
        finally
        {
            syncLock.Release();
        }
    }
}

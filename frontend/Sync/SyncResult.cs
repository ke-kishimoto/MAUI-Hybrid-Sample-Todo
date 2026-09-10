namespace Frontend.Sync;

public sealed record SyncResult(int SucceededCount, int FailedCount)
{
    public int ProcessedCount => SucceededCount + FailedCount;
}

namespace BackupAssistant.Test.TestHelpers
{
    /// <summary>
    /// A <see cref="SynchronizationContext"/> that runs posted/sent callbacks synchronously and
    /// inline, regardless of which thread posts them. Progress&lt;T&gt; captures
    /// SynchronizationContext.Current at construction time and marshals every report through it,
    /// so tests that assert on Progress&lt;T&gt; callbacks need this installed to get deterministic,
    /// synchronous delivery instead of polling for an asynchronously-dispatched update.
    /// </summary>
    public class InlineSynchronizationContext : SynchronizationContext
    {
        public override void Post(SendOrPostCallback d, object? state)
        {
            d(state);
        }

        public override void Send(SendOrPostCallback d, object? state)
        {
            d(state);
        }
    }
}

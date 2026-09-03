namespace Reactive.Boolean;

/// <summary>
/// Specifies how a scheduling operator completes when its source completes while the operator's timer is still running.
/// </summary>
public enum CompletionBehavior
{
    /// <summary>
    /// The resulting observable completes immediately. A value that was still pending on the timer is never emitted.
    /// </summary>
    CompleteImmediately,

    /// <summary>
    /// The resulting observable stays alive until the timer elapses, emits the pending value and completes afterwards.
    /// It completes immediately when no value is pending.
    /// </summary>
    CompleteAfterTimer
}

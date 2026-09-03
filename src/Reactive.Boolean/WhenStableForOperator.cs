using System.Reactive.Concurrency;

namespace Reactive.Boolean;

internal sealed class WhenStableForOperator(
    IObserver<bool> observer,
    TimeSpan timeSpan,
    IScheduler scheduler,
    bool distinctUntilChanged,
    bool resetTimerOnConsecutiveValue,
    CompletionBehavior completionBehavior)
    : TimedBooleanOperator(observer, timeSpan, scheduler, distinctUntilChanged, completionBehavior)
{
    protected override void OnSourceValue(bool value)
    {
        if (LastEmittedValue == null)
        {
            Emit(value);
            return;
        }

        if (value == LastEmittedValue)
        {
            // The source returned to the emitted value before the change became stable.
            StopTimer();
            Emit(value);
            return;
        }

        if (!TimerRunning || resetTimerOnConsecutiveValue)
        {
            StartTimer();
        }
    }

    // The timer only runs while the latest source value differs from the emitted one, so it is the pending change.
    protected override void OnTimerElapsed() => Emit(LastSourceValue!.Value);
}

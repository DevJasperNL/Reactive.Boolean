using System.Reactive.Concurrency;

namespace Reactive.Boolean;

internal sealed class BlinkWhileTrueOperator(
    IObserver<bool> observer,
    TimeSpan onDuration,
    TimeSpan offDuration,
    IScheduler scheduler,
    bool distinctUntilChanged,
    bool resetTimerOnConsecutiveTrue,
    CompletionBehavior completionBehavior)
    : TimedBooleanOperator(observer, onDuration, scheduler, distinctUntilChanged, completionBehavior)
{
    // The base timer duration is the "true" phase; only the "false" phase needs its own duration.

    // Under CompleteAfterTimer only the current "true" phase is finished, so an "off" phase has nothing pending.
    protected override bool HasPendingValue => TimerRunning && LastEmittedValue == true;

    protected override void OnSourceValue(bool value, bool? previous)
    {
        if (!value)
        {
            StopTimer();
            Emit(false);
            return;
        }

        if (previous != true || resetTimerOnConsecutiveTrue)
        {
            Emit(true);
            StartTimer();
        }
    }

    protected override void OnTimerElapsed()
    {
        if (LastEmittedValue == true)
        {
            Emit(false);
            if (!SourceCompleted)
            {
                StartTimer(offDuration);
            }

            return;
        }

        Emit(true);
        StartTimer();
    }
}

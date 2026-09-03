using System.Reactive.Concurrency;

namespace Reactive.Boolean;

internal sealed class PersistTrueForOperator(
    IObserver<bool> observer,
    TimeSpan timeSpan,
    IScheduler scheduler,
    bool distinctUntilChanged,
    bool resetTimerOnConsecutiveFalse,
    CompletionBehavior completionBehavior)
    : TimedBooleanOperator(observer, timeSpan, scheduler, distinctUntilChanged, completionBehavior)
{
    protected override void OnSourceValue(bool value)
    {
        if (value)
        {
            StopTimer();
            Emit(true);
            return;
        }

        if (LastSourceValue == true)
        {
            StartTimer();
            return;
        }

        if (TimerRunning)
        {
            if (resetTimerOnConsecutiveFalse)
            {
                StartTimer();
            }

            return;
        }

        Emit(false);
    }

    protected override void OnTimerElapsed() => Emit(false);
}

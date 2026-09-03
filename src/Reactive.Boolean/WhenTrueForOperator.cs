using System.Reactive.Concurrency;

namespace Reactive.Boolean;

internal sealed class WhenTrueForOperator(
    IObserver<bool> observer,
    TimeSpan timeSpan,
    IScheduler scheduler,
    bool distinctUntilChanged,
    bool resetTimerOnConsecutiveTrue,
    CompletionBehavior completionBehavior)
    : TimedBooleanOperator(observer, timeSpan, scheduler, distinctUntilChanged, completionBehavior)
{
    protected override void OnSourceValue(bool value)
    {
        if (!value)
        {
            StopTimer();
            Emit(false);
            return;
        }

        if (LastSourceValue == null)
        {
            Emit(false);
        }

        if (LastSourceValue != true)
        {
            StartTimer();
            return;
        }

        if (TimerRunning)
        {
            if (resetTimerOnConsecutiveTrue)
            {
                StartTimer();
            }

            return;
        }

        Emit(true);
    }

    protected override void OnTimerElapsed() => Emit(true);
}

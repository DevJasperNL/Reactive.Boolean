using System.Reactive.Concurrency;

namespace Reactive.Boolean;

internal sealed class LimitTrueDurationOperator(
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

        if (LastSourceValue != true)
        {
            Emit(true);
            StartTimer();
            return;
        }

        if (TimerRunning)
        {
            Emit(true);
            if (resetTimerOnConsecutiveTrue)
            {
                StartTimer();
            }

            return;
        }

        // The limit was reached and the source merely repeats "true": only a reset re-arms the limit.
        if (resetTimerOnConsecutiveTrue)
        {
            Emit(true);
            StartTimer();
        }
    }

    protected override void OnTimerElapsed() => Emit(false);
}

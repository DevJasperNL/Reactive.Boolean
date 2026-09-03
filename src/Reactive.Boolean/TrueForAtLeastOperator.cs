using System.Reactive.Concurrency;

namespace Reactive.Boolean;

internal sealed class TrueForAtLeastOperator(
    IObserver<bool> observer,
    TimeSpan timeSpan,
    IScheduler scheduler,
    bool distinctUntilChanged,
    bool resetTimerOnConsecutiveTrue,
    CompletionBehavior completionBehavior)
    : TimedBooleanOperator(observer, timeSpan, scheduler, distinctUntilChanged, completionBehavior)
{
    protected override bool HasPendingValue => TimerRunning && LastSourceValue == false;

    protected override void OnSourceValue(bool value)
    {
        if (!value)
        {
            // A "false" during the window is withheld; the timer releases it if the source is still false by then.
            if (!TimerRunning)
            {
                Emit(false);
            }

            return;
        }

        // A rising edge always (re)starts the window; a repeated "true" only when asked to.
        if (!TimerRunning || resetTimerOnConsecutiveTrue || LastSourceValue != true)
        {
            StartTimer();
        }

        Emit(true);
    }

    protected override void OnTimerElapsed()
    {
        if (LastSourceValue == false)
        {
            Emit(false);
        }
    }
}

using System.Reactive.Concurrency;

namespace Reactive.Boolean;

internal sealed class PulseTrueForOperator(
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
            // A "false" during the pulse is withheld; the timer ends the pulse regardless of the source.
            if (!TimerRunning)
            {
                Emit(false);
            }

            return;
        }

        // A rising edge always (re)starts the pulse; a repeated "true" only when asked to.
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

        if (resetTimerOnConsecutiveTrue)
        {
            Emit(true);
            StartTimer();
        }
    }

    protected override void OnTimerElapsed() => Emit(false);
}

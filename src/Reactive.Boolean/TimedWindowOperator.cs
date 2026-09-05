using System.Reactive.Concurrency;

namespace Reactive.Boolean;

/// <summary>
/// Opens a window of the timespan on every rising edge of the source and decides what "false" means while it is open
/// and when it ends. <see cref="BooleanObservableExtensions.TrueForAtLeast"/> withholds a "false" until the window ends,
/// <see cref="BooleanObservableExtensions.LimitTrueDuration"/> forces a "false" when it ends and
/// <see cref="BooleanObservableExtensions.PulseTrueFor"/> does both.
/// </summary>
internal sealed class TimedWindowOperator(
    IObserver<bool> observer,
    TimeSpan timeSpan,
    IScheduler scheduler,
    bool distinctUntilChanged,
    bool resetTimerOnConsecutiveTrue,
    CompletionBehavior completionBehavior,
    bool withholdFalseDuringWindow,
    bool forceFalseAtEnd)
    : TimedBooleanOperator(observer, timeSpan, scheduler, distinctUntilChanged, completionBehavior)
{
    protected override bool HasPendingValue => TimerRunning && (forceFalseAtEnd || LastSourceValue == false);

    protected override void OnSourceValue(bool value, bool? previous)
    {
        if (!value)
        {
            if (withholdFalseDuringWindow && TimerRunning)
            {
                return;
            }

            StopTimer();
            Emit(false);
            return;
        }

        // A rising edge always opens a window; a repeated "true" only restarts it when asked to.
        if (previous != true)
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

        // The window ended and the source merely repeats "true".
        if (resetTimerOnConsecutiveTrue)
        {
            Emit(true);
            StartTimer();
        }
        else if (!forceFalseAtEnd)
        {
            // Nothing was forced, so the output still tracks the source.
            Emit(true);
        }
    }

    protected override void OnTimerElapsed()
    {
        if (forceFalseAtEnd || LastSourceValue == false)
        {
            Emit(false);
        }
    }
}

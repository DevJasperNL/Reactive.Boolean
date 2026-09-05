using System.Reactive.Concurrency;

namespace Reactive.Boolean;

/// <summary>
/// Delays a transition of the source by the timespan and cancels it when the source reverts before the timer runs out.
/// <see cref="BooleanObservableExtensions.WhenTrueFor"/> (delay "true", assume an initial "false"),
/// <see cref="BooleanObservableExtensions.PersistTrueFor"/> (delay "false", pass the first value through) and
/// <see cref="BooleanObservableExtensions.WhenStableFor"/> (delay both, pass the first value through) are all instances of this machine.
/// </summary>
internal sealed class DelayedTransitionOperator(
    IObserver<bool> observer,
    TimeSpan timeSpan,
    IScheduler scheduler,
    bool distinctUntilChanged,
    bool resetTimerOnConsecutiveValue,
    CompletionBehavior completionBehavior,
    bool delayTrue,
    bool delayFalse,
    bool? assumedInitialValue)
    : TimedBooleanOperator(observer, timeSpan, scheduler, distinctUntilChanged, completionBehavior)
{
    protected override void OnSourceValue(bool value, bool? previous)
    {
        if (LastEmittedValue == null)
        {
            if (assumedInitialValue == null)
            {
                Emit(value);
                return;
            }

            // The assumed value is the current state; a differing first value is a transition like any other.
            Emit(assumedInitialValue.Value);
            if (value == assumedInitialValue)
            {
                return;
            }
        }

        var delayed = value ? delayTrue : delayFalse;
        if (value == LastEmittedValue || !delayed)
        {
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

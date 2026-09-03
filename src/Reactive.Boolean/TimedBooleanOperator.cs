using System.Reactive.Concurrency;
using System.Reactive.Disposables;

namespace Reactive.Boolean;

/// <summary>
/// Base class for the time-based operators. Subscribes to the source exactly once and drives a single restartable timer
/// on the provided scheduler, so that source values and timer expiry are handled as one ordered stream of events under
/// one lock. Subclasses only decide what to emit and when to (re)start the timer.
/// </summary>
internal abstract class TimedBooleanOperator(
    IObserver<bool> observer,
    TimeSpan timeSpan,
    IScheduler scheduler,
    bool distinctUntilChanged,
    CompletionBehavior completionBehavior)
{
    private readonly object _gate = new();
    private readonly SerialDisposable _timer = new();

    private int _timerGeneration;
    private bool? _lastEmittedValue;
    private bool _sourceCompleted;
    private bool _terminated;

    /// <summary>
    /// Inside <see cref="OnSourceValue"/>: the value that preceded the one being handled.
    /// Inside <see cref="OnTimerElapsed"/>: the latest value. Null until the source has emitted.
    /// </summary>
    protected bool? LastSourceValue { get; private set; }

    protected bool TimerRunning { get; private set; }

    /// <summary>
    /// Whether the running timer will emit a value when it elapses. Decides whether
    /// <see cref="CompletionBehavior.CompleteAfterTimer"/> has anything to wait for.
    /// </summary>
    protected virtual bool HasPendingValue => TimerRunning;

    protected abstract void OnSourceValue(bool value);

    protected abstract void OnTimerElapsed();

    public IDisposable Run(IObservable<bool> source)
    {
        var subscription = source.Subscribe(OnNext, OnError, OnCompleted);
        var stop = Disposable.Create(() =>
        {
            lock (_gate)
            {
                _terminated = true;
            }
        });
        return new CompositeDisposable(subscription, _timer, stop);
    }

    protected void Emit(bool value)
    {
        if (distinctUntilChanged && _lastEmittedValue == value)
        {
            return;
        }

        _lastEmittedValue = value;
        observer.OnNext(value);
    }

    protected void StartTimer()
    {
        var generation = ++_timerGeneration;
        TimerRunning = true;
        _timer.Disposable = scheduler.Schedule(timeSpan, () => TimerElapsed(generation));
    }

    protected void StopTimer()
    {
        _timerGeneration++;
        TimerRunning = false;
        _timer.Disposable = Disposable.Empty;
    }

    private void OnNext(bool value)
    {
        lock (_gate)
        {
            if (_terminated)
            {
                return;
            }

            OnSourceValue(value);
            LastSourceValue = value;
        }
    }

    private void OnError(Exception error)
    {
        lock (_gate)
        {
            if (_terminated)
            {
                return;
            }

            Terminate();
            observer.OnError(error);
        }
    }

    private void OnCompleted()
    {
        lock (_gate)
        {
            if (_terminated)
            {
                return;
            }

            if (completionBehavior == CompletionBehavior.CompleteAfterTimer && HasPendingValue)
            {
                _sourceCompleted = true;
                return;
            }

            Terminate();
            observer.OnCompleted();
        }
    }

    private void TimerElapsed(int generation)
    {
        lock (_gate)
        {
            // A stale callback can still fire when it was dequeued just before the timer was restarted or stopped.
            if (_terminated || generation != _timerGeneration)
            {
                return;
            }

            TimerRunning = false;
            OnTimerElapsed();

            if (_sourceCompleted && !TimerRunning)
            {
                Terminate();
                observer.OnCompleted();
            }
        }
    }

    private void Terminate()
    {
        _terminated = true;
        StopTimer();
    }
}

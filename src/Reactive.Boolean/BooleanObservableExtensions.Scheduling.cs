using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace Reactive.Boolean
{
    public static partial class BooleanObservableExtensions
    {
        /// <summary>
        /// Returns an observable that won't emit "false" for at least <paramref name="timeSpan"/> after an initial "true" is emitted by <paramref name="source"/>.
        /// If a "false" is emitted during the <paramref name="timeSpan"/>, it will be emitted immediately after the timer is completed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="timeSpan"></param>
        /// <param name="scheduler"></param>
        /// <param name="distinctUntilChanged">If set to "false", the resulting observable will not be distinct. Both consecutive "true" and "false" values will be emitted. Note that consecutive "false" values that occur during the timer, will only be emitted as a single "false" once the timer runs out.</param>
        /// <param name="resetTimerOnConsecutiveTrue">If "true", every "true" that is emitted by <paramref name="source"/> will reset the timer.</param>
        /// <returns></returns>
        public static IObservable<bool> TrueForAtLeast(
            this IObservable<bool> source,
            TimeSpan timeSpan,
            IScheduler scheduler,
            bool distinctUntilChanged = true,
            bool resetTimerOnConsecutiveTrue = false)
        {
            var timer = CreateTimer(source, timeSpan, scheduler, resetTimerOnConsecutiveTrue);

            if (distinctUntilChanged)
            {
                return source.CombineLatest(timer)
                    .Where(t => t.First || !t.Second)
                    .Select(t => t.First)
                    .DistinctUntilChanged();
            }

            return source.CombineLatest(timer)
                .Select(tuple => ((bool?)tuple.First, (bool?)tuple.Second)) // We convert to nullables to allow an initial seed of (null, null) in the pairing method.
                .Pairwise((null, null)) // As double true values from the source do need to be emitted. We need the previous item to determine whether the change comes from the source or from the timer. 
                .Where(tuple =>
                {
                    var previous = tuple.Item1;
                    var current = tuple.Item2;

                    var currentValue = current.Item1;

                    var previousTimer = previous.Item2;
                    var currentTimer = current.Item2;

                    var timerValueChanged = previousTimer != null && previousTimer != currentTimer;
                    if (!timerValueChanged)
                    {
                        return currentValue == true || currentTimer == false;
                    }
                    // Only when the timer is done and the current value is false, we have to emit something based on the timer.
                    return currentValue == false && currentTimer == false;
                })
                .Select(t => t.Item2.Item1!.Value); // We can assume the item is not null as we only use null for the previous value.
        }

        /// <summary>
        /// Returns an observable that won't emit "true" for at least <paramref name="timeSpan"/> after an initial "false" is emitted by <paramref name="source"/>.
        /// If a "true" is emitted during the <paramref name="timeSpan"/>, it will be emitted immediately after the timer is completed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="timeSpan"></param>
        /// <param name="scheduler"></param>
        /// <param name="distinctUntilChanged">If set to "false", the resulting observable will not be distinct. Both consecutive "true" and "false" values will be emitted. Note that consecutive "true" values that occur during the timer, will only be emitted as a single "true" once the timer runs out.</param>
        /// <param name="resetTimerOnConsecutiveFalse">If "true", every "false" that is emitted by <paramref name="source"/> will reset the timer.</param>
        /// <returns></returns>
        public static IObservable<bool> FalseForAtLeast(
            this IObservable<bool> source,
            TimeSpan timeSpan,
            IScheduler scheduler,
            bool distinctUntilChanged = true,
            bool resetTimerOnConsecutiveFalse = false) => 
            source
                .Not()
                .TrueForAtLeast(timeSpan, scheduler, distinctUntilChanged, resetTimerOnConsecutiveFalse)
                .Not();

        /// <summary>
        /// Returns an observable that delays the first "false" that is emitted after a "true" by <paramref name="source"/> for a duration of <paramref name="timeSpan"/>.
        /// Resulting observable is distinct.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="timeSpan"></param>
        /// <param name="scheduler"></param>
        /// <param name="resetTimerOnConsecutiveFalse">If "true", every "false" that is emitted by <paramref name="source"/> will reset the timer.</param>
        /// <returns></returns>
        public static IObservable<bool> PersistTrueFor(
            this IObservable<bool> source,
            TimeSpan timeSpan,
            IScheduler scheduler,
            bool resetTimerOnConsecutiveFalse = false)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(scheduler);
            if (timeSpan <= TimeSpan.Zero)
            {
                return source;
            }

            /*
             * Note: Attempting to implement this method with both parameters distinctUntilChanged and resetTimerOnConsecutiveFalse is not impossible, but is difficult to do in a clean way:
             * The difficulty with distinctUntilChanged as false, is that consecutive false values should be emitted if the timer is not running.
             * The difficulty with resetTimerOnConsecutiveFalse as true, is that consecutive false values should only reset a timer when it's already running.
             * Only implementing resetTimerOnConsecutiveFalse was relatively easy, as we can always use a false value to reset the Throttle timer and simply apply DistinctUntilChanged to the end result.
             */

            var trueObservable = source.Where(b => b);
            var initialFalseObservable = source.Take(1).Where(b => !b);

            IObservable<bool> delayedFalseObservable;
            if (resetTimerOnConsecutiveFalse)
            {
                delayedFalseObservable = source
                    .Skip(1) // Skip the first value as both the first true and the first false should immediately publish a value.
                    .Throttle(timeSpan,
                        scheduler) // This will publish the value after x time. Note that we also pass "true" values here, to prevent a delayed "false" from being released.
                    .Where(b => !b); // As a "true" is also passed to throttle, we filter it out here.
            }
            else
            {
                delayedFalseObservable = source
                    .Skip(1) // Skip the first value as both the first true and the first false should immediately publish a value.
                    .DistinctUntilChanged() // Filter out consecutive values to prevent the timer from being reset.
                    .Throttle(timeSpan,
                        scheduler) // This will publish the value after x time. Note that we also pass "true" values here, to prevent a delayed "false" from being released.
                    .Where(b => !b); // As a "true" is also passed to throttle, we filter it out here.
            }

            return trueObservable
                .Merge(initialFalseObservable)
                .Merge(delayedFalseObservable)
                .DistinctUntilChanged();
        }

        /// <summary>
        /// Returns an observable that delays the first "true" that is emitted after a "false" by <paramref name="source"/> for a duration of <paramref name="timeSpan"/>.
        /// Resulting observable is distinct.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="timeSpan"></param>
        /// <param name="scheduler"></param>
        /// <param name="resetTimerOnConsecutiveTrue">If "true", every "true" that is emitted by <paramref name="source"/> will reset the timer.</param>
        /// <returns></returns>
        public static IObservable<bool> PersistFalseFor(
            this IObservable<bool> source,
            TimeSpan timeSpan,
            IScheduler scheduler,
            bool resetTimerOnConsecutiveTrue = false) =>
            source
                .Not()
                .PersistTrueFor(timeSpan, scheduler, resetTimerOnConsecutiveTrue)
                .Not();

        /// <summary>
        /// Returns an observable that emits "true" once <paramref name="source"/> does not emit "false" for a minimum of <paramref name="timeSpan"/>.
        /// Resulting observable is distinct.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="timeSpan"></param>
        /// <param name="scheduler"></param>
        /// <param name="resetTimerOnConsecutiveTrue">If "true", every "true" that is emitted by <paramref name="source"/> will reset the timer.</param>
        public static IObservable<bool> WhenTrueFor(
            this IObservable<bool> source,
            TimeSpan timeSpan,
            IScheduler scheduler, 
            bool resetTimerOnConsecutiveTrue = false)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(scheduler);

            if (timeSpan <= TimeSpan.Zero)
            {
                return source;
            }

            /*
             * Note: Attempting to implement this method with both parameters distinctUntilChanged and resetTimerOnConsecutiveTrue together is not impossible, but is difficult to do in a clean way:
             * The difficulty with distinctUntilChanged as false, is that consecutive true values should be emitted if the timer isn't running anymore.
             * The difficulty with resetTimerOnConsecutiveTrue as true, is that consecutive true values should only reset a timer when it's still running.
             * Only implementing resetTimerOnConsecutiveTrue was relatively easy, as we can always use a true value to reset the Throttle timer and simply apply DistinctUntilChanged to the end result.
             */

            var falseObservable = source.Where(b => !b);

            var initialFalseObservable = source
                .Take(1)
                .Where(b => b) // Only when immediately true, we need to publish a false in the beginning as falseObservable won't provide it for us.
                .Select(_ => false);

            IObservable<bool> delayedTrueObservable;
            if (resetTimerOnConsecutiveTrue)
            {
                delayedTrueObservable = source
                    .Throttle(timeSpan, scheduler) // This will publish the value after x time. Note that we also pass "false" values here, to prevent a delayed "true" from being released.
                    .Where(b => b); // As a "false" is also passed to throttle, we filter it out here.
            }
            else
            {
                delayedTrueObservable = source
                    .DistinctUntilChanged() // Filter out consecutive values to prevent the timer from being reset.
                    .Throttle(timeSpan, scheduler) // This will publish the value after x time. Note that we also pass "false" values here, to prevent a delayed "true" from being released.
                    .Where(b => b); // As a "false" is also passed to throttle, we filter it out here.
            }

            return falseObservable
                .Merge(initialFalseObservable)
                .Merge(delayedTrueObservable)
                .DistinctUntilChanged();
        }

        /// <summary>
        /// Returns an observable that emits "false" once <paramref name="source"/> does not emit "true" for a minimum of <paramref name="timeSpan"/>.
        /// Resulting observable is distinct.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="timeSpan"></param>
        /// <param name="scheduler"></param>
        /// <param name="resetTimerOnConsecutiveFalse">If "true", every "false" that is emitted by <paramref name="source"/> will reset the timer.</param>
        public static IObservable<bool> WhenFalseFor(
            this IObservable<bool> source,
            TimeSpan timeSpan,
            IScheduler scheduler,
            bool resetTimerOnConsecutiveFalse = false) =>
            source
                .Not()
                .WhenTrueFor(timeSpan, scheduler, resetTimerOnConsecutiveFalse)
                .Not();

        /// <summary>
        /// Returns an observable that will automatically emit "false" if <paramref name="source"/> does not emit a "false" itself within <paramref name="timeSpan"/> after emitting "true".
        /// </summary>
        /// <param name="source"></param>
        /// <param name="timeSpan"></param>
        /// <param name="scheduler"></param>
        /// <param name="distinctUntilChanged">If set to "false", the resulting observable will not be distinct. Both consecutive "true" and "false" values will be emitted.</param>
        /// <param name="resetTimerOnConsecutiveTrue">If "true", every "true" that is emitted by <paramref name="source"/> will reset the timer.</param>
        /// <returns></returns>
        public static IObservable<bool> LimitTrueDuration(this IObservable<bool> source, TimeSpan timeSpan, IScheduler scheduler,
            bool distinctUntilChanged = true,
            bool resetTimerOnConsecutiveTrue = false)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(scheduler);
            
            if (distinctUntilChanged)
            {
                var falseWhenTrueFor = source
                    .WhenTrueFor(timeSpan, scheduler, resetTimerOnConsecutiveTrue)
                    .Where(b => b)
                    .Select(_ => false);

                return source.Merge(falseWhenTrueFor).DistinctUntilChanged();
            }

            var timer = CreateTimer(source, timeSpan, scheduler, resetTimerOnConsecutiveTrue);
            return source.CombineLatest(timer)
                .Select(tuple => ((bool?)tuple.First, (bool?)tuple.Second)) // We convert to nullables to allow an initial seed of (null, null) in the pairing method.
                .Pairwise((null, null)) // As double true values from the source do need to be emitted. We need the previous item to determine whether the change comes from the source or from the timer. 
                .Select(tuple =>
                {
                    var previous = tuple.Item1;
                    var current = tuple.Item2;

                    var previousValue = previous.Item1;
                    var currentValue = current.Item1;

                    var previousTimer = previous.Item2;
                    var currentTimer = current.Item2;

                    var timerValueChanged = previousTimer != null && previousTimer != currentTimer;
                    if (!timerValueChanged)
                    {
                        // Always emit false, and only emit true if the timer is (still) running.
                        if (currentValue == false || (currentValue == true && currentTimer == true))
                        {
                            return currentValue;
                        }
                        // It can happen that we receive the "true" that initiates the timer before we receive the value indicated that the timer has started. For that reason we also pass every first "true" value if the timer is not running.
                        if (previousValue != true && currentValue == true && currentTimer == false)
                        {
                            return true;
                        }

                        return null;
                    }

                    // Only when the timer is done and the current value is true, we have to emit false based on the timer.
                    if (currentValue == true && currentTimer == false)
                    {
                        return false;
                    }

                    return null;
                })
                .Where(b => b != null)
                .Select(b => b!.Value);
        }

        /// <summary>
        /// Returns an observable that will automatically emit "true" if <paramref name="source"/> does not emit a "true" itself within <paramref name="timeSpan"/> after emitting "false".
        /// </summary>
        /// <param name="source"></param>
        /// <param name="timeSpan"></param>
        /// <param name="scheduler"></param>
        /// <param name="distinctUntilChanged">If set to "false", the resulting observable will not be distinct. Both consecutive "true" and "false" values will be emitted.</param>
        /// <param name="resetTimerOnConsecutiveTrue">If "true", every "false" that is emitted by <paramref name="source"/> will reset the timer.</param>
        /// <returns></returns>
        public static IObservable<bool> LimitFalseDuration(
            this IObservable<bool> source, 
            TimeSpan timeSpan,
            IScheduler scheduler,
            bool distinctUntilChanged = true,
            bool resetTimerOnConsecutiveTrue = false) =>
            source
                .Not()
                .LimitTrueDuration(timeSpan, scheduler, distinctUntilChanged, resetTimerOnConsecutiveTrue)
                .Not();

        /// <summary>
        /// Returns an observable that immediately returns false, emits true when <paramref name="triggerObservable"/> emits true and will emit false again after a duration of <paramref name="timeSpan"/>.
        /// </summary>
        private static IObservable<bool> CreateTimer(
            IObservable<bool> triggerObservable, 
            TimeSpan timeSpan,
            IScheduler scheduler, 
            bool resetTimerOnConsecutiveTrue = false)
        {
            var trueObservable = triggerObservable.Where(b => b);
            IObservable<bool> delayedFalseObservable;
            if (resetTimerOnConsecutiveTrue)
            {
                delayedFalseObservable = triggerObservable
                    .Where(b => b)
                    .Throttle(timeSpan, scheduler)
                    .Select(b => !b); // We invert the result as we want to turn the timer off x time after the "true" was emitted.
            }
            else
            {
                delayedFalseObservable = triggerObservable
                    .DistinctUntilChanged()
                    .Where(b => b)
                    .Throttle(timeSpan, scheduler)
                    .Select(b => !b); // We invert the result as we want to turn the timer off x time after the "true" was emitted.
            }

            return trueObservable
                .Merge(delayedFalseObservable)
                .Prepend(false)
                .DistinctUntilChanged();
        }
    }
}

using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace Reactive.Boolean
{
    public static partial class BooleanObservableExtensions
    {
        /// <summary>
        /// Returns an observable that stays true for a <paramref name="timeSpan"/> once the base <paramref name="observable"/> turns back to false.
        /// Resulting observable is distinct.
        /// </summary>
        public static IObservable<bool> PersistTrueFor(
            this IObservable<bool> observable,
            IScheduler scheduler,
            TimeSpan timeSpan)
        {
            ArgumentNullException.ThrowIfNull(observable);
            ArgumentNullException.ThrowIfNull(scheduler);

            if (timeSpan <= TimeSpan.Zero)
            {
                return observable;
            }

            var trueObservable = observable.Where(b => b);

            var initialFalseObservable = observable.Take(1).Where(b => !b);

            var delayedFalseObservable = observable
                .Skip(1) // Skip the first value as both the first true and the first false should immediately publish a value.
                .DistinctUntilChanged() // Filter out consecutive values to prevent the timer from being reset.
                .Throttle(timeSpan, scheduler) // This will publish the value after x time. Note that we also pass "true" values here, to prevent a delayed "false" from being released.
                .Where(b => !b); // As a "true" is also passed to throttle, we filter it out here.

            return trueObservable
                .Merge(initialFalseObservable)
                .Merge(delayedFalseObservable)
                .DistinctUntilChanged();
        }

        /// <summary>
        /// Returns an observable that emits true once the base <paramref name="observable"/> emits true for a minimum <paramref name="timeSpan"/>.
        /// Resulting observable is distinct.
        /// </summary>
        /// <param name="observable"></param>
        /// <param name="scheduler"></param>
        /// <param name="timeSpan">The minimum time the base observable needs to be true before true is emitted in the resulting observable.</param>
        public static IObservable<bool> WhenTrueFor(
            this IObservable<bool> observable,
            IScheduler scheduler,
            TimeSpan timeSpan)
        {
            ArgumentNullException.ThrowIfNull(observable);
            ArgumentNullException.ThrowIfNull(scheduler);

            if (timeSpan <= TimeSpan.Zero)
            {
                return observable;
            }

            var falseObservable = observable.Where(b => !b);

            var initialFalseObservable = observable
                .Take(1)
                .Where(b => b) // Only when immediately true, we need to publish a false in the beginning as falseObservable won't provide it for us.
                .Select(_ => false);

            var delayedTrueObservable = observable
                .DistinctUntilChanged() // Filter out consecutive values to prevent the timer from being reset.
                .Throttle(timeSpan, scheduler) // This will publish the value after x time. Note that we also pass "false" values here, to prevent a delayed "true" from being released.
                .Where(b => b); // As a "false" is also passed to throttle, we filter it out here.

            return falseObservable
                .Merge(initialFalseObservable)
                .Merge(delayedTrueObservable)
                .DistinctUntilChanged();
        }

        /// <summary>
        /// Returns an observable that stays true for a maximum of <paramref name="timeSpan"/>. If the base <paramref name="observable"/> emits false before the time has passed, the resulting observable also emits false.
        /// Resulting observable is distinct.
        /// </summary>
        public static IObservable<bool> LimitTrueDuration(this IObservable<bool> observable, IScheduler scheduler, TimeSpan timeSpan)
        {
            var falseWhenTrueFor = observable
                .WhenTrueFor(scheduler, timeSpan)
                .Where(b => b)
                .Select(_ => false);

            return observable.Merge(falseWhenTrueFor).DistinctUntilChanged();
        }
    }
}

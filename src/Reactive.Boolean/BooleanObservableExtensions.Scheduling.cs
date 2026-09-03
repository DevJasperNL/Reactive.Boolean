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
        /// <param name="resetTimerOnConsecutiveTrue">If "true", every "true" that is emitted by <paramref name="source"/> will reset the timer. A "true" that follows a "false" always (re)starts the timer.</param>
        /// <param name="completionBehavior">Determines what happens when <paramref name="source"/> completes while a "false" is being withheld: drop it and complete immediately (default), or emit it once the timer runs out and complete afterwards.</param>
        /// <returns></returns>
        public static IObservable<bool> TrueForAtLeast(
            this IObservable<bool> source,
            TimeSpan timeSpan,
            IScheduler scheduler,
            bool distinctUntilChanged = true,
            bool resetTimerOnConsecutiveTrue = false,
            CompletionBehavior completionBehavior = CompletionBehavior.CompleteImmediately)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(scheduler);
            if (timeSpan <= TimeSpan.Zero)
            {
                return source;
            }

            return Observable.Create<bool>(observer =>
                new TrueForAtLeastOperator(observer, timeSpan, scheduler, distinctUntilChanged, resetTimerOnConsecutiveTrue, completionBehavior)
                    .Run(source));
        }

        /// <summary>
        /// Returns an observable that won't emit "true" for at least <paramref name="timeSpan"/> after an initial "false" is emitted by <paramref name="source"/>.
        /// If a "true" is emitted during the <paramref name="timeSpan"/>, it will be emitted immediately after the timer is completed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="timeSpan"></param>
        /// <param name="scheduler"></param>
        /// <param name="distinctUntilChanged">If set to "false", the resulting observable will not be distinct. Both consecutive "true" and "false" values will be emitted. Note that consecutive "true" values that occur during the timer, will only be emitted as a single "true" once the timer runs out.</param>
        /// <param name="resetTimerOnConsecutiveFalse">If "true", every "false" that is emitted by <paramref name="source"/> will reset the timer. A "false" that follows a "true" always (re)starts the timer.</param>
        /// <param name="completionBehavior">Determines what happens when <paramref name="source"/> completes while a "true" is being withheld: drop it and complete immediately (default), or emit it once the timer runs out and complete afterwards.</param>
        /// <returns></returns>
        public static IObservable<bool> FalseForAtLeast(
            this IObservable<bool> source,
            TimeSpan timeSpan,
            IScheduler scheduler,
            bool distinctUntilChanged = true,
            bool resetTimerOnConsecutiveFalse = false,
            CompletionBehavior completionBehavior = CompletionBehavior.CompleteImmediately) =>
            source
                .Not()
                .TrueForAtLeast(timeSpan, scheduler, distinctUntilChanged, resetTimerOnConsecutiveFalse, completionBehavior)
                .Not();

        /// <summary>
        /// Returns an observable that delays the first "false" that is emitted after a "true" by <paramref name="source"/> for a duration of <paramref name="timeSpan"/>.
        /// A "true" emitted during that time cancels the delayed "false".
        /// </summary>
        /// <param name="source"></param>
        /// <param name="timeSpan"></param>
        /// <param name="scheduler"></param>
        /// <param name="resetTimerOnConsecutiveFalse">If "true", every "false" that is emitted by <paramref name="source"/> while the timer runs will reset the timer.</param>
        /// <param name="distinctUntilChanged">If set to "false", the resulting observable will not be distinct. Both consecutive "true" and "false" values will be emitted. Note that consecutive "false" values that occur during the timer, will only be emitted as a single "false" once the timer runs out.</param>
        /// <param name="completionBehavior">Determines what happens when <paramref name="source"/> completes while a "false" is being delayed: drop it and complete immediately (default), or emit it once the timer runs out and complete afterwards.</param>
        /// <returns></returns>
        public static IObservable<bool> PersistTrueFor(
            this IObservable<bool> source,
            TimeSpan timeSpan,
            IScheduler scheduler,
            bool resetTimerOnConsecutiveFalse = false,
            bool distinctUntilChanged = true,
            CompletionBehavior completionBehavior = CompletionBehavior.CompleteImmediately)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(scheduler);
            if (timeSpan <= TimeSpan.Zero)
            {
                return source;
            }

            return Observable.Create<bool>(observer =>
                new PersistTrueForOperator(observer, timeSpan, scheduler, distinctUntilChanged, resetTimerOnConsecutiveFalse, completionBehavior)
                    .Run(source));
        }

        /// <summary>
        /// Returns an observable that delays the first "true" that is emitted after a "false" by <paramref name="source"/> for a duration of <paramref name="timeSpan"/>.
        /// A "false" emitted during that time cancels the delayed "true".
        /// </summary>
        /// <param name="source"></param>
        /// <param name="timeSpan"></param>
        /// <param name="scheduler"></param>
        /// <param name="resetTimerOnConsecutiveTrue">If "true", every "true" that is emitted by <paramref name="source"/> while the timer runs will reset the timer.</param>
        /// <param name="distinctUntilChanged">If set to "false", the resulting observable will not be distinct. Both consecutive "true" and "false" values will be emitted. Note that consecutive "true" values that occur during the timer, will only be emitted as a single "true" once the timer runs out.</param>
        /// <param name="completionBehavior">Determines what happens when <paramref name="source"/> completes while a "true" is being delayed: drop it and complete immediately (default), or emit it once the timer runs out and complete afterwards.</param>
        /// <returns></returns>
        public static IObservable<bool> PersistFalseFor(
            this IObservable<bool> source,
            TimeSpan timeSpan,
            IScheduler scheduler,
            bool resetTimerOnConsecutiveTrue = false,
            bool distinctUntilChanged = true,
            CompletionBehavior completionBehavior = CompletionBehavior.CompleteImmediately) =>
            source
                .Not()
                .PersistTrueFor(timeSpan, scheduler, resetTimerOnConsecutiveTrue, distinctUntilChanged, completionBehavior)
                .Not();

        /// <summary>
        /// Returns an observable that emits "true" once <paramref name="source"/> does not emit "false" for a minimum of <paramref name="timeSpan"/>.
        /// The resulting observable emits "false" for the first value of <paramref name="source"/> and for every "false".
        /// </summary>
        /// <param name="source"></param>
        /// <param name="timeSpan"></param>
        /// <param name="scheduler"></param>
        /// <param name="resetTimerOnConsecutiveTrue">If "true", every "true" that is emitted by <paramref name="source"/> while the timer runs will reset the timer.</param>
        /// <param name="distinctUntilChanged">If set to "false", the resulting observable will not be distinct. Consecutive "false" values are emitted, as are consecutive "true" values received after the timer ran out. "true" values received while the timer runs are not emitted.</param>
        /// <param name="completionBehavior">Determines what happens when <paramref name="source"/> completes while the timer runs: complete immediately without emitting "true" (default), or emit "true" once the timer runs out and complete afterwards.</param>
        public static IObservable<bool> WhenTrueFor(
            this IObservable<bool> source,
            TimeSpan timeSpan,
            IScheduler scheduler,
            bool resetTimerOnConsecutiveTrue = false,
            bool distinctUntilChanged = true,
            CompletionBehavior completionBehavior = CompletionBehavior.CompleteImmediately)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(scheduler);
            if (timeSpan <= TimeSpan.Zero)
            {
                return source;
            }

            return Observable.Create<bool>(observer =>
                new WhenTrueForOperator(observer, timeSpan, scheduler, distinctUntilChanged, resetTimerOnConsecutiveTrue, completionBehavior)
                    .Run(source));
        }

        /// <summary>
        /// Returns an observable that emits "false" once <paramref name="source"/> does not emit "true" for a minimum of <paramref name="timeSpan"/>.
        /// The resulting observable emits "true" for the first value of <paramref name="source"/> and for every "true".
        /// </summary>
        /// <param name="source"></param>
        /// <param name="timeSpan"></param>
        /// <param name="scheduler"></param>
        /// <param name="resetTimerOnConsecutiveFalse">If "true", every "false" that is emitted by <paramref name="source"/> while the timer runs will reset the timer.</param>
        /// <param name="distinctUntilChanged">If set to "false", the resulting observable will not be distinct. Consecutive "true" values are emitted, as are consecutive "false" values received after the timer ran out. "false" values received while the timer runs are not emitted.</param>
        /// <param name="completionBehavior">Determines what happens when <paramref name="source"/> completes while the timer runs: complete immediately without emitting "false" (default), or emit "false" once the timer runs out and complete afterwards.</param>
        public static IObservable<bool> WhenFalseFor(
            this IObservable<bool> source,
            TimeSpan timeSpan,
            IScheduler scheduler,
            bool resetTimerOnConsecutiveFalse = false,
            bool distinctUntilChanged = true,
            CompletionBehavior completionBehavior = CompletionBehavior.CompleteImmediately) =>
            source
                .Not()
                .WhenTrueFor(timeSpan, scheduler, resetTimerOnConsecutiveFalse, distinctUntilChanged, completionBehavior)
                .Not();

        /// <summary>
        /// Returns an observable that will automatically emit "false" if <paramref name="source"/> does not emit a "false" itself within <paramref name="timeSpan"/> after emitting "true".
        /// Once the limit is reached, a repeated "true" from <paramref name="source"/> is ignored until it emits "false" again, unless <paramref name="resetTimerOnConsecutiveTrue"/> is set.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="timeSpan"></param>
        /// <param name="scheduler"></param>
        /// <param name="distinctUntilChanged">If set to "false", the resulting observable will not be distinct. Both consecutive "true" and "false" values will be emitted.</param>
        /// <param name="resetTimerOnConsecutiveTrue">If "true", every "true" that is emitted by <paramref name="source"/> will reset the timer. A "true" received after the limit was reached then re-arms the limit and is emitted again.</param>
        /// <param name="completionBehavior">Determines what happens when <paramref name="source"/> completes while the timer runs: complete immediately without emitting the limiting "false" (default), or emit it once the timer runs out and complete afterwards.</param>
        /// <returns></returns>
        public static IObservable<bool> LimitTrueDuration(
            this IObservable<bool> source,
            TimeSpan timeSpan,
            IScheduler scheduler,
            bool distinctUntilChanged = true,
            bool resetTimerOnConsecutiveTrue = false,
            CompletionBehavior completionBehavior = CompletionBehavior.CompleteImmediately)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(scheduler);

            return Observable.Create<bool>(observer =>
                new LimitTrueDurationOperator(observer, timeSpan, scheduler, distinctUntilChanged, resetTimerOnConsecutiveTrue, completionBehavior)
                    .Run(source));
        }

        /// <summary>
        /// Returns an observable that will automatically emit "true" if <paramref name="source"/> does not emit a "true" itself within <paramref name="timeSpan"/> after emitting "false".
        /// Once the limit is reached, a repeated "false" from <paramref name="source"/> is ignored until it emits "true" again, unless <paramref name="resetTimerOnConsecutiveFalse"/> is set.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="timeSpan"></param>
        /// <param name="scheduler"></param>
        /// <param name="distinctUntilChanged">If set to "false", the resulting observable will not be distinct. Both consecutive "true" and "false" values will be emitted.</param>
        /// <param name="resetTimerOnConsecutiveFalse">If "true", every "false" that is emitted by <paramref name="source"/> will reset the timer. A "false" received after the limit was reached then re-arms the limit and is emitted again.</param>
        /// <param name="completionBehavior">Determines what happens when <paramref name="source"/> completes while the timer runs: complete immediately without emitting the limiting "true" (default), or emit it once the timer runs out and complete afterwards.</param>
        /// <returns></returns>
        public static IObservable<bool> LimitFalseDuration(
            this IObservable<bool> source,
            TimeSpan timeSpan,
            IScheduler scheduler,
            bool distinctUntilChanged = true,
            bool resetTimerOnConsecutiveFalse = false,
            CompletionBehavior completionBehavior = CompletionBehavior.CompleteImmediately) =>
            source
                .Not()
                .LimitTrueDuration(timeSpan, scheduler, distinctUntilChanged, resetTimerOnConsecutiveFalse, completionBehavior)
                .Not();
    }
}

using System.Reactive.Linq;

namespace Reactive.Boolean
{
    internal static class ObservableExtensions
    {
        /// <summary>
        /// Returns an observable that represents pairs of previous and current values of the source observable. First pair that is emitted will use <paramref name="seed"/> as previous value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        internal static IObservable<(T TPrevious, T TCurrent)> Pairwise<T>(this IObservable<T> source, T seed)
        {
            return source
                .StartWith(seed) // Prepend the seed value
                .Buffer(2, 1) // Create overlapping pairs
                .Where(pair => pair.Count == 2) // Prevent completing the observable from emitting a single item
                .Select(pair => (pair[0], pair[1])); // Convert to (Previous, Current) tuple
        }
    }
}

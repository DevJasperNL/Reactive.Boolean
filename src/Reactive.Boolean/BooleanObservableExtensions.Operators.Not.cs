using System.Reactive.Linq;

namespace Reactive.Boolean
{
    public static partial class BooleanObservableExtensions
    {
        /// <summary>
        /// Returns an observable in which the input is inverted.
        /// (returns observable.Select(b => !b))
        /// </summary>
        public static IObservable<bool> Not(this IObservable<bool> source)
        {
            ArgumentNullException.ThrowIfNull(source);

            return source.Select(b => !b);
        }
    }
}

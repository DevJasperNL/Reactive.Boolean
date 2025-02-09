using System.Reactive.Linq;

namespace Reactive.Boolean
{
    public static partial class BooleanObservableExtensions
    {
        /// <summary>
        /// Returns an observable that combines the latest results of two observables using an XOR operator.
        /// </summary>
        /// <param name="observable1"></param>
        /// <param name="observable2"></param>
        /// <param name="distinctUntilChanged">When true, DistinctUntilChanged is applied to the result.</param>
        public static IObservable<bool> Xor(
            this IObservable<bool> observable1,
            IObservable<bool> observable2,
            bool distinctUntilChanged = true)
        {
            ArgumentNullException.ThrowIfNull(observable1);
            ArgumentNullException.ThrowIfNull(observable2);

            if (distinctUntilChanged)
            {
                return observable1
                    .CombineLatest(observable2, (o1, o2) => o1 ^ o2)
                    .DistinctUntilChanged();
            }

            return observable1
                .CombineLatest(observable2, (o1, o2) => o1 ^ o2);
        }

        /// <summary>
        /// Returns an observable that combines the latest results of two observables using an XNOR operator.
        /// </summary>
        /// <param name="observable1"></param>
        /// <param name="observable2"></param>
        /// <param name="distinctUntilChanged">When true, DistinctUntilChanged is applied to the result.</param>
        public static IObservable<bool> Xnor(
            this IObservable<bool> observable1,
            IObservable<bool> observable2,
            bool distinctUntilChanged = true) =>
            observable1.Xor(observable2, distinctUntilChanged).Not();
    }
}

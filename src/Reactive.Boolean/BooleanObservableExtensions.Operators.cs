using System.Reactive.Linq;

namespace Reactive.Boolean
{
    public static partial class BooleanObservableExtensions
    {
        /// <summary>
        /// Returns an observable in which the input is inverted.
        /// (returns observable.Select(b => !b))
        /// </summary>
        public static IObservable<bool> Not(this IObservable<bool> observable)
        {
            ArgumentNullException.ThrowIfNull(observable);

            return observable.Select(b => !b);
        }

        /// <summary>
        /// Returns an observable that combines the latest results of two observables using an AND operator.
        /// Note: This method is an alias for "And". It is created to prevent namespace conflicts with the "And" method in reactive joins.
        /// </summary>
        /// <param name="observable1"></param>
        /// <param name="observable2"></param>
        /// <param name="distinctUntilChanged">When true, DistinctUntilChanged is applied, meaning that false cannot occur multiple times in a row.</param>
        public static IObservable<bool> AndOp(
            this IObservable<bool> observable1,
            IObservable<bool> observable2,
            bool distinctUntilChanged = true) => And(observable1, observable2, distinctUntilChanged);

        /// <summary>
        /// Returns an observable that combines the latest results of two observables using an AND operator.
        /// </summary>
        /// <param name="observable1"></param>
        /// <param name="observable2"></param>
        /// <param name="distinctUntilChanged">When true, DistinctUntilChanged is applied to the result. Note that without using DistinctUntilChanged, false can occur multiple times in a row even when the input observables are distinct.</param>
        public static IObservable<bool> And(
            this IObservable<bool> observable1,
            IObservable<bool> observable2,
            bool distinctUntilChanged = true)
        {
            ArgumentNullException.ThrowIfNull(observable1);
            ArgumentNullException.ThrowIfNull(observable2);

            if (distinctUntilChanged)
            {
                return
                observable1
                .CombineLatest(observable2, (o1, o2) => o1 && o2)
                        .DistinctUntilChanged();
            }

            return observable1
                .CombineLatest(observable2, (o1, o2) => o1 && o2);
        }

        /// <summary>
        /// Returns an observable that combines the latest results of two observables using an OR operator.
        /// </summary>
        /// <param name="observable1"></param>
        /// <param name="observable2"></param>
        /// <param name="distinctUntilChanged">When true, DistinctUntilChanged is applied to the result. Note that without using DistinctUntilChanged, true can occur multiple times in a row even when the input observables are distinct.</param>
        public static IObservable<bool> Or(
            this IObservable<bool> observable1,
            IObservable<bool> observable2,
            bool distinctUntilChanged = true)
        {
            ArgumentNullException.ThrowIfNull(observable1);
            ArgumentNullException.ThrowIfNull(observable2);

            if (distinctUntilChanged)
            {
                return observable1
                    .CombineLatest(observable2, (o1, o2) => o1 || o2)
                    .DistinctUntilChanged();
            }

            return observable1
                .CombineLatest(observable2, (o1, o2) => o1 || o2);
        }

        /// <summary>
        /// Returns an observable that combines the latest results of two observables using an XOR operator.
        /// </summary>
        /// <param name="observable1"></param>
        /// <param name="observable2"></param>
        /// <param name="distinctUntilChanged">When true, DistinctUntilChanged is applied to the result.</param>
        public static IObservable<bool> XOr(
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
    }
}

using System.Reactive.Linq;

namespace Reactive.Boolean
{
    public static partial class BooleanObservableExtensions
    {
        /// <summary>
        /// Returns an observable that combines the latest results of two observables using an OR operator.
        /// </summary>
        /// <param name="observable1"></param>
        /// <param name="observable2"></param>
        /// <param name="operatorDistinctness">
        ///   <para>OutputDistinctUntilChanged: DistinctUntilChanged is applied to the returned observable, meaning a "true" can only be followed by a "false" and vice versa.</para>
        ///   <para>InputDistinctUntilChanged: DistinctUntilChanged is applied to the inputs only. Meaning that consecutive values on the input do not change the output, but input changes on different inputs can. For example, going from "true", "false" to "true", "true" will emit consecutive "true" values.</para>
        ///   <para>NotDistinct: DistinctUntilChanged is never applied. Meaning both consecutive input and output values will be emitted.</para>
        /// </param>
        public static IObservable<bool> Or(
            this IObservable<bool> observable1,
            IObservable<bool> observable2,
            OperatorDistinctness operatorDistinctness = OperatorDistinctness.OutputDistinctUntilChanged)
        {
            ArgumentNullException.ThrowIfNull(observable1);
            ArgumentNullException.ThrowIfNull(observable2);

            if (operatorDistinctness == OperatorDistinctness.InputDistinctUntilChanged)
            {
                observable1 = observable1.DistinctUntilChanged();
                observable2 = observable2.DistinctUntilChanged();
            }

            if (operatorDistinctness == OperatorDistinctness.OutputDistinctUntilChanged)
            {
                return observable1
                    .CombineLatest(observable2, (o1, o2) => o1 || o2)
                    .DistinctUntilChanged();
            }

            return observable1
                .CombineLatest(observable2, (o1, o2) => o1 || o2);
        }

        /// <summary>
        /// Returns an observable that combines the latest results of two observables using an OR operator.
        /// (Uses source.CombineLatest(values => values.Any(v => v)))
        /// </summary>
        /// <param name="source"></param>
        /// <param name="operatorDistinctness">
        ///   <para>OutputDistinctUntilChanged: DistinctUntilChanged is applied to the returned observable, meaning a "true" can only be followed by a "false" and vice versa.</para>
        ///   <para>InputDistinctUntilChanged: DistinctUntilChanged is applied to the inputs only. Meaning that consecutive values on the input do not change the output, but input changes on different inputs can. For example, going from "true", "false" to "true", "true" will emit consecutive "true" values.</para>
        ///   <para>NotDistinct: DistinctUntilChanged is never applied. Meaning both consecutive input and output values will be emitted.</para>
        /// </param>
        public static IObservable<bool> Or(
            this IEnumerable<IObservable<bool>> source, 
            OperatorDistinctness operatorDistinctness = OperatorDistinctness.OutputDistinctUntilChanged)
        {
            ArgumentNullException.ThrowIfNull(source);

            if (operatorDistinctness == OperatorDistinctness.InputDistinctUntilChanged)
            {
                source = source.Select(o => o.DistinctUntilChanged());
            }
            if (operatorDistinctness == OperatorDistinctness.OutputDistinctUntilChanged)
            {
                return source
                    .CombineLatest(values => values.Any(v => v))
                    .DistinctUntilChanged();
            }
            return source.CombineLatest(values => values.Any(v => v));
        }

        /// <summary>
        /// Returns an observable that combines the latest results of two observables using an OR operator.
        /// (Uses source.CombineLatest(values => values.Any(v => v)) on an array made from all observables)
        /// </summary>
        /// <param name="observable"></param>
        /// <param name="observables"></param>
        /// <param name="operatorDistinctness">
        ///   <para>OutputDistinctUntilChanged: DistinctUntilChanged is applied to the returned observable, meaning a "true" can only be followed by a "false" and vice versa.</para>
        ///   <para>InputDistinctUntilChanged: DistinctUntilChanged is applied to the inputs only. Meaning that consecutive values on the input do not change the output, but input changes on different inputs can. For example, going from "true", "false" to "true", "true" will emit consecutive "true" values.</para>
        ///   <para>NotDistinct: DistinctUntilChanged is never applied. Meaning both consecutive input and output values will be emitted.</para>
        /// </param>
        public static IObservable<bool> Or(
            this IObservable<bool> observable,
            IEnumerable<IObservable<bool>> observables,
            OperatorDistinctness operatorDistinctness = OperatorDistinctness.OutputDistinctUntilChanged) =>
            new[] { observable }.Concat(observables).Or(operatorDistinctness);

        /// <summary>
        /// Returns an observable that combines the latest results of two observables using an OR operator.
        /// (Uses source.CombineLatest(values => values.Any(v => v)) on an array made from all observables)
        /// operatorDistinctness will be OutputDistinctUntilChanged.
        /// </summary>
        public static IObservable<bool> Or(
            this IObservable<bool> observable,
            params IObservable<bool>[] observables) =>
            new[] { observable }.Concat(observables).Or();

        /// <summary>
        /// Returns an observable that combines the latest results of two observables using an OR operator.
        /// (Uses source.CombineLatest(values => values.Any(v => v)) on an array made from all observables)
        /// </summary>
        /// <param name="observable1"></param>
        /// <param name="observable2"></param>
        /// <param name="observable3"></param>
        /// <param name="operatorDistinctness">
        ///   <para>OutputDistinctUntilChanged: DistinctUntilChanged is applied to the returned observable, meaning a "true" can only be followed by a "false" and vice versa.</para>
        ///   <para>InputDistinctUntilChanged: DistinctUntilChanged is applied to the inputs only. Meaning that consecutive values on the input do not change the output, but input changes on different inputs can. For example, going from "true", "false" to "true", "true" will emit consecutive "true" values.</para>
        ///   <para>NotDistinct: DistinctUntilChanged is never applied. Meaning both consecutive input and output values will be emitted.</para>
        /// </param>
        public static IObservable<bool> Or(
            this IObservable<bool> observable1,
            IObservable<bool> observable2,
            IObservable<bool> observable3,
            OperatorDistinctness operatorDistinctness) =>
            new[] { observable1, observable2, observable3 }.Or(operatorDistinctness);

        /// <summary>
        /// Returns an observable that combines the latest results of two observables using an OR operator.
        /// (Uses source.CombineLatest(values => values.Any(v => v)) on an array made from all observables)
        /// </summary>
        /// <param name="observable1"></param>
        /// <param name="observable2"></param>
        /// <param name="observable3"></param>
        /// <param name="observable4"></param>
        /// <param name="operatorDistinctness">
        ///   <para>OutputDistinctUntilChanged: DistinctUntilChanged is applied to the returned observable, meaning a "true" can only be followed by a "false" and vice versa.</para>
        ///   <para>InputDistinctUntilChanged: DistinctUntilChanged is applied to the inputs only. Meaning that consecutive values on the input do not change the output, but input changes on different inputs can. For example, going from "true", "false" to "true", "true" will emit consecutive "true" values.</para>
        ///   <para>NotDistinct: DistinctUntilChanged is never applied. Meaning both consecutive input and output values will be emitted.</para>
        /// </param>
        public static IObservable<bool> Or(
            this IObservable<bool> observable1,
            IObservable<bool> observable2,
            IObservable<bool> observable3,
            IObservable<bool> observable4,
            OperatorDistinctness operatorDistinctness) =>
            new[] { observable1, observable2, observable3, observable4 }.Or(operatorDistinctness);

        /// <summary>
        /// Returns an observable that combines the latest results of two observables using an NOR operator.
        /// </summary>
        /// <param name="observable1"></param>
        /// <param name="observable2"></param>
        /// <param name="operatorDistinctness">
        ///   <para>OutputDistinctUntilChanged: DistinctUntilChanged is applied to the returned observable, meaning a "true" can only be followed by a "false" and vice versa.</para>
        ///   <para>InputDistinctUntilChanged: DistinctUntilChanged is applied to the inputs only. Meaning that consecutive values on the input do not change the output, but input changes on different inputs can. For example, going from "true", "false" to "true", "true" will emit consecutive "false" values.</para>
        ///   <para>NotDistinct: DistinctUntilChanged is never applied. Meaning both consecutive input and output values will be emitted.</para>
        /// </param>
        public static IObservable<bool> Nor(
            this IObservable<bool> observable1,
            IObservable<bool> observable2,
            OperatorDistinctness operatorDistinctness = OperatorDistinctness.OutputDistinctUntilChanged) =>
            observable1.Or(observable2, operatorDistinctness).Not();

        /// <summary>
        /// Returns an observable that combines the latest results of two observables using an NOR operator.
        /// (Uses source.CombineLatest(values => values.Any(v => v)).Not())
        /// </summary>
        /// <param name="source"></param>
        /// <param name="operatorDistinctness">
        ///   <para>OutputDistinctUntilChanged: DistinctUntilChanged is applied to the returned observable, meaning a "true" can only be followed by a "false" and vice versa.</para>
        ///   <para>InputDistinctUntilChanged: DistinctUntilChanged is applied to the inputs only. Meaning that consecutive values on the input do not change the output, but input changes on different inputs can. For example, going from "true", "false" to "true", "true" will emit consecutive "false" values.</para>
        ///   <para>NotDistinct: DistinctUntilChanged is never applied. Meaning both consecutive input and output values will be emitted.</para>
        /// </param>
        public static IObservable<bool> Nor(
            this IEnumerable<IObservable<bool>> source,
            OperatorDistinctness operatorDistinctness = OperatorDistinctness.OutputDistinctUntilChanged) =>
            source.Or(operatorDistinctness).Not();

        /// <summary>
        /// Returns an observable that combines the latest results of two observables using an NOR operator.
        /// (Uses source.CombineLatest(values => values.Any(v => v)).Not() on an array made from all observables)
        /// </summary>
        /// <param name="observable"></param>
        /// <param name="observables"></param>
        /// <param name="operatorDistinctness">
        ///   <para>OutputDistinctUntilChanged: DistinctUntilChanged is applied to the returned observable, meaning a "true" can only be followed by a "false" and vice versa.</para>
        ///   <para>InputDistinctUntilChanged: DistinctUntilChanged is applied to the inputs only. Meaning that consecutive values on the input do not change the output, but input changes on different inputs can. For example, going from "true", "false" to "true", "true" will emit consecutive "false" values.</para>
        ///   <para>NotDistinct: DistinctUntilChanged is never applied. Meaning both consecutive input and output values will be emitted.</para>
        /// </param>
        public static IObservable<bool> Nor(
            this IObservable<bool> observable,
            IEnumerable<IObservable<bool>> observables,
            OperatorDistinctness operatorDistinctness = OperatorDistinctness.OutputDistinctUntilChanged) =>
            observable.Or(observables, operatorDistinctness).Not();

        /// <summary>
        /// Returns an observable that combines the latest results of two observables using an NOR operator.
        /// (Uses source.CombineLatest(values => values.Any(v => v)).Not() on an array made from all observables)
        /// operatorDistinctness will be OutputDistinctUntilChanged.
        /// </summary>
        public static IObservable<bool> Nor(
            this IObservable<bool> observable,
            params IObservable<bool>[] observables) =>
            observable.Or(observables).Not();

        /// <summary>
        /// Returns an observable that combines the latest results of two observables using an NOR operator.
        /// (Uses source.CombineLatest(values => values.Any(v => v)).Not() on an array made from all observables)
        /// </summary>
        /// <param name="observable1"></param>
        /// <param name="observable2"></param>
        /// <param name="observable3"></param>
        /// <param name="operatorDistinctness">
        ///   <para>OutputDistinctUntilChanged: DistinctUntilChanged is applied to the returned observable, meaning a "true" can only be followed by a "false" and vice versa.</para>
        ///   <para>InputDistinctUntilChanged: DistinctUntilChanged is applied to the inputs only. Meaning that consecutive values on the input do not change the output, but input changes on different inputs can. For example, going from "true", "false" to "true", "true" will emit consecutive "false" values.</para>
        ///   <para>NotDistinct: DistinctUntilChanged is never applied. Meaning both consecutive input and output values will be emitted.</para>
        /// </param>
        public static IObservable<bool> Nor(
            this IObservable<bool> observable1,
            IObservable<bool> observable2,
            IObservable<bool> observable3,
            OperatorDistinctness operatorDistinctness) =>
            new[] { observable1, observable2, observable3 }.Or(operatorDistinctness);

        /// <summary>
        /// Returns an observable that combines the latest results of two observables using an NOR operator.
        /// (Uses source.CombineLatest(values => values.Any(v => v)).Not() on an array made from all observables)
        /// </summary>
        /// <param name="observable1"></param>
        /// <param name="observable2"></param>
        /// <param name="observable3"></param>
        /// <param name="observable4"></param>
        /// <param name="operatorDistinctness">
        ///   <para>OutputDistinctUntilChanged: DistinctUntilChanged is applied to the returned observable, meaning a "true" can only be followed by a "false" and vice versa.</para>
        ///   <para>InputDistinctUntilChanged: DistinctUntilChanged is applied to the inputs only. Meaning that consecutive values on the input do not change the output, but input changes on different inputs can. For example, going from "true", "false" to "true", "true" will emit consecutive "false" values.</para>
        ///   <para>NotDistinct: DistinctUntilChanged is never applied. Meaning both consecutive input and output values will be emitted.</para>
        /// </param>
        public static IObservable<bool> Nor(
            this IObservable<bool> observable1,
            IObservable<bool> observable2,
            IObservable<bool> observable3,
            IObservable<bool> observable4,
            OperatorDistinctness operatorDistinctness) =>
            new[] { observable1, observable2, observable3, observable4 }.Or(operatorDistinctness);
    }
}

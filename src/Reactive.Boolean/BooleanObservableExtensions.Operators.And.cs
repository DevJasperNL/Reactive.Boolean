using System.Reactive.Linq;

namespace Reactive.Boolean
{
    /// <summary>
    /// Extension methods for <see cref="IObservable{Boolean}"/>.
    /// </summary>
    public static partial class BooleanObservableExtensions
    {
        /// <summary>
        /// Returns an observable that combines the latest results of two observables using an AND operator.
        /// </summary>
        /// <param name="observable1"></param>
        /// <param name="observable2"></param>
        /// <param name="operatorDistinctness">
        ///   <para>OutputDistinctUntilChanged: DistinctUntilChanged is applied to the returned observable, meaning a "true" can only be followed by a "false" and vice versa.</para>
        ///   <para>InputDistinctUntilChanged: DistinctUntilChanged is applied to the inputs only. Meaning that consecutive values on the input do not change the output, but input changes on different inputs can. For example, going from "false", "false" to "true", "false" will emit consecutive "false" values.</para>
        ///   <para>NotDistinct: DistinctUntilChanged is never applied. Meaning both consecutive input and output values will be emitted.</para>
        /// </param>
        public static IObservable<bool> And(
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
                return
                    observable1
                        .CombineLatest(observable2, (o1, o2) => o1 && o2)
                        .DistinctUntilChanged();
            }

            return observable1
                .CombineLatest(observable2, (o1, o2) => o1 && o2);
        }

        /// <summary>
        /// Returns an observable that combines the latest results of all observables using an AND operator.
        /// (Uses source.CombineLatest(values => values.All(v => v)))
        /// </summary>
        /// <param name="source"></param>
        /// <param name="operatorDistinctness">
        ///   <para>OutputDistinctUntilChanged: DistinctUntilChanged is applied to the returned observable, meaning a "true" can only be followed by a "false" and vice versa.</para>
        ///   <para>InputDistinctUntilChanged: DistinctUntilChanged is applied to the inputs only. Meaning that consecutive values on the input do not change the output, but input changes on different inputs can. For example, going from "false", "false" to "true", "false" will emit consecutive "false" values.</para>
        ///   <para>NotDistinct: DistinctUntilChanged is never applied. Meaning both consecutive input and output values will be emitted.</para>
        /// </param>
        public static IObservable<bool> And(
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
                    .CombineLatest(values => values.All(v => v))
                    .DistinctUntilChanged();
            }
            return source.CombineLatest(values => values.All(v => v));
        }

        /// <summary>
        /// Returns an observable that combines the latest results of all observables using an AND operator.
        /// (Uses source.CombineLatest(values => values.All(v => v)) on an array made from all observables)
        /// </summary>
        /// <param name="observable"></param>
        /// <param name="observables"></param>
        /// <param name="operatorDistinctness">
        ///   <para>OutputDistinctUntilChanged: DistinctUntilChanged is applied to the returned observable, meaning a "true" can only be followed by a "false" and vice versa.</para>
        ///   <para>InputDistinctUntilChanged: DistinctUntilChanged is applied to the inputs only. Meaning that consecutive values on the input do not change the output, but input changes on different inputs can. For example, going from "false", "false" to "true", "false" will emit consecutive "false" values.</para>
        ///   <para>NotDistinct: DistinctUntilChanged is never applied. Meaning both consecutive input and output values will be emitted.</para>
        /// </param>
        public static IObservable<bool> And(
            this IObservable<bool> observable,
            IEnumerable<IObservable<bool>> observables,
            OperatorDistinctness operatorDistinctness = OperatorDistinctness.OutputDistinctUntilChanged) =>
            new[] { observable }.Concat(observables).And(operatorDistinctness);

        /// <summary>
        /// Returns an observable that combines the latest results of all observables using an AND operator.
        /// (Uses source.CombineLatest(values => values.All(v => v)) on an array made from all observables)
        /// operatorDistinctness will be OutputDistinctUntilChanged.
        /// </summary>
        public static IObservable<bool> And(
            this IObservable<bool> observable,
            params IObservable<bool>[] observables) =>
            new[] { observable }.Concat(observables).And();

        /// <summary>
        /// Returns an observable that combines the latest results of all observables using an AND operator.
        /// (Uses source.CombineLatest(values => values.All(v => v)) on an array made from all observables)
        /// </summary>
        /// <param name="observable1"></param>
        /// <param name="observable2"></param>
        /// <param name="observable3"></param>
        /// <param name="operatorDistinctness">
        ///   <para>OutputDistinctUntilChanged: DistinctUntilChanged is applied to the returned observable, meaning a "true" can only be followed by a "false" and vice versa.</para>
        ///   <para>InputDistinctUntilChanged: DistinctUntilChanged is applied to the inputs only. Meaning that consecutive values on the input do not change the output, but input changes on different inputs can. For example, going from "false", "false" to "true", "false" will emit consecutive "false" values.</para>
        ///   <para>NotDistinct: DistinctUntilChanged is never applied. Meaning both consecutive input and output values will be emitted.</para>
        /// </param>
        public static IObservable<bool> And(
            this IObservable<bool> observable1,
            IObservable<bool> observable2,
            IObservable<bool> observable3,
            OperatorDistinctness operatorDistinctness) =>
            new[] { observable1, observable2, observable3 }.And(operatorDistinctness);

        /// <summary>
        /// Returns an observable that combines the latest results of all observables using an AND operator.
        /// (Uses source.CombineLatest(values => values.All(v => v)) on an array made from all observables)
        /// </summary>
        /// <param name="observable1"></param>
        /// <param name="observable2"></param>
        /// <param name="observable3"></param>
        /// <param name="observable4"></param>
        /// <param name="operatorDistinctness">
        ///   <para>OutputDistinctUntilChanged: DistinctUntilChanged is applied to the returned observable, meaning a "true" can only be followed by a "false" and vice versa.</para>
        ///   <para>InputDistinctUntilChanged: DistinctUntilChanged is applied to the inputs only. Meaning that consecutive values on the input do not change the output, but input changes on different inputs can. For example, going from "false", "false" to "true", "false" will emit consecutive "false" values.</para>
        ///   <para>NotDistinct: DistinctUntilChanged is never applied. Meaning both consecutive input and output values will be emitted.</para>
        /// </param>
        public static IObservable<bool> And(
            this IObservable<bool> observable1,
            IObservable<bool> observable2,
            IObservable<bool> observable3,
            IObservable<bool> observable4,
            OperatorDistinctness operatorDistinctness) =>
            new[] { observable1, observable2, observable3, observable4 }.And(operatorDistinctness);

        /// <summary>
        /// Returns an observable that combines the latest results of two observables using an NAND operator.
        /// </summary>
        /// <param name="observable1"></param>
        /// <param name="observable2"></param>
        /// <param name="operatorDistinctness">
        ///   <para>OutputDistinctUntilChanged: DistinctUntilChanged is applied to the returned observable, meaning a "true" can only be followed by a "false" and vice versa.</para>
        ///   <para>InputDistinctUntilChanged: DistinctUntilChanged is applied to the inputs only. Meaning that consecutive values on the input do not change the output, but input changes on different inputs can. For example, going from "false", "false" to "true", "false" will emit consecutive "true" values.</para>
        ///   <para>NotDistinct: DistinctUntilChanged is never applied. Meaning both consecutive input and output values will be emitted.</para>
        /// </param>
        public static IObservable<bool> Nand(
            this IObservable<bool> observable1,
            IObservable<bool> observable2,
            OperatorDistinctness operatorDistinctness = OperatorDistinctness.OutputDistinctUntilChanged) =>
            observable1.And(observable2, operatorDistinctness).Not();

        /// <summary>
        /// Returns an observable that combines the latest results of all observables using an NAND operator.
        /// (Uses source.CombineLatest(values => values.All(v => v)).Not())
        /// </summary>
        /// <param name="source"></param>
        /// <param name="operatorDistinctness">
        ///   <para>OutputDistinctUntilChanged: DistinctUntilChanged is applied to the returned observable, meaning a "true" can only be followed by a "false" and vice versa.</para>
        ///   <para>InputDistinctUntilChanged: DistinctUntilChanged is applied to the inputs only. Meaning that consecutive values on the input do not change the output, but input changes on different inputs can. For example, going from "false", "false" to "true", "false" will emit consecutive "true" values.</para>
        ///   <para>NotDistinct: DistinctUntilChanged is never applied. Meaning both consecutive input and output values will be emitted.</para>
        /// </param>
        public static IObservable<bool> Nand(
            this IEnumerable<IObservable<bool>> source,
            OperatorDistinctness operatorDistinctness = OperatorDistinctness.OutputDistinctUntilChanged) =>
            source.And(operatorDistinctness).Not();

        /// <summary>
        /// Returns an observable that combines the latest results of all observables using an NAND operator.
        /// (Uses source.CombineLatest(values => values.All(v => v)).Not() on an array made from all observables)
        /// </summary>
        /// <param name="observable"></param>
        /// <param name="observables"></param>
        /// <param name="operatorDistinctness">
        ///   <para>OutputDistinctUntilChanged: DistinctUntilChanged is applied to the returned observable, meaning a "true" can only be followed by a "false" and vice versa.</para>
        ///   <para>InputDistinctUntilChanged: DistinctUntilChanged is applied to the inputs only. Meaning that consecutive values on the input do not change the output, but input changes on different inputs can. For example, going from "false", "false" to "true", "false" will emit consecutive "true" values.</para>
        ///   <para>NotDistinct: DistinctUntilChanged is never applied. Meaning both consecutive input and output values will be emitted.</para>
        /// </param>
        public static IObservable<bool> Nand(
            this IObservable<bool> observable,
            IEnumerable<IObservable<bool>> observables,
            OperatorDistinctness operatorDistinctness = OperatorDistinctness.OutputDistinctUntilChanged) =>
            observable.And(observables, operatorDistinctness).Not();

        /// <summary>
        /// Returns an observable that combines the latest results of all observables using an NAND operator.
        /// (Uses source.CombineLatest(values => values.All(v => v)).Not() on an array made from all observables)
        /// operatorDistinctness will be OutputDistinctUntilChanged.
        /// </summary>
        public static IObservable<bool> Nand(
            this IObservable<bool> observable,
            params IObservable<bool>[] observables) =>
            observable.And(observables).Not();

        /// <summary>
        /// Returns an observable that combines the latest results of all observables using an NAND operator.
        /// (Uses source.CombineLatest(values => values.All(v => v)).Not() on an array made from all observables)
        /// </summary>
        /// <param name="observable1"></param>
        /// <param name="observable2"></param>
        /// <param name="observable3"></param>
        /// <param name="operatorDistinctness">
        ///   <para>OutputDistinctUntilChanged: DistinctUntilChanged is applied to the returned observable, meaning a "true" can only be followed by a "false" and vice versa.</para>
        ///   <para>InputDistinctUntilChanged: DistinctUntilChanged is applied to the inputs only. Meaning that consecutive values on the input do not change the output, but input changes on different inputs can. For example, going from "false", "false" to "true", "false" will emit consecutive "true" values.</para>
        ///   <para>NotDistinct: DistinctUntilChanged is never applied. Meaning both consecutive input and output values will be emitted.</para>
        /// </param>
        public static IObservable<bool> Nand(
            this IObservable<bool> observable1,
            IObservable<bool> observable2,
            IObservable<bool> observable3,
            OperatorDistinctness operatorDistinctness) =>
            observable1.And(observable2, observable3, operatorDistinctness).Not();

        /// <summary>
        /// Returns an observable that combines the latest results of all observables using an NAND operator.
        /// (Uses source.CombineLatest(values => values.All(v => v)).Not() on an array made from all observables)
        /// </summary>
        /// <param name="observable1"></param>
        /// <param name="observable2"></param>
        /// <param name="observable3"></param>
        /// <param name="observable4"></param>
        /// <param name="operatorDistinctness">
        ///   <para>OutputDistinctUntilChanged: DistinctUntilChanged is applied to the returned observable, meaning a "true" can only be followed by a "false" and vice versa.</para>
        ///   <para>InputDistinctUntilChanged: DistinctUntilChanged is applied to the inputs only. Meaning that consecutive values on the input do not change the output, but input changes on different inputs can. For example, going from "false", "false" to "true", "false" will emit consecutive "true" values.</para>
        ///   <para>NotDistinct: DistinctUntilChanged is never applied. Meaning both consecutive input and output values will be emitted.</para>
        /// </param>
        public static IObservable<bool> Nand(
            this IObservable<bool> observable1,
            IObservable<bool> observable2,
            IObservable<bool> observable3,
            IObservable<bool> observable4,
            OperatorDistinctness operatorDistinctness) =>
            observable1.And(observable2, observable3, observable4, operatorDistinctness).Not();

        /// <summary>
        /// Returns an observable that combines the latest results of two observables using an AND operator.
        /// Note: This method is an alias for "And". It is created to prevent namespace conflicts with the "And" method in reactive joins.
        /// </summary>
        /// <param name="observable1"></param>
        /// <param name="observable2"></param>
        /// <param name="operatorDistinctness">
        ///   <para>OutputDistinctUntilChanged: DistinctUntilChanged is applied to the returned observable, meaning a "true" can only be followed by a "false" and vice versa.</para>
        ///   <para>InputDistinctUntilChanged: DistinctUntilChanged is applied to the inputs only. Meaning that consecutive values on the input do not change the output, but input changes on different inputs can. For example, going from "false", "false" to "true", "false" will emit consecutive "false" values.</para>
        ///   <para>NotDistinct: DistinctUntilChanged is never applied. Meaning both consecutive input and output values will be emitted.</para>
        /// </param>
        public static IObservable<bool> AndOp(
            this IObservable<bool> observable1,
            IObservable<bool> observable2,
            OperatorDistinctness operatorDistinctness) =>
            observable1.And(observable2, operatorDistinctness);

        /// <summary>
        /// Returns an observable that combines the latest results of all observables using an AND operator.
        /// (Uses source.CombineLatest(values => values.All(v => v)))
        /// Note: This method is an alias for "And". It is created to prevent namespace conflicts with the "And" method in reactive joins.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="operatorDistinctness">
        ///   <para>OutputDistinctUntilChanged: DistinctUntilChanged is applied to the returned observable, meaning a "true" can only be followed by a "false" and vice versa.</para>
        ///   <para>InputDistinctUntilChanged: DistinctUntilChanged is applied to the inputs only. Meaning that consecutive values on the input do not change the output, but input changes on different inputs can. For example, going from "false", "false" to "true", "false" will emit consecutive "false" values.</para>
        ///   <para>NotDistinct: DistinctUntilChanged is never applied. Meaning both consecutive input and output values will be emitted.</para>
        /// </param>
        public static IObservable<bool> AndOp(
            this IEnumerable<IObservable<bool>> source,
            OperatorDistinctness operatorDistinctness = OperatorDistinctness.OutputDistinctUntilChanged) =>
            source.And(operatorDistinctness);

        /// <summary>
        /// Returns an observable that combines the latest results of all observables using an AND operator.
        /// (Uses source.CombineLatest(values => values.All(v => v)) on an array made from all observables)
        /// Note: This method is an alias for "And". It is created to prevent namespace conflicts with the "And" method in reactive joins.
        /// </summary>
        /// <param name="observable"></param>
        /// <param name="observables"></param>
        /// <param name="operatorDistinctness">
        ///   <para>OutputDistinctUntilChanged: DistinctUntilChanged is applied to the returned observable, meaning a "true" can only be followed by a "false" and vice versa.</para>
        ///   <para>InputDistinctUntilChanged: DistinctUntilChanged is applied to the inputs only. Meaning that consecutive values on the input do not change the output, but input changes on different inputs can. For example, going from "false", "false" to "true", "false" will emit consecutive "false" values.</para>
        ///   <para>NotDistinct: DistinctUntilChanged is never applied. Meaning both consecutive input and output values will be emitted.</para>
        /// </param>
        public static IObservable<bool> AndOp(
            this IObservable<bool> observable,
            IEnumerable<IObservable<bool>> observables,
            OperatorDistinctness operatorDistinctness = OperatorDistinctness.OutputDistinctUntilChanged) =>
            observable.And(observables, operatorDistinctness);

        /// <summary>
        /// Returns an observable that combines the latest results of all observables using an AND operator.
        /// (Uses source.CombineLatest(values => values.All(v => v)) on an array made from all observables)
        /// operatorDistinctness will be OutputDistinctUntilChanged.
        /// </summary>
        public static IObservable<bool> AndOp(
            this IObservable<bool> observable,
            params IObservable<bool>[] observables) =>
            new[] { observable }.Concat(observables).And();

        /// <summary>
        /// Returns an observable that combines the latest results of all observables using an AND operator.
        /// (Uses source.CombineLatest(values => values.All(v => v)) on an array made from all observables)
        /// Note: This method is an alias for "And". It is created to prevent namespace conflicts with the "And" method in reactive joins.
        /// </summary>
        /// <param name="observable1"></param>
        /// <param name="observable2"></param>
        /// <param name="observable3"></param>
        /// <param name="operatorDistinctness">
        ///   <para>OutputDistinctUntilChanged: DistinctUntilChanged is applied to the returned observable, meaning a "true" can only be followed by a "false" and vice versa.</para>
        ///   <para>InputDistinctUntilChanged: DistinctUntilChanged is applied to the inputs only. Meaning that consecutive values on the input do not change the output, but input changes on different inputs can. For example, going from "false", "false" to "true", "false" will emit consecutive "false" values.</para>
        ///   <para>NotDistinct: DistinctUntilChanged is never applied. Meaning both consecutive input and output values will be emitted.</para>
        /// </param>
        public static IObservable<bool> AndOp(
            this IObservable<bool> observable1,
            IObservable<bool> observable2,
            IObservable<bool> observable3,
            OperatorDistinctness operatorDistinctness) =>
            observable1.And(observable2, observable3, operatorDistinctness);

        /// <summary>
        /// Returns an observable that combines the latest results of all observables using an AND operator.
        /// (Uses source.CombineLatest(values => values.All(v => v)) on an array made from all observables)
        /// Note: This method is an alias for "And". It is created to prevent namespace conflicts with the "And" method in reactive joins.
        /// </summary>
        /// <param name="observable1"></param>
        /// <param name="observable2"></param>
        /// <param name="observable3"></param>
        /// <param name="observable4"></param>
        /// <param name="operatorDistinctness">
        ///   <para>OutputDistinctUntilChanged: DistinctUntilChanged is applied to the returned observable, meaning a "true" can only be followed by a "false" and vice versa.</para>
        ///   <para>InputDistinctUntilChanged: DistinctUntilChanged is applied to the inputs only. Meaning that consecutive values on the input do not change the output, but input changes on different inputs can. For example, going from "false", "false" to "true", "false" will emit consecutive "false" values.</para>
        ///   <para>NotDistinct: DistinctUntilChanged is never applied. Meaning both consecutive input and output values will be emitted.</para>
        /// </param>
        public static IObservable<bool> AndOp(
            this IObservable<bool> observable1,
            IObservable<bool> observable2,
            IObservable<bool> observable3,
            IObservable<bool> observable4,
            OperatorDistinctness operatorDistinctness) =>
            observable1.And(observable2, observable3, observable4, operatorDistinctness);
    }
}

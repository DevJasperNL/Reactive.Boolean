using System.Reactive.Linq;

namespace Reactive.Boolean
{
    public static partial class BooleanObservableExtensions
    {
        /// <summary>
        /// Subscribes actions to be executed when the <paramref name="observable"/> emits true or false.
        /// </summary>
        public static IDisposable SubscribeTrueFalse(
            this IObservable<bool> observable,
            Action trueAction,
            Action falseAction)
        {
            ArgumentNullException.ThrowIfNull(observable);
            ArgumentNullException.ThrowIfNull(trueAction);
            ArgumentNullException.ThrowIfNull(falseAction);

            return observable.Subscribe(b =>
            {
                if (b)
                {
                    trueAction();
                    return;
                }

                falseAction();
            });
        }

        /// <summary>
        /// Subscribes an action to be executed when the <paramref name="observable"/> emits false.
        /// </summary>
        public static IDisposable SubscribeFalse(
            this IObservable<bool> observable,
            Action action)
        {
            ArgumentNullException.ThrowIfNull(observable);
            ArgumentNullException.ThrowIfNull(action);

            return observable.Where(b => !b).Subscribe(_ => action());
        }

        /// <summary>
        /// Subscribes an action to be executed when the <paramref name="observable"/> emits true.
        /// </summary>
        public static IDisposable SubscribeTrue(
            this IObservable<bool> observable,
            Action action)
        {
            ArgumentNullException.ThrowIfNull(observable);
            ArgumentNullException.ThrowIfNull(action);

            return observable.Where(b => b).Subscribe(_ => action());
        }
    }
}

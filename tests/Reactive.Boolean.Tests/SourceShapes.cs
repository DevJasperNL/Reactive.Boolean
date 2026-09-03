using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Reactive.Boolean.Tests
{
    /// <summary>
    /// Ways of delivering the same two values to an operator. A hot subject hands each value to every subscriber before
    /// producing the next one; the other shapes do not, and the cold shapes also complete. Operators must behave the same for all of them.
    /// </summary>
    public enum SourceShape
    {
        Subject,
        SelectManyBurst,
        ColdArray,
        ColdConcat,
        DeferPrepend
    }

    internal static class SourceShapes
    {
        /// <summary>
        /// Creates a source that emits <paramref name="first"/> followed by <paramref name="second"/> once <c>Start</c> is invoked
        /// (or upon subscription for the cold shapes).
        /// </summary>
        public static (IObservable<bool> Source, Action Start) Create(SourceShape shape, bool first, bool second)
        {
            var subject = new Subject<bool>();
            return shape switch
            {
                SourceShape.Subject => (subject, () => { subject.OnNext(first); subject.OnNext(second); }),
                SourceShape.SelectManyBurst => (subject.SelectMany(_ => new[] { first, second }), () => subject.OnNext(true)),
                SourceShape.ColdArray => (new[] { first, second }.ToObservable(), () => { }),
                SourceShape.ColdConcat => (Observable.Return(first).Concat(Observable.Return(second)), () => { }),
                SourceShape.DeferPrepend => (Observable.Defer(() => subject.Prepend(first)), () => subject.OnNext(second)),
                _ => throw new ArgumentOutOfRangeException(nameof(shape))
            };
        }
    }
}

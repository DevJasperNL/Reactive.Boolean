using Microsoft.Reactive.Testing;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Reactive.Boolean.Tests
{
    [TestClass]
    public class BooleanObservableExtensionsSchedulingBlinkWhileTrueTests
    {
        private static readonly TimeSpan On = TimeSpan.FromTicks(2);
        private static readonly TimeSpan Off = TimeSpan.FromTicks(3);

        [TestMethod]
        [DataRow(false, false, true)]
        [DataRow(false, false, false)]
        [DataRow(true, false, true)]
        [DataRow(true, false, false)]
        [DataRow(false, true, true)]
        [DataRow(false, true, false)]
        [DataRow(true, true, true)]
        [DataRow(true, true, false)]
        public void BlinkWhileTrue_InitialValue(bool distinctUntilChanged, bool resetTimerOnConsecutiveTrue, bool initialValue)
        {
            // Arrange
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var blinkObservable = subject.BlinkWhileTrue(On, Off, scheduler, distinctUntilChanged, resetTimerOnConsecutiveTrue);

            bool? result = null;
            blinkObservable.Subscribe(b => result = b);

            // Act
            subject.OnNext(initialValue);

            // Assert
            Assert.AreEqual(initialValue, result);
        }

        [TestMethod]
        [DataRow(false, false)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(true, true)]
        public void BlinkWhileTrue_Toggles(bool distinctUntilChanged, bool resetTimerOnConsecutiveTrue)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var blinkObservable = subject.BlinkWhileTrue(On, Off, scheduler, distinctUntilChanged, resetTimerOnConsecutiveTrue);

            var results = new List<bool>();
            blinkObservable.Subscribe(results.Add);

            subject.OnNext(true);
            CollectionAssert.AreEqual(new[] { true }, results);

            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(new[] { true }, results);

            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(new[] { true, false }, results);

            scheduler.AdvanceBy(2);
            CollectionAssert.AreEqual(new[] { true, false }, results);

            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(new[] { true, false, true }, results);

            scheduler.AdvanceBy(2);
            CollectionAssert.AreEqual(new[] { true, false, true, false }, results);
        }

        [TestMethod]
        public void BlinkWhileTrue_SingleInterval_Toggles()
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var blinkObservable = subject.BlinkWhileTrue(On, scheduler);

            var results = new List<bool>();
            blinkObservable.Subscribe(results.Add);

            subject.OnNext(true);
            scheduler.AdvanceBy(2);
            CollectionAssert.AreEqual(new[] { true, false }, results);

            scheduler.AdvanceBy(2);
            CollectionAssert.AreEqual(new[] { true, false, true }, results);

            scheduler.AdvanceBy(2);
            CollectionAssert.AreEqual(new[] { true, false, true, false }, results);
        }

        [TestMethod]
        [DataRow(false, false)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(true, true)]
        public void BlinkWhileTrue_FalseDuringOnPhase_StopsImmediately(bool distinctUntilChanged, bool resetTimerOnConsecutiveTrue)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var blinkObservable = subject.BlinkWhileTrue(On, Off, scheduler, distinctUntilChanged, resetTimerOnConsecutiveTrue);

            var results = new List<bool>();
            blinkObservable.Subscribe(results.Add);

            subject.OnNext(true);
            scheduler.AdvanceBy(1);
            subject.OnNext(false);
            CollectionAssert.AreEqual(new[] { true, false }, results);

            scheduler.AdvanceBy(10);
            CollectionAssert.AreEqual(new[] { true, false }, results);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void BlinkWhileTrue_FalseDuringOffPhase_StopsImmediately(bool distinctUntilChanged)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var blinkObservable = subject.BlinkWhileTrue(On, Off, scheduler, distinctUntilChanged);

            var results = new List<bool>();
            blinkObservable.Subscribe(results.Add);

            subject.OnNext(true);
            scheduler.AdvanceBy(2);
            CollectionAssert.AreEqual(new[] { true, false }, results);

            scheduler.AdvanceBy(1);
            subject.OnNext(false);
            var expected = distinctUntilChanged ? new[] { true, false } : new[] { true, false, false };
            CollectionAssert.AreEqual(expected, results);

            scheduler.AdvanceBy(10);
            CollectionAssert.AreEqual(expected, results);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void BlinkWhileTrue_RepeatedTrue_IgnoredWithoutReset(bool distinctUntilChanged)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var blinkObservable = subject.BlinkWhileTrue(On, Off, scheduler, distinctUntilChanged);

            var results = new List<bool>();
            blinkObservable.Subscribe(results.Add);

            subject.OnNext(true);
            scheduler.AdvanceBy(1);
            subject.OnNext(true); // Neither emitted nor restarting the phase.
            CollectionAssert.AreEqual(new[] { true }, results);

            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(new[] { true, false }, results);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void BlinkWhileTrue_RepeatedTrueDuringOnPhase_RestartsWithReset(bool distinctUntilChanged)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var blinkObservable = subject.BlinkWhileTrue(On, Off, scheduler, distinctUntilChanged, resetTimerOnConsecutiveTrue: true);

            var results = new List<bool>();
            blinkObservable.Subscribe(results.Add);

            subject.OnNext(true);
            scheduler.AdvanceBy(1);
            subject.OnNext(true);
            var expected = distinctUntilChanged ? new[] { true } : new[] { true, true };
            CollectionAssert.AreEqual(expected, results);

            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(expected, results);

            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(expected.Append(false).ToArray(), results);
        }

        [TestMethod]
        public void BlinkWhileTrue_RepeatedTrueDuringOffPhase_RestartsWithReset()
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var blinkObservable = subject.BlinkWhileTrue(On, Off, scheduler, resetTimerOnConsecutiveTrue: true);

            var results = new List<bool>();
            blinkObservable.Subscribe(results.Add);

            subject.OnNext(true);
            scheduler.AdvanceBy(2);
            CollectionAssert.AreEqual(new[] { true, false }, results);

            scheduler.AdvanceBy(1);
            subject.OnNext(true);
            CollectionAssert.AreEqual(new[] { true, false, true }, results);

            scheduler.AdvanceBy(2);
            CollectionAssert.AreEqual(new[] { true, false, true, false }, results);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void BlinkWhileTrue_ConsecutiveFalse_NotDistinct_Emitted(bool resetTimerOnConsecutiveTrue)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var blinkObservable = subject.BlinkWhileTrue(On, Off, scheduler, distinctUntilChanged: false, resetTimerOnConsecutiveTrue);

            var results = new List<bool>();
            blinkObservable.Subscribe(results.Add);

            subject.OnNext(false);
            subject.OnNext(false);
            CollectionAssert.AreEqual(new[] { false, false }, results);
        }

        [TestMethod]
        [DataRow(false, false)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(true, true)]
        public void BlinkWhileTrue_CompleteImmediately_MidOnPhase_CompletesAtOnce(bool distinctUntilChanged, bool resetTimerOnConsecutiveTrue)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var blinkObservable = subject.BlinkWhileTrue(On, Off, scheduler, distinctUntilChanged, resetTimerOnConsecutiveTrue, CompletionBehavior.CompleteImmediately);

            var results = new List<bool>();
            var completed = false;
            blinkObservable.Subscribe(results.Add, () => completed = true);

            subject.OnNext(true);
            scheduler.AdvanceBy(1);
            subject.OnCompleted();
            Assert.IsTrue(completed);

            scheduler.AdvanceBy(10);
            CollectionAssert.AreEqual(new[] { true }, results);
        }

        [TestMethod]
        [DataRow(false, false)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(true, true)]
        public void BlinkWhileTrue_CompleteAfterTimer_MidOnPhase_FinishesPhaseThenCompletes(bool distinctUntilChanged, bool resetTimerOnConsecutiveTrue)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var blinkObservable = subject.BlinkWhileTrue(On, Off, scheduler, distinctUntilChanged, resetTimerOnConsecutiveTrue, CompletionBehavior.CompleteAfterTimer);

            var results = new List<bool>();
            var completed = false;
            blinkObservable.Subscribe(results.Add, () => completed = true);

            subject.OnNext(true);
            scheduler.AdvanceBy(1);
            subject.OnCompleted();
            Assert.IsFalse(completed);
            CollectionAssert.AreEqual(new[] { true }, results);

            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(new[] { true, false }, results);
            Assert.IsTrue(completed);

            scheduler.AdvanceBy(10);
            CollectionAssert.AreEqual(new[] { true, false }, results);
        }

        [TestMethod]
        [DataRow(false, false)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(true, true)]
        public void BlinkWhileTrue_CompleteAfterTimer_MidOffPhase_CompletesImmediately(bool distinctUntilChanged, bool resetTimerOnConsecutiveTrue)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var blinkObservable = subject.BlinkWhileTrue(On, Off, scheduler, distinctUntilChanged, resetTimerOnConsecutiveTrue, CompletionBehavior.CompleteAfterTimer);

            var results = new List<bool>();
            var completed = false;
            blinkObservable.Subscribe(results.Add, () => completed = true);

            subject.OnNext(true);
            scheduler.AdvanceBy(2);
            CollectionAssert.AreEqual(new[] { true, false }, results);

            scheduler.AdvanceBy(1);
            subject.OnCompleted();
            Assert.IsTrue(completed);

            scheduler.AdvanceBy(10);
            CollectionAssert.AreEqual(new[] { true, false }, results);
        }

        [TestMethod]
        [DataRow(false, false)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(true, true)]
        public void BlinkWhileTrue_Error(bool distinctUntilChanged, bool resetTimerOnConsecutiveTrue)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var blinkObservable = subject.BlinkWhileTrue(On, Off, scheduler, distinctUntilChanged, resetTimerOnConsecutiveTrue);

            bool? result = null;
            Exception? receivedException = null;
            blinkObservable.Subscribe(b => result = b, e => receivedException = e);

            subject.OnNext(true);

            scheduler.AdvanceBy(1);
            Assert.IsTrue(result);

            var exception = new InvalidOperationException("This is a test");
            subject.OnError(exception);
            Assert.AreEqual(receivedException, exception);
        }

        [TestMethod]
        [DataRow(false, false)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(true, true)]
        public void BlinkWhileTrue_DisposeCancelsTimer(bool distinctUntilChanged, bool resetTimerOnConsecutiveTrue)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var blinkObservable = subject.BlinkWhileTrue(On, Off, scheduler, distinctUntilChanged, resetTimerOnConsecutiveTrue);

            var results = new List<bool>();
            var subscription = blinkObservable.Subscribe(results.Add);

            subject.OnNext(true);
            subscription.Dispose();

            scheduler.AdvanceBy(10);
            CollectionAssert.AreEqual(new[] { true }, results);
        }

        [TestMethod]
        [DataRow(false, false)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(true, true)]
        public void BlinkWhileTrue_SubscribesToSourceOnce(bool distinctUntilChanged, bool resetTimerOnConsecutiveTrue)
        {
            var subscriptions = 0;
            var source = Observable.Defer(() =>
            {
                subscriptions++;
                return Observable.Never<bool>();
            });

            source.BlinkWhileTrue(On, Off, new TestScheduler(), distinctUntilChanged, resetTimerOnConsecutiveTrue).Subscribe();

            Assert.AreEqual(1, subscriptions);
        }

        [TestMethod]
        [DataRow(SourceShape.Subject, false, false)]
        [DataRow(SourceShape.Subject, true, false)]
        [DataRow(SourceShape.Subject, false, true)]
        [DataRow(SourceShape.Subject, true, true)]
        [DataRow(SourceShape.SelectManyBurst, false, false)]
        [DataRow(SourceShape.SelectManyBurst, true, false)]
        [DataRow(SourceShape.SelectManyBurst, false, true)]
        [DataRow(SourceShape.SelectManyBurst, true, true)]
        [DataRow(SourceShape.ColdArray, false, false)]
        [DataRow(SourceShape.ColdArray, true, false)]
        [DataRow(SourceShape.ColdArray, false, true)]
        [DataRow(SourceShape.ColdArray, true, true)]
        [DataRow(SourceShape.ColdConcat, false, false)]
        [DataRow(SourceShape.ColdConcat, true, false)]
        [DataRow(SourceShape.ColdConcat, false, true)]
        [DataRow(SourceShape.ColdConcat, true, true)]
        [DataRow(SourceShape.DeferPrepend, false, false)]
        [DataRow(SourceShape.DeferPrepend, true, false)]
        [DataRow(SourceShape.DeferPrepend, false, true)]
        [DataRow(SourceShape.DeferPrepend, true, true)]
        public void BlinkWhileTrue_FalseThenTrue_SameForEverySourceShape(SourceShape shape, bool distinctUntilChanged, bool resetTimerOnConsecutiveTrue)
        {
            var scheduler = new TestScheduler();
            var (source, start) = SourceShapes.Create(shape, false, true);
            var blinkObservable = source.BlinkWhileTrue(On, Off, scheduler, distinctUntilChanged, resetTimerOnConsecutiveTrue, CompletionBehavior.CompleteAfterTimer);

            var results = new List<bool>();
            blinkObservable.Subscribe(results.Add);
            start();
            CollectionAssert.AreEqual(new[] { false, true }, results);

            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(new[] { false, true }, results);

            // The cold shapes complete here after finishing the "true" phase, so only the first phase is common to every shape.
            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(new[] { false, true, false }, results);
        }

        [TestMethod]
        public void BlinkWhileTrue_ZeroOrNegativeDuration_Throws()
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => subject.BlinkWhileTrue(TimeSpan.Zero, scheduler));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => subject.BlinkWhileTrue(TimeSpan.FromTicks(-1), scheduler));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => subject.BlinkWhileTrue(TimeSpan.Zero, Off, scheduler));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => subject.BlinkWhileTrue(On, TimeSpan.Zero, scheduler));
        }

        [TestMethod]
        public void BlinkWhileFalse_IsInverse()
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var blinkObservable = subject.BlinkWhileFalse(On, Off, scheduler);

            var results = new List<bool>();
            blinkObservable.Subscribe(results.Add);

            subject.OnNext(false);
            CollectionAssert.AreEqual(new[] { false }, results);

            scheduler.AdvanceBy(2);
            CollectionAssert.AreEqual(new[] { false, true }, results);

            scheduler.AdvanceBy(3);
            CollectionAssert.AreEqual(new[] { false, true, false }, results);

            subject.OnNext(true);
            CollectionAssert.AreEqual(new[] { false, true, false, true }, results);
        }
    }
}

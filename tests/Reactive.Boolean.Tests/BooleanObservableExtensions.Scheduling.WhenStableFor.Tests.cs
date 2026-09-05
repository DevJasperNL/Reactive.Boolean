using Microsoft.Reactive.Testing;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Reactive.Boolean.Tests
{
    [TestClass]
    public class BooleanObservableExtensionsSchedulingWhenStableForTests
    {
        [TestMethod]
        [DataRow(false, false, true)]
        [DataRow(false, false, false)]
        [DataRow(true, false, true)]
        [DataRow(true, false, false)]
        [DataRow(false, true, true)]
        [DataRow(false, true, false)]
        [DataRow(true, true, true)]
        [DataRow(true, true, false)]
        public void WhenStableFor_InitialValueIsImmediate(bool resetTimerOnConsecutiveValue, bool distinctUntilChanged, bool initialValue)
        {
            // Arrange
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var stableObservable = subject.WhenStableFor(TimeSpan.FromMinutes(1), scheduler, resetTimerOnConsecutiveValue, distinctUntilChanged);

            bool? result = null;
            stableObservable.Subscribe(b => result = b);

            // Act
            subject.OnNext(initialValue);

            // Assert
            Assert.AreEqual(initialValue, result);
        }

        [TestMethod]
        [DataRow(false, true)]
        [DataRow(false, false)]
        [DataRow(true, true)]
        [DataRow(true, false)]
        public void WhenStableFor_InitialValue_Distinct(bool resetTimerOnConsecutiveValue, bool initialValue)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var stableObservable = subject.WhenStableFor(TimeSpan.FromMinutes(1), scheduler, resetTimerOnConsecutiveValue);

            var results = new List<bool>();
            stableObservable.Subscribe(results.Add);

            subject.OnNext(initialValue);
            subject.OnNext(initialValue);

            CollectionAssert.AreEqual(new[] { initialValue }, results);
        }

        [TestMethod]
        [DataRow(false, true)]
        [DataRow(false, false)]
        [DataRow(true, true)]
        [DataRow(true, false)]
        public void WhenStableFor_InitialValue_NotDistinct(bool resetTimerOnConsecutiveValue, bool initialValue)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var stableObservable = subject.WhenStableFor(TimeSpan.FromMinutes(1), scheduler, resetTimerOnConsecutiveValue, distinctUntilChanged: false);

            var results = new List<bool>();
            stableObservable.Subscribe(results.Add);

            subject.OnNext(initialValue);
            subject.OnNext(initialValue);

            CollectionAssert.AreEqual(new[] { initialValue, initialValue }, results);
        }

        [TestMethod]
        [DataRow(false, false, true)]
        [DataRow(false, false, false)]
        [DataRow(true, false, true)]
        [DataRow(true, false, false)]
        [DataRow(false, true, true)]
        [DataRow(false, true, false)]
        [DataRow(true, true, true)]
        [DataRow(true, true, false)]
        public void WhenStableFor_ChangeAfterTimeSpan(bool resetTimerOnConsecutiveValue, bool distinctUntilChanged, bool initialValue)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var stableObservable = subject.WhenStableFor(TimeSpan.FromTicks(2), scheduler, resetTimerOnConsecutiveValue, distinctUntilChanged);

            var results = new List<bool>();
            stableObservable.Subscribe(results.Add);

            subject.OnNext(initialValue);
            subject.OnNext(!initialValue);
            CollectionAssert.AreEqual(new[] { initialValue }, results);

            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(new[] { initialValue }, results);

            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(new[] { initialValue, !initialValue }, results);
        }

        [TestMethod]
        [DataRow(false, true)]
        [DataRow(false, false)]
        [DataRow(true, true)]
        [DataRow(true, false)]
        public void WhenStableFor_RevertDuringTimer_Cancels_Distinct(bool resetTimerOnConsecutiveValue, bool initialValue)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var stableObservable = subject.WhenStableFor(TimeSpan.FromTicks(2), scheduler, resetTimerOnConsecutiveValue);

            var results = new List<bool>();
            stableObservable.Subscribe(results.Add);

            subject.OnNext(initialValue);
            subject.OnNext(!initialValue);
            scheduler.AdvanceBy(1);
            subject.OnNext(initialValue);
            CollectionAssert.AreEqual(new[] { initialValue }, results);

            scheduler.AdvanceBy(2);
            CollectionAssert.AreEqual(new[] { initialValue }, results);
        }

        [TestMethod]
        [DataRow(false, true)]
        [DataRow(false, false)]
        [DataRow(true, true)]
        [DataRow(true, false)]
        public void WhenStableFor_RevertDuringTimer_Cancels_NotDistinct(bool resetTimerOnConsecutiveValue, bool initialValue)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var stableObservable = subject.WhenStableFor(TimeSpan.FromTicks(2), scheduler, resetTimerOnConsecutiveValue, distinctUntilChanged: false);

            var results = new List<bool>();
            stableObservable.Subscribe(results.Add);

            subject.OnNext(initialValue);
            subject.OnNext(!initialValue);
            scheduler.AdvanceBy(1);
            subject.OnNext(initialValue);
            CollectionAssert.AreEqual(new[] { initialValue, initialValue }, results);

            scheduler.AdvanceBy(2);
            CollectionAssert.AreEqual(new[] { initialValue, initialValue }, results);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void WhenStableFor_TimerNotResetOnConsecutiveValue(bool distinctUntilChanged)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var stableObservable = subject.WhenStableFor(TimeSpan.FromTicks(2), scheduler, distinctUntilChanged: distinctUntilChanged);

            var results = new List<bool>();
            stableObservable.Subscribe(results.Add);

            subject.OnNext(true);
            subject.OnNext(false);
            scheduler.AdvanceBy(1);
            subject.OnNext(false); // Should not reset timer
            CollectionAssert.AreEqual(new[] { true }, results);

            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(new[] { true, false }, results);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void WhenStableFor_TimerResetOnConsecutiveValue(bool distinctUntilChanged)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var stableObservable = subject.WhenStableFor(TimeSpan.FromTicks(2), scheduler, resetTimerOnConsecutiveValue: true, distinctUntilChanged: distinctUntilChanged);

            var results = new List<bool>();
            stableObservable.Subscribe(results.Add);

            subject.OnNext(true);
            subject.OnNext(false);
            scheduler.AdvanceBy(1);
            subject.OnNext(false); // Should reset timer
            CollectionAssert.AreEqual(new[] { true }, results);

            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(new[] { true }, results);

            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(new[] { true, false }, results);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void WhenStableFor_ConsecutiveCurrentValue_NotDistinct_Emitted(bool resetTimerOnConsecutiveValue)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var stableObservable = subject.WhenStableFor(TimeSpan.FromTicks(2), scheduler, resetTimerOnConsecutiveValue, distinctUntilChanged: false);

            var results = new List<bool>();
            stableObservable.Subscribe(results.Add);

            subject.OnNext(true);
            subject.OnNext(true);
            CollectionAssert.AreEqual(new[] { true, true }, results);

            scheduler.AdvanceBy(2);
            CollectionAssert.AreEqual(new[] { true, true }, results);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void WhenStableFor_ChangesBackAndForth(bool distinctUntilChanged)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var stableObservable = subject.WhenStableFor(TimeSpan.FromTicks(2), scheduler, distinctUntilChanged: distinctUntilChanged);

            var results = new List<bool>();
            stableObservable.Subscribe(results.Add);

            subject.OnNext(true);
            subject.OnNext(false);
            scheduler.AdvanceBy(2);
            CollectionAssert.AreEqual(new[] { true, false }, results);

            subject.OnNext(true);
            scheduler.AdvanceBy(2);
            CollectionAssert.AreEqual(new[] { true, false, true }, results);
        }

        [TestMethod]
        [DataRow(false, false)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(true, true)]
        public void WhenStableFor_CompleteIsImmediate(bool resetTimerOnConsecutiveValue, bool distinctUntilChanged)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var stableObservable = subject.WhenStableFor(TimeSpan.FromTicks(2), scheduler, resetTimerOnConsecutiveValue, distinctUntilChanged);

            bool? result = null;
            var completed = false;
            stableObservable.Subscribe(b => result = b, _ => { }, () => completed = true);

            subject.OnNext(true);
            subject.OnNext(false);

            scheduler.AdvanceBy(1);
            Assert.IsTrue(result);

            subject.OnCompleted();
            Assert.IsTrue(completed);
        }

        [TestMethod]
        [DataRow(false, false)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(true, true)]
        public void WhenStableFor_Error(bool resetTimerOnConsecutiveValue, bool distinctUntilChanged)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var stableObservable = subject.WhenStableFor(TimeSpan.FromTicks(2), scheduler, resetTimerOnConsecutiveValue, distinctUntilChanged);

            bool? result = null;
            Exception? receivedException = null;
            stableObservable.Subscribe(b => result = b, e => receivedException = e);

            subject.OnNext(true);
            subject.OnNext(false);

            scheduler.AdvanceBy(1);
            Assert.IsTrue(result);

            var exception = new InvalidOperationException("This is a test");
            subject.OnError(exception);
            Assert.AreEqual(receivedException, exception);
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
        public void WhenStableFor_TrueThenFalse_SameForEverySourceShape(SourceShape shape, bool resetTimerOnConsecutiveValue, bool distinctUntilChanged)
        {
            var scheduler = new TestScheduler();
            var (source, start) = SourceShapes.Create(shape, true, false);
            var stableObservable = source.WhenStableFor(TimeSpan.FromTicks(2), scheduler, resetTimerOnConsecutiveValue, distinctUntilChanged, CompletionBehavior.CompleteAfterTimer);

            var results = new List<bool>();
            stableObservable.Subscribe(results.Add);
            start();
            CollectionAssert.AreEqual(new[] { true }, results);

            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(new[] { true }, results);

            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(new[] { true, false }, results);
        }

        [TestMethod]
        [DataRow(false, false)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(true, true)]
        public void WhenStableFor_SubscribesToSourceOnce(bool resetTimerOnConsecutiveValue, bool distinctUntilChanged)
        {
            var subscriptions = 0;
            var source = Observable.Defer(() =>
            {
                subscriptions++;
                return Observable.Never<bool>();
            });

            source.WhenStableFor(TimeSpan.FromTicks(2), new TestScheduler(), resetTimerOnConsecutiveValue, distinctUntilChanged).Subscribe();

            Assert.AreEqual(1, subscriptions);
        }

        [TestMethod]
        [DataRow(false, false)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(true, true)]
        public void WhenStableFor_CompleteImmediately_DoesNotEmitPendingValue(bool resetTimerOnConsecutiveValue, bool distinctUntilChanged)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var stableObservable = subject.WhenStableFor(TimeSpan.FromTicks(2), scheduler, resetTimerOnConsecutiveValue, distinctUntilChanged, CompletionBehavior.CompleteImmediately);

            var results = new List<bool>();
            var completed = false;
            stableObservable.Subscribe(results.Add, () => completed = true);

            subject.OnNext(true);
            subject.OnNext(false);
            scheduler.AdvanceBy(1);
            subject.OnCompleted();
            Assert.IsTrue(completed);

            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(new[] { true }, results);
        }

        [TestMethod]
        [DataRow(false, false)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(true, true)]
        public void WhenStableFor_CompleteAfterTimer_EmitsPendingValueThenCompletes(bool resetTimerOnConsecutiveValue, bool distinctUntilChanged)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var stableObservable = subject.WhenStableFor(TimeSpan.FromTicks(2), scheduler, resetTimerOnConsecutiveValue, distinctUntilChanged, CompletionBehavior.CompleteAfterTimer);

            var results = new List<bool>();
            var completed = false;
            stableObservable.Subscribe(results.Add, () => completed = true);

            subject.OnNext(true);
            subject.OnNext(false);
            scheduler.AdvanceBy(1);
            subject.OnCompleted();
            Assert.IsFalse(completed);
            CollectionAssert.AreEqual(new[] { true }, results);

            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(new[] { true, false }, results);
            Assert.IsTrue(completed);
        }

        [TestMethod]
        [DataRow(false, false)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(true, true)]
        public void WhenStableFor_CompleteAfterTimer_NoTimerRunning_CompletesImmediately(bool resetTimerOnConsecutiveValue, bool distinctUntilChanged)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var stableObservable = subject.WhenStableFor(TimeSpan.FromTicks(2), scheduler, resetTimerOnConsecutiveValue, distinctUntilChanged, CompletionBehavior.CompleteAfterTimer);

            var results = new List<bool>();
            var completed = false;
            stableObservable.Subscribe(results.Add, () => completed = true);

            subject.OnNext(true);
            subject.OnNext(false);
            subject.OnNext(true); // Cancels the pending change.
            scheduler.AdvanceBy(1);
            subject.OnCompleted();
            Assert.IsTrue(completed);

            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(distinctUntilChanged ? new[] { true } : new[] { true, true }, results);
        }

        [TestMethod]
        [DataRow(false, false)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(true, true)]
        public void WhenStableFor_DisposeCancelsTimer(bool resetTimerOnConsecutiveValue, bool distinctUntilChanged)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var stableObservable = subject.WhenStableFor(TimeSpan.FromTicks(2), scheduler, resetTimerOnConsecutiveValue, distinctUntilChanged);

            var results = new List<bool>();
            var subscription = stableObservable.Subscribe(results.Add);

            subject.OnNext(true);
            subject.OnNext(false);
            subscription.Dispose();

            scheduler.AdvanceBy(2);
            CollectionAssert.AreEqual(new[] { true }, results);
        }

        [TestMethod]
        public void WhenStableFor_ZeroOrNegativeTimeSpan_Throws()
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => subject.WhenStableFor(TimeSpan.Zero, scheduler));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => subject.WhenStableFor(TimeSpan.FromTicks(-1), scheduler));
        }

        [TestMethod]
        public void WhenStableFor_ValueFedBackFromObserver_IsNotLost()
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var stableObservable = subject.WhenStableFor(TimeSpan.FromTicks(2), scheduler);

            var results = new List<bool>();
            stableObservable.Subscribe(b =>
            {
                results.Add(b);
                if (b)
                {
                    subject.OnNext(false);
                }
            });

            subject.OnNext(true);
            CollectionAssert.AreEqual(new[] { true }, results);

            scheduler.AdvanceBy(2);
            CollectionAssert.AreEqual(new[] { true, false }, results);
        }
    }
}

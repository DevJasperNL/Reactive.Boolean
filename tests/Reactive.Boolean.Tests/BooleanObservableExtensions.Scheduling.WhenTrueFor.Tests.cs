using Microsoft.Reactive.Testing;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Reactive.Boolean.Tests
{
    [TestClass]
    public class BooleanObservableExtensionsSchedulingWhenTrueForTests
    {
        [TestMethod]
        [DataRow(true, true)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(false, false)]
        public void WhenTrueFor_InitialValueAlwaysFalse(bool resetTimerOnConsecutiveTrue, bool initialValue)
        {
            // Arrange
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.WhenTrueFor(TimeSpan.FromMinutes(1), scheduler, resetTimerOnConsecutiveTrue);

            bool? result = null;
            memoryObservable.Subscribe(b => result = b);

            // Act
            subject.OnNext(initialValue);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        [DataRow(false, true)]
        [DataRow(false, false)]
        [DataRow(true, true)]
        [DataRow(true, false)]
        public void WhenTrueFor_InitialValue_IsDistinct(bool resetTimerOnConsecutiveTrue, bool initialValue)
        {
            // Arrange
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.WhenTrueFor(TimeSpan.FromMinutes(1), scheduler, resetTimerOnConsecutiveTrue);

            var results = new List<bool>();
            memoryObservable.Subscribe(results.Add);

            // Act
            subject.OnNext(initialValue);
            subject.OnNext(initialValue);

            // Assert
            CollectionAssert.AreEqual(new[] { false }, results);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void WhenTrueFor_TrueAfterTimeSpan(bool resetTimerOnConsecutiveTrue)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.WhenTrueFor(TimeSpan.FromTicks(2), scheduler, resetTimerOnConsecutiveTrue);

            bool? result = null;
            memoryObservable.Subscribe(b => result = b);

            subject.OnNext(true);
            Assert.IsFalse(result);

            scheduler.AdvanceBy(1);
            Assert.IsFalse(result);

            scheduler.AdvanceBy(1);
            Assert.IsTrue(result);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void WhenTrueFor_TrueAfterTimeSpan_Repeat(bool resetTimerOnConsecutiveTrue)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.WhenTrueFor(TimeSpan.FromTicks(2), scheduler, resetTimerOnConsecutiveTrue);

            bool? result = null;
            memoryObservable.Subscribe(b => result = b);

            subject.OnNext(true);
            Assert.IsFalse(result);

            scheduler.AdvanceBy(1);
            Assert.IsFalse(result);

            scheduler.AdvanceBy(1);
            Assert.IsTrue(result);

            subject.OnNext(false);
            subject.OnNext(true);
            Assert.IsFalse(result);

            scheduler.AdvanceBy(1);
            Assert.IsFalse(result);

            scheduler.AdvanceBy(1);
            Assert.IsTrue(result);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void WhenTrueFor_FalseIsImmediate(bool resetTimerOnConsecutiveTrue)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.WhenTrueFor(TimeSpan.FromTicks(1), scheduler, resetTimerOnConsecutiveTrue);

            bool? result = null;
            memoryObservable.Subscribe(b => result = b);

            subject.OnNext(true);
            scheduler.AdvanceBy(1);
            Assert.IsTrue(result);

            subject.OnNext(false);
            Assert.IsFalse(result);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void WhenTrueFor_RemainsTrueForTimeSpanAfterTrue(bool resetTimerOnConsecutiveTrue)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.WhenTrueFor(TimeSpan.FromTicks(2), scheduler, resetTimerOnConsecutiveTrue);

            bool? result = null;
            memoryObservable.Subscribe(b => result = b);

            scheduler.AdvanceBy(1);
            subject.OnNext(true);
            Assert.IsFalse(result);

            scheduler.AdvanceBy(1);
            Assert.IsFalse(result);

            scheduler.AdvanceBy(1);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void WhenTrueFor_FalseResetsTimer()
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.WhenTrueFor(TimeSpan.FromTicks(2), scheduler);

            bool? result = null;
            memoryObservable.Subscribe(b => result = b);

            subject.OnNext(true);

            scheduler.AdvanceBy(1);
            subject.OnNext(false);
            subject.OnNext(true);

            scheduler.AdvanceBy(1);
            Assert.IsFalse(result);

            scheduler.AdvanceBy(1);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void WhenTrueFor_TimerNotResetOnConsecutiveTrue()
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.WhenTrueFor(TimeSpan.FromTicks(2), scheduler);

            var results = new List<bool>();
            memoryObservable.Subscribe(results.Add);

            subject.OnNext(true);
            CollectionAssert.AreEqual(new[] { false }, results);

            scheduler.AdvanceBy(1);
            subject.OnNext(true); // Should not reset timer
            CollectionAssert.AreEqual(new[] { false }, results);

            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(new[] { false, true }, results);
        }

        [TestMethod]
        public void WhenTrueFor_TimerResetOnConsecutiveTrue()
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.WhenTrueFor(TimeSpan.FromTicks(2), scheduler, resetTimerOnConsecutiveTrue: true);

            var results = new List<bool>();
            memoryObservable.Subscribe(results.Add);

            subject.OnNext(true);
            CollectionAssert.AreEqual(new[] { false }, results);

            scheduler.AdvanceBy(1);
            subject.OnNext(true); // Should reset timer
            CollectionAssert.AreEqual(new[] { false }, results);

            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(new[] { false }, results);

            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(new[] { false, true }, results);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void WhenTrueFor_CompleteIsImmediate(bool resetTimerOnConsecutiveTrue)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.WhenTrueFor(TimeSpan.FromTicks(2), scheduler, resetTimerOnConsecutiveTrue);

            bool? result = null;
            bool completed = false;
            memoryObservable.Subscribe(b => result = b, _ => { }, () => completed = true);

            subject.OnNext(true);

            scheduler.AdvanceBy(1);
            Assert.IsFalse(result);

            subject.OnCompleted();
            Assert.IsTrue(completed);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void WhenTrueFor_Error(bool resetTimerOnConsecutiveTrue)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.WhenTrueFor(TimeSpan.FromTicks(2), scheduler, resetTimerOnConsecutiveTrue);

            bool? result = null;
            Exception? receivedException = null;
            memoryObservable.Subscribe(b => result = b, e => receivedException = e);

            subject.OnNext(true);

            scheduler.AdvanceBy(1);
            Assert.IsFalse(result);

            var exception = new InvalidOperationException("This is a test");
            subject.OnError(exception);
            Assert.AreEqual(receivedException, exception);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void WhenTrueFor_ConsecutiveFalse_NotDistinct(bool resetTimerOnConsecutiveTrue)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.WhenTrueFor(TimeSpan.FromMinutes(1), scheduler, resetTimerOnConsecutiveTrue, distinctUntilChanged: false);

            var results = new List<bool>();
            memoryObservable.Subscribe(results.Add);

            subject.OnNext(false);
            subject.OnNext(false);

            CollectionAssert.AreEqual(new[] { false, false }, results);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void WhenTrueFor_ConsecutiveTrueDuringTimer_NotDistinct_NotEmitted(bool resetTimerOnConsecutiveTrue)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.WhenTrueFor(TimeSpan.FromTicks(2), scheduler, resetTimerOnConsecutiveTrue, distinctUntilChanged: false);

            var results = new List<bool>();
            memoryObservable.Subscribe(results.Add);

            subject.OnNext(true);
            subject.OnNext(true);
            CollectionAssert.AreEqual(new[] { false }, results);

            scheduler.AdvanceBy(2);
            CollectionAssert.AreEqual(new[] { false, true }, results);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void WhenTrueFor_ConsecutiveTrueAfterTimer_NotDistinct_Emitted(bool resetTimerOnConsecutiveTrue)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.WhenTrueFor(TimeSpan.FromTicks(2), scheduler, resetTimerOnConsecutiveTrue, distinctUntilChanged: false);

            var results = new List<bool>();
            memoryObservable.Subscribe(results.Add);

            subject.OnNext(true);
            scheduler.AdvanceBy(2);
            CollectionAssert.AreEqual(new[] { false, true }, results);

            subject.OnNext(true);
            CollectionAssert.AreEqual(new[] { false, true, true }, results);
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
        public void WhenTrueFor_FalseThenTrue_SameForEverySourceShape(SourceShape shape, bool resetTimerOnConsecutiveTrue, bool distinctUntilChanged)
        {
            var scheduler = new TestScheduler();
            var (source, start) = SourceShapes.Create(shape, false, true);
            var memoryObservable = source.WhenTrueFor(TimeSpan.FromTicks(2), scheduler, resetTimerOnConsecutiveTrue, distinctUntilChanged, CompletionBehavior.CompleteAfterTimer);

            var results = new List<bool>();
            memoryObservable.Subscribe(results.Add);
            start();
            CollectionAssert.AreEqual(new[] { false }, results);

            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(new[] { false }, results);

            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(new[] { false, true }, results);
        }

        [TestMethod]
        [DataRow(false, false)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(true, true)]
        public void WhenTrueFor_SubscribesToSourceOnce(bool resetTimerOnConsecutiveTrue, bool distinctUntilChanged)
        {
            var subscriptions = 0;
            var source = Observable.Defer(() =>
            {
                subscriptions++;
                return Observable.Never<bool>();
            });

            source.WhenTrueFor(TimeSpan.FromTicks(2), new TestScheduler(), resetTimerOnConsecutiveTrue, distinctUntilChanged).Subscribe();

            Assert.AreEqual(1, subscriptions);
        }

        [TestMethod]
        [DataRow(false, false)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(true, true)]
        public void WhenTrueFor_CompleteImmediately_DoesNotEmitTrue(bool resetTimerOnConsecutiveTrue, bool distinctUntilChanged)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.WhenTrueFor(TimeSpan.FromTicks(2), scheduler, resetTimerOnConsecutiveTrue, distinctUntilChanged, CompletionBehavior.CompleteImmediately);

            var results = new List<bool>();
            var completed = false;
            memoryObservable.Subscribe(results.Add, () => completed = true);

            subject.OnNext(true);
            scheduler.AdvanceBy(1);
            subject.OnCompleted();
            Assert.IsTrue(completed);

            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(new[] { false }, results);
        }

        [TestMethod]
        [DataRow(false, false)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(true, true)]
        public void WhenTrueFor_CompleteAfterTimer_EmitsTrueThenCompletes(bool resetTimerOnConsecutiveTrue, bool distinctUntilChanged)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.WhenTrueFor(TimeSpan.FromTicks(2), scheduler, resetTimerOnConsecutiveTrue, distinctUntilChanged, CompletionBehavior.CompleteAfterTimer);

            var results = new List<bool>();
            var completed = false;
            memoryObservable.Subscribe(results.Add, () => completed = true);

            subject.OnNext(true);
            scheduler.AdvanceBy(1);
            subject.OnCompleted();
            Assert.IsFalse(completed);
            CollectionAssert.AreEqual(new[] { false }, results);

            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(new[] { false, true }, results);
            Assert.IsTrue(completed);
        }

        [TestMethod]
        [DataRow(false, false)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(true, true)]
        public void WhenTrueFor_CompleteAfterTimer_NoTimerRunning_CompletesImmediately(bool resetTimerOnConsecutiveTrue, bool distinctUntilChanged)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.WhenTrueFor(TimeSpan.FromTicks(2), scheduler, resetTimerOnConsecutiveTrue, distinctUntilChanged, CompletionBehavior.CompleteAfterTimer);

            var results = new List<bool>();
            var completed = false;
            memoryObservable.Subscribe(results.Add, () => completed = true);

            subject.OnNext(true);
            subject.OnNext(false); // Stops the timer.
            scheduler.AdvanceBy(1);
            subject.OnCompleted();
            Assert.IsTrue(completed);

            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(distinctUntilChanged ? new[] { false } : new[] { false, false }, results);
        }

        [TestMethod]
        [DataRow(false, false)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(true, true)]
        public void WhenTrueFor_DisposeCancelsTimer(bool resetTimerOnConsecutiveTrue, bool distinctUntilChanged)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.WhenTrueFor(TimeSpan.FromTicks(2), scheduler, resetTimerOnConsecutiveTrue, distinctUntilChanged);

            var results = new List<bool>();
            var subscription = memoryObservable.Subscribe(results.Add);

            subject.OnNext(true);
            subscription.Dispose();

            scheduler.AdvanceBy(2);
            CollectionAssert.AreEqual(new[] { false }, results);
        }

        [TestMethod]
        public void WhenTrueFor_ZeroOrNegativeTimeSpan_Throws()
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => subject.WhenTrueFor(TimeSpan.Zero, scheduler));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => subject.WhenTrueFor(TimeSpan.FromTicks(-1), scheduler));
        }

        [TestMethod]
        public void WhenTrueFor_ImmediateScheduler_TimerFiresInline()
        {
            var subject = new Subject<bool>();
            var memoryObservable = subject.WhenTrueFor(TimeSpan.FromTicks(1), Scheduler.Immediate);

            var results = new List<bool>();
            memoryObservable.Subscribe(results.Add);

            subject.OnNext(true);
            CollectionAssert.AreEqual(new[] { false, true }, results);
        }
    }
}
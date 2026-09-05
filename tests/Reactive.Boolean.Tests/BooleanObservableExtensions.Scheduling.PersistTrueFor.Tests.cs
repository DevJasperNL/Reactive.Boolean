using Microsoft.Reactive.Testing;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Reactive.Boolean.Tests
{
    [TestClass]
    public class BooleanObservableExtensionsSchedulingPersistTrueForTests
    {
        [TestMethod]
        [DataRow(true, true)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(false, false)]
        public void PersistTrueFor_InitialValue(bool resetTimerOnConsecutiveFalse, bool initialValue)
        {
            // Arrange
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.PersistTrueFor(TimeSpan.FromMinutes(1), scheduler, resetTimerOnConsecutiveFalse);
            
            bool? result = null;
            memoryObservable.Subscribe(b => result = b);

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
        public void PersistTrueFor_InitialValue_IsDistinct(bool resetTimerOnConsecutiveFalse, bool initialValue)
        {
            // Arrange
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.PersistTrueFor(TimeSpan.FromMinutes(1), scheduler, resetTimerOnConsecutiveFalse);

            var results = new List<bool>();
            memoryObservable.Subscribe(results.Add);

            // Act
            subject.OnNext(initialValue);
            subject.OnNext(initialValue);

            // Assert
            CollectionAssert.AreEqual(new[] { initialValue }, results);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void PersistTrueFor_RemainsTrue(bool resetTimerOnConsecutiveFalse)
        {
            // Arrange
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.PersistTrueFor(TimeSpan.FromMinutes(1), scheduler, resetTimerOnConsecutiveFalse);

            bool? result = null;
            memoryObservable.Subscribe(b => result = b);

            subject.OnNext(true);

            // Act
            subject.OnNext(false);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void PersistTrueFor_RemainsTrueForTimeSpan(bool resetTimerOnConsecutiveFalse)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.PersistTrueFor(TimeSpan.FromTicks(2), scheduler, resetTimerOnConsecutiveFalse);

            bool? result = null;
            memoryObservable.Subscribe(b => result = b);

            subject.OnNext(true);
            subject.OnNext(false);
            Assert.IsTrue(result);

            scheduler.AdvanceBy(1);
            Assert.IsTrue(result);
            
            scheduler.AdvanceBy(1);
            Assert.IsFalse(result);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void PersistTrueFor_RemainsTrueForTimeSpan_Repeat(bool resetTimerOnConsecutiveFalse)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.PersistTrueFor(TimeSpan.FromTicks(2), scheduler, resetTimerOnConsecutiveFalse);

            bool? result = null;
            memoryObservable.Subscribe(b => result = b);

            subject.OnNext(true);
            subject.OnNext(false);
            Assert.IsTrue(result);

            scheduler.AdvanceBy(1);
            Assert.IsTrue(result);

            scheduler.AdvanceBy(1);
            Assert.IsFalse(result);

            subject.OnNext(true);
            subject.OnNext(false);
            Assert.IsTrue(result);

            scheduler.AdvanceBy(1);
            Assert.IsTrue(result);

            scheduler.AdvanceBy(1);
            Assert.IsFalse(result);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void PersistTrueFor_RemainsTrueForTimeSpanAfterFalse(bool resetTimerOnConsecutiveFalse)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.PersistTrueFor(TimeSpan.FromTicks(2), scheduler, resetTimerOnConsecutiveFalse);

            bool? result = null;
            memoryObservable.Subscribe(b => result = b);

            subject.OnNext(true);
            Assert.IsTrue(result);

            scheduler.AdvanceBy(1);
            subject.OnNext(false);
            Assert.IsTrue(result);

            scheduler.AdvanceBy(1);
            Assert.IsTrue(result);

            scheduler.AdvanceBy(1);
            Assert.IsFalse(result);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void PersistTrueFor_TrueResetsTimer(bool resetTimerOnConsecutiveFalse)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.PersistTrueFor(TimeSpan.FromTicks(2), scheduler, resetTimerOnConsecutiveFalse);

            bool? result = null;
            memoryObservable.Subscribe(b => result = b);

            subject.OnNext(true);
            subject.OnNext(false);
            Assert.IsTrue(result);

            scheduler.AdvanceBy(1);
            subject.OnNext(true);
            subject.OnNext(false);

            scheduler.AdvanceBy(1);
            Assert.IsTrue(result);

            scheduler.AdvanceBy(1);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void PersistTrueFor_TimerNotResetOnConsecutiveFalse()
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.PersistTrueFor(TimeSpan.FromTicks(2), scheduler);

            var results = new List<bool>();
            memoryObservable.Subscribe(results.Add);

            subject.OnNext(true);
            subject.OnNext(false);
            CollectionAssert.AreEqual(new[] { true }, results);

            scheduler.AdvanceBy(1);
            subject.OnNext(false); // Should not reset timer
            CollectionAssert.AreEqual(new[] { true }, results);

            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(new[] { true, false }, results);
        }
        
        [TestMethod]
        public void PersistTrueFor_TimerResetOnConsecutiveFalse()
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.PersistTrueFor(TimeSpan.FromTicks(2), scheduler, resetTimerOnConsecutiveFalse: true);

            var results = new List<bool>();
            memoryObservable.Subscribe(results.Add);

            subject.OnNext(true);
            subject.OnNext(false);
            CollectionAssert.AreEqual(new[] { true }, results);

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
        public void PersistTrueFor_CompleteIsImmediate(bool resetTimerOnConsecutiveFalse)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.PersistTrueFor(TimeSpan.FromTicks(2), scheduler, resetTimerOnConsecutiveFalse);

            bool? result = null;
            bool completed = false;
            memoryObservable.Subscribe(b => result = b, _ => { }, () => completed = true);

            subject.OnNext(true);
            subject.OnNext(false);

            scheduler.AdvanceBy(1);
            Assert.IsTrue(result);

            subject.OnCompleted();
            Assert.IsTrue(completed);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void PersistTrueFor_Error(bool resetTimerOnConsecutiveFalse)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.PersistTrueFor(TimeSpan.FromTicks(2), scheduler, resetTimerOnConsecutiveFalse);

            bool? result = null;
            Exception? receivedException = null;
            memoryObservable.Subscribe(b => result = b, e => receivedException = e);

            subject.OnNext(true);
            subject.OnNext(false);

            scheduler.AdvanceBy(1);
            Assert.IsTrue(result);

            var exception = new InvalidOperationException("This is a test");
            subject.OnError(exception);
            Assert.AreEqual(receivedException, exception);
        }

        [TestMethod]
        [DataRow(false, true)]
        [DataRow(false, false)]
        [DataRow(true, true)]
        [DataRow(true, false)]
        public void PersistTrueFor_InitialValue_NotDistinct(bool resetTimerOnConsecutiveFalse, bool initialValue)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.PersistTrueFor(TimeSpan.FromMinutes(1), scheduler, resetTimerOnConsecutiveFalse, distinctUntilChanged: false);

            var results = new List<bool>();
            memoryObservable.Subscribe(results.Add);

            subject.OnNext(initialValue);
            subject.OnNext(initialValue);

            CollectionAssert.AreEqual(new[] { initialValue, initialValue }, results);
        }

        [TestMethod]
        public void PersistTrueFor_ConsecutiveFalseDuringTimer_NotDistinct_EmitsSingleFalse()
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.PersistTrueFor(TimeSpan.FromTicks(2), scheduler, distinctUntilChanged: false);

            var results = new List<bool>();
            memoryObservable.Subscribe(results.Add);

            subject.OnNext(true);
            subject.OnNext(false);
            subject.OnNext(false);
            CollectionAssert.AreEqual(new[] { true }, results);

            scheduler.AdvanceBy(2);
            CollectionAssert.AreEqual(new[] { true, false }, results);

            subject.OnNext(false); // Timer is no longer running, so consecutive "false" values are emitted again.
            CollectionAssert.AreEqual(new[] { true, false, false }, results);
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
        public void PersistTrueFor_TrueThenFalse_SameForEverySourceShape(SourceShape shape, bool resetTimerOnConsecutiveFalse, bool distinctUntilChanged)
        {
            var scheduler = new TestScheduler();
            var (source, start) = SourceShapes.Create(shape, true, false);
            var memoryObservable = source.PersistTrueFor(TimeSpan.FromTicks(2), scheduler, resetTimerOnConsecutiveFalse, distinctUntilChanged, CompletionBehavior.CompleteAfterTimer);

            var results = new List<bool>();
            memoryObservable.Subscribe(results.Add);
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
        public void PersistTrueFor_SubscribesToSourceOnce(bool resetTimerOnConsecutiveFalse, bool distinctUntilChanged)
        {
            var subscriptions = 0;
            var source = Observable.Defer(() =>
            {
                subscriptions++;
                return Observable.Never<bool>();
            });

            source.PersistTrueFor(TimeSpan.FromTicks(2), new TestScheduler(), resetTimerOnConsecutiveFalse, distinctUntilChanged).Subscribe();

            Assert.AreEqual(1, subscriptions);
        }

        [TestMethod]
        [DataRow(false, false)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(true, true)]
        public void PersistTrueFor_CompleteImmediately_DropsDelayedFalse(bool resetTimerOnConsecutiveFalse, bool distinctUntilChanged)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.PersistTrueFor(TimeSpan.FromTicks(2), scheduler, resetTimerOnConsecutiveFalse, distinctUntilChanged, CompletionBehavior.CompleteImmediately);

            var results = new List<bool>();
            var completed = false;
            memoryObservable.Subscribe(results.Add, () => completed = true);

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
        public void PersistTrueFor_CompleteAfterTimer_EmitsDelayedFalseThenCompletes(bool resetTimerOnConsecutiveFalse, bool distinctUntilChanged)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.PersistTrueFor(TimeSpan.FromTicks(2), scheduler, resetTimerOnConsecutiveFalse, distinctUntilChanged, CompletionBehavior.CompleteAfterTimer);

            var results = new List<bool>();
            var completed = false;
            memoryObservable.Subscribe(results.Add, () => completed = true);

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
        public void PersistTrueFor_CompleteAfterTimer_NothingDelayed_CompletesImmediately(bool resetTimerOnConsecutiveFalse, bool distinctUntilChanged)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.PersistTrueFor(TimeSpan.FromTicks(2), scheduler, resetTimerOnConsecutiveFalse, distinctUntilChanged, CompletionBehavior.CompleteAfterTimer);

            var results = new List<bool>();
            var completed = false;
            memoryObservable.Subscribe(results.Add, () => completed = true);

            subject.OnNext(true);
            subject.OnNext(false);
            subject.OnNext(true); // Cancels the delayed "false".
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
        public void PersistTrueFor_DisposeCancelsTimer(bool resetTimerOnConsecutiveFalse, bool distinctUntilChanged)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.PersistTrueFor(TimeSpan.FromTicks(2), scheduler, resetTimerOnConsecutiveFalse, distinctUntilChanged);

            var results = new List<bool>();
            var subscription = memoryObservable.Subscribe(results.Add);

            subject.OnNext(true);
            subject.OnNext(false);
            subscription.Dispose();

            scheduler.AdvanceBy(2);
            CollectionAssert.AreEqual(new[] { true }, results);
        }
        [TestMethod]
        public void PersistTrueFor_ZeroOrNegativeTimeSpan_Throws()
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => subject.PersistTrueFor(TimeSpan.Zero, scheduler));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => subject.PersistTrueFor(TimeSpan.FromTicks(-1), scheduler));
        }

    }
}
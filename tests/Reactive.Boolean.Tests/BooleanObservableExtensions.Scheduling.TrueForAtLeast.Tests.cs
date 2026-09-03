using Microsoft.Reactive.Testing;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Reactive.Boolean.Tests
{
    [TestClass]
    public class BooleanObservableExtensionsSchedulingTrueForAtLeastTests
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
        public void TrueForAtLeast_InitialValue(bool distinctUntilChanged, bool resetTimerOnConsecutiveTrue, bool initialValue)
        {
            // Arrange
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.TrueForAtLeast(TimeSpan.FromMinutes(1), scheduler, distinctUntilChanged, resetTimerOnConsecutiveTrue);
            
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
        public void TrueForAtLeast_InitialValue_Distinct(bool resetTimerOnConsecutiveTrue, bool initialValue)
        {
            // Arrange
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.TrueForAtLeast(TimeSpan.FromMinutes(1), scheduler, resetTimerOnConsecutiveTrue: resetTimerOnConsecutiveTrue);

            var results = new List<bool>();
            memoryObservable.Subscribe(results.Add);

            // Act
            subject.OnNext(initialValue);
            subject.OnNext(initialValue);

            // Assert
            CollectionAssert.AreEqual(new[] { initialValue }, results);
        }

        [TestMethod]
        [DataRow(false, true)]
        [DataRow(false, false)]
        [DataRow(true, true)]
        [DataRow(true, false)]
        public void TrueForAtLeast_InitialValue_NotDistinct(bool resetTimerOnConsecutiveTrue, bool initialValue)
        {
            // Arrange
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable =
                subject.TrueForAtLeast(TimeSpan.FromMinutes(1), scheduler, distinctUntilChanged: false, resetTimerOnConsecutiveTrue);

            var results = new List<bool>();
            memoryObservable.Subscribe(results.Add);

            // Act
            subject.OnNext(initialValue);
            subject.OnNext(initialValue);

            // Assert
            CollectionAssert.AreEqual(new[] { initialValue, initialValue }, results);
        }

        [TestMethod]
        [DataRow(false, false)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(true, true)]
        public void TrueForAtLeast_RemainsTrue(bool distinctUntilChanged, bool resetTimerOnConsecutiveTrue)
        {
            // Arrange
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.TrueForAtLeast(TimeSpan.FromMinutes(1), scheduler, distinctUntilChanged, resetTimerOnConsecutiveTrue);

            bool? result = null;
            memoryObservable.Subscribe(b => result = b);
            subject.OnNext(true);

            // Act
            subject.OnNext(false);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        [DataRow(false, false)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(true, true)]
        public void TrueForAtLeast_RemainsTrueForTimeSpan(bool distinctUntilChanged, bool resetTimerOnConsecutiveTrue)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.TrueForAtLeast(TimeSpan.FromTicks(2), scheduler, distinctUntilChanged, resetTimerOnConsecutiveTrue);

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
        [DataRow(false, false)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(true, true)]
        public void TrueForAtLeast_RemainsTrueForTimeSpan_Repeat(bool distinctUntilChanged, bool resetTimerOnConsecutiveTrue)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.TrueForAtLeast(TimeSpan.FromTicks(2), scheduler, distinctUntilChanged, resetTimerOnConsecutiveTrue);

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
        [DataRow(false, false)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(true, true)]
        public void TrueForAtLeast_CanTurnFalseAfterTimeSpan(bool distinctUntilChanged, bool resetTimerOnConsecutiveTrue)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.TrueForAtLeast(TimeSpan.FromTicks(2), scheduler, distinctUntilChanged, resetTimerOnConsecutiveTrue);

            bool? result = null;
            memoryObservable.Subscribe(b => result = b);

            subject.OnNext(true);
            Assert.IsTrue(result);

            scheduler.AdvanceBy(1);
            scheduler.AdvanceBy(1);
            Assert.IsTrue(result);

            subject.OnNext(false);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TrueForAtLeast_TimerNotResetOnConsecutiveTrue_Distinct()
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.TrueForAtLeast(TimeSpan.FromTicks(2), scheduler);

            var results = new List<bool>();
            memoryObservable.Subscribe(results.Add);

            subject.OnNext(true);
            CollectionAssert.AreEqual(new[] { true }, results);

            scheduler.AdvanceBy(1);
            subject.OnNext(true); // Should not reset timer
            subject.OnNext(false);
            CollectionAssert.AreEqual(new[] { true }, results);

            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(new[] { true, false }, results);

            subject.OnNext(false);
            CollectionAssert.AreEqual(new[] { true, false }, results);
        }

        [TestMethod]
        public void TrueForAtLeast_TimerNotResetOnConsecutiveTrue_NotDistinct()
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.TrueForAtLeast(TimeSpan.FromTicks(2), scheduler, distinctUntilChanged: false);

            var results = new List<bool>();
            memoryObservable.Subscribe(results.Add);

            subject.OnNext(true);
            CollectionAssert.AreEqual(new[] { true }, results);

            scheduler.AdvanceBy(1);
            subject.OnNext(true); // Should not reset timer
            subject.OnNext(false);
            CollectionAssert.AreEqual(new[] { true, true }, results);

            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(new[] { true, true, false }, results);

            subject.OnNext(false);
            CollectionAssert.AreEqual(new[] { true, true, false, false }, results);
        }

        [TestMethod]
        public void TrueForAtLeast_TimerResetOnConsecutiveTrue_Distinct()
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.TrueForAtLeast(TimeSpan.FromTicks(2), scheduler, resetTimerOnConsecutiveTrue: true);

            var results = new List<bool>();
            memoryObservable.Subscribe(results.Add);

            subject.OnNext(true);
            CollectionAssert.AreEqual(new[] { true }, results);

            scheduler.AdvanceBy(1);
            subject.OnNext(true); // Should reset timer
            subject.OnNext(false);

            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(new[] { true }, results);

            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(new[] { true, false }, results);

            subject.OnNext(false);
            CollectionAssert.AreEqual(new[] { true, false }, results);
        }

        [TestMethod]
        public void TrueForAtLeast_TimerResetOnConsecutiveTrue_NotDistinct()
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.TrueForAtLeast(TimeSpan.FromTicks(2), scheduler, distinctUntilChanged: false, resetTimerOnConsecutiveTrue: true);

            var results = new List<bool>();
            memoryObservable.Subscribe(results.Add);

            subject.OnNext(true);
            CollectionAssert.AreEqual(new[] { true }, results);

            scheduler.AdvanceBy(1);
            subject.OnNext(true); // Should reset timer
            subject.OnNext(false);

            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(new[] { true, true }, results);

            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(new[] { true, true, false }, results);

            subject.OnNext(false);
            CollectionAssert.AreEqual(new[] { true, true, false, false }, results);
        }

        [TestMethod]
        [DataRow(false, false)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(true, true)]
        public void TrueForAtLeast_CompleteIsImmediate(bool distinctUntilChanged, bool resetTimerOnConsecutiveTrue)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.TrueForAtLeast(TimeSpan.FromTicks(2), scheduler, distinctUntilChanged, resetTimerOnConsecutiveTrue);

            bool? result = null;
            var completed = false;
            memoryObservable.Subscribe(b => result = b, _ => { }, () => completed = true);

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
        public void TrueForAtLeast_Error(bool distinctUntilChanged, bool resetTimerOnConsecutiveTrue)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.TrueForAtLeast(TimeSpan.FromTicks(2), scheduler, distinctUntilChanged, resetTimerOnConsecutiveTrue);

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
        public void TrueForAtLeast_TrueThenFalse_SameForEverySourceShape(SourceShape shape, bool distinctUntilChanged, bool resetTimerOnConsecutiveTrue)
        {
            var scheduler = new TestScheduler();
            var (source, start) = SourceShapes.Create(shape, true, false);
            var memoryObservable = source.TrueForAtLeast(TimeSpan.FromTicks(2), scheduler, distinctUntilChanged, resetTimerOnConsecutiveTrue, CompletionBehavior.CompleteAfterTimer);

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
        public void TrueForAtLeast_SubscribesToSourceOnce(bool distinctUntilChanged, bool resetTimerOnConsecutiveTrue)
        {
            var subscriptions = 0;
            var source = Observable.Defer(() =>
            {
                subscriptions++;
                return Observable.Never<bool>();
            });

            source.TrueForAtLeast(TimeSpan.FromTicks(2), new TestScheduler(), distinctUntilChanged, resetTimerOnConsecutiveTrue).Subscribe();

            Assert.AreEqual(1, subscriptions);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void TrueForAtLeast_TrueAfterFalseInsideTimer_RestartsTimer(bool distinctUntilChanged)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.TrueForAtLeast(TimeSpan.FromTicks(3), scheduler, distinctUntilChanged);

            var results = new List<bool>();
            memoryObservable.Subscribe(results.Add);

            subject.OnNext(true);
            scheduler.AdvanceBy(1);
            subject.OnNext(false);
            scheduler.AdvanceBy(1);
            subject.OnNext(true); // Not consecutive: a "true" after a "false" restarts the timer.
            subject.OnNext(false);

            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(distinctUntilChanged ? new[] { true } : new[] { true, true }, results);

            scheduler.AdvanceBy(2);
            CollectionAssert.AreEqual(distinctUntilChanged ? new[] { true, false } : new[] { true, true, false }, results);
        }

        [TestMethod]
        [DataRow(false, false)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(true, true)]
        public void TrueForAtLeast_CompleteImmediately_DropsWithheldFalse(bool distinctUntilChanged, bool resetTimerOnConsecutiveTrue)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.TrueForAtLeast(TimeSpan.FromTicks(2), scheduler, distinctUntilChanged, resetTimerOnConsecutiveTrue, CompletionBehavior.CompleteImmediately);

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
        public void TrueForAtLeast_CompleteAfterTimer_EmitsWithheldFalseThenCompletes(bool distinctUntilChanged, bool resetTimerOnConsecutiveTrue)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.TrueForAtLeast(TimeSpan.FromTicks(2), scheduler, distinctUntilChanged, resetTimerOnConsecutiveTrue, CompletionBehavior.CompleteAfterTimer);

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
        public void TrueForAtLeast_CompleteAfterTimer_NothingWithheld_CompletesImmediately(bool distinctUntilChanged, bool resetTimerOnConsecutiveTrue)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.TrueForAtLeast(TimeSpan.FromTicks(2), scheduler, distinctUntilChanged, resetTimerOnConsecutiveTrue, CompletionBehavior.CompleteAfterTimer);

            var results = new List<bool>();
            var completed = false;
            memoryObservable.Subscribe(results.Add, () => completed = true);

            subject.OnNext(true);
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
        public void TrueForAtLeast_DisposeCancelsTimer(bool distinctUntilChanged, bool resetTimerOnConsecutiveTrue)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.TrueForAtLeast(TimeSpan.FromTicks(2), scheduler, distinctUntilChanged, resetTimerOnConsecutiveTrue);

            var results = new List<bool>();
            var subscription = memoryObservable.Subscribe(results.Add);

            subject.OnNext(true);
            subject.OnNext(false);
            subscription.Dispose();

            scheduler.AdvanceBy(2);
            CollectionAssert.AreEqual(new[] { true }, results);
        }
    }
}
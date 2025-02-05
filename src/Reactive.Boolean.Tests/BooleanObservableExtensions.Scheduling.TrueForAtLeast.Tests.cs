using Microsoft.Reactive.Testing;
using System;
using System.Reactive.Subjects;

namespace Reactive.Boolean.Tests
{
    [TestClass]
    public class BooleanObservableExtensionsSchedulingTrueForAtLeastTests
    {
        [DataTestMethod]
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

        [DataTestMethod]
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

        [DataTestMethod]
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

        [DataTestMethod]
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
            Assert.AreEqual(true, result);
        }

        [DataTestMethod]
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
            Assert.AreEqual(true, result);

            scheduler.AdvanceBy(1);
            Assert.AreEqual(true, result);
            
            scheduler.AdvanceBy(1);
            Assert.AreEqual(false, result);
        }

        [DataTestMethod]
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
            Assert.AreEqual(true, result);

            scheduler.AdvanceBy(1);
            Assert.AreEqual(true, result);

            scheduler.AdvanceBy(1);
            Assert.AreEqual(false, result);

            subject.OnNext(true);
            subject.OnNext(false);
            Assert.AreEqual(true, result);

            scheduler.AdvanceBy(1);
            Assert.AreEqual(true, result);

            scheduler.AdvanceBy(1);
            Assert.AreEqual(false, result);
        }

        [DataTestMethod]
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
            Assert.AreEqual(true, result);

            scheduler.AdvanceBy(1);
            scheduler.AdvanceBy(1);
            Assert.AreEqual(true, result);

            subject.OnNext(false);
            Assert.AreEqual(false, result);
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

        [DataTestMethod]
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
            Assert.AreEqual(true, result);

            subject.OnCompleted();
            Assert.IsTrue(completed);
        }

        [DataTestMethod]
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
            Assert.AreEqual(true, result);

            var exception = new InvalidOperationException("This is a test");
            subject.OnError(exception);
            Assert.AreEqual(receivedException, exception);
        }
    }
}
using Microsoft.Reactive.Testing;
using System.Reactive.Subjects;

namespace Reactive.Boolean.Tests
{
    [TestClass]
    public class BooleanObservableExtensionsSchedulingPersistTrueForTests
    {
        [DataTestMethod]
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

        [DataTestMethod]
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

        [DataTestMethod]
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
            Assert.AreEqual(true, result);
        }

        [DataTestMethod]
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
            Assert.AreEqual(true, result);

            scheduler.AdvanceBy(1);
            Assert.AreEqual(true, result);
            
            scheduler.AdvanceBy(1);
            Assert.AreEqual(false, result);
        }

        [DataTestMethod]
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
            Assert.AreEqual(true, result);

            scheduler.AdvanceBy(1);
            subject.OnNext(false);
            Assert.AreEqual(true, result);

            scheduler.AdvanceBy(1);
            Assert.AreEqual(true, result);

            scheduler.AdvanceBy(1);
            Assert.AreEqual(false, result);
        }

        [DataTestMethod]
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
            Assert.AreEqual(true, result);

            scheduler.AdvanceBy(1);
            subject.OnNext(true);
            subject.OnNext(false);

            scheduler.AdvanceBy(1);
            Assert.AreEqual(true, result);

            scheduler.AdvanceBy(1);
            Assert.AreEqual(false, result);
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

        [DataTestMethod]
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
            Assert.AreEqual(true, result);

            subject.OnCompleted();
            Assert.IsTrue(completed);
        }

        [DataTestMethod]
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
            Assert.AreEqual(true, result);

            var exception = new InvalidOperationException("This is a test");
            subject.OnError(exception);
            Assert.AreEqual(receivedException, exception);
        }
    }
}
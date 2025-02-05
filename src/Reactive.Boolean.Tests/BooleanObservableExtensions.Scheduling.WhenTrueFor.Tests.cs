using Microsoft.Reactive.Testing;
using System.Reactive.Subjects;

namespace Reactive.Boolean.Tests
{
    [TestClass]
    public class BooleanObservableExtensionsSchedulingWhenTrueForTests
    {
        [DataTestMethod]
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
            Assert.AreEqual(false, result);
        }

        [DataTestMethod]
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

        [DataTestMethod]
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
            Assert.AreEqual(false, result);

            scheduler.AdvanceBy(1);
            Assert.AreEqual(false, result);

            scheduler.AdvanceBy(1);
            Assert.AreEqual(true, result);
        }

        [DataTestMethod]
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
            Assert.AreEqual(false, result);

            scheduler.AdvanceBy(1);
            Assert.AreEqual(false, result);

            scheduler.AdvanceBy(1);
            Assert.AreEqual(true, result);

            subject.OnNext(false);
            subject.OnNext(true);
            Assert.AreEqual(false, result);

            scheduler.AdvanceBy(1);
            Assert.AreEqual(false, result);

            scheduler.AdvanceBy(1);
            Assert.AreEqual(true, result);
        }

        [DataTestMethod]
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
            Assert.AreEqual(true, result);

            subject.OnNext(false);
            Assert.AreEqual(false, result);
        }

        [DataTestMethod]
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
            Assert.AreEqual(false, result);

            scheduler.AdvanceBy(1);
            Assert.AreEqual(false, result);

            scheduler.AdvanceBy(1);
            Assert.AreEqual(true, result);
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
            Assert.AreEqual(false, result);

            scheduler.AdvanceBy(1);
            Assert.AreEqual(true, result);
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

        [DataTestMethod]
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
            Assert.AreEqual(false, result);

            subject.OnCompleted();
            Assert.IsTrue(completed);
        }

        [DataTestMethod]
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
            Assert.AreEqual(false, result);

            var exception = new InvalidOperationException("This is a test");
            subject.OnError(exception);
            Assert.AreEqual(receivedException, exception);
        }
    }
}
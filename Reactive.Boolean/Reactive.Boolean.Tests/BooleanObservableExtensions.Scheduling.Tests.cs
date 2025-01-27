using Microsoft.Reactive.Testing;
using System.Reactive.Subjects;

namespace Reactive.Boolean.Tests
{
    [TestClass]
    public class BooleanObservableExtensionsSchedulingTests
    {
        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void PersistTrueFor_InitialValue(bool initialValue)
        {
            // Arrange
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.PersistTrueFor(scheduler, TimeSpan.FromMinutes(1));
            
            bool? result = null;
            memoryObservable.Subscribe(b => result = b);

            // Act
            subject.OnNext(initialValue);

            // Assert
            Assert.AreEqual(initialValue, result);
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void PersistTrueFor_InitialValue_IsDistinct(bool initialValue)
        {
            // Arrange
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.PersistTrueFor(scheduler, TimeSpan.FromMinutes(1));

            var results = new List<bool>();
            memoryObservable.Subscribe(results.Add);

            // Act
            subject.OnNext(initialValue);
            subject.OnNext(initialValue);

            // Assert
            CollectionAssert.AreEqual(new[] { initialValue }, results);
        }

        [TestMethod]
        public void PersistTrueFor_RemainsTrue()
        {
            // Arrange
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.PersistTrueFor(scheduler, TimeSpan.FromMinutes(1));

            bool? result = null;
            memoryObservable.Subscribe(b => result = b);
            subject.OnNext(true);

            // Act
            subject.OnNext(false);

            // Assert
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void PersistTrueFor_RemainsTrueForTimeSpan()
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.PersistTrueFor(scheduler, TimeSpan.FromTicks(2));

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
        public void WhenTrueFor_InitialValueAlwaysFalse(bool initialValue)
        {
            // Arrange
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.WhenTrueFor(scheduler, TimeSpan.FromMinutes(1));

            bool? result = null;
            memoryObservable.Subscribe(b => result = b);

            // Act
            subject.OnNext(initialValue);

            // Assert
            Assert.AreEqual(false, result);
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void WhenTrueFor_InitialValue_IsDistinct(bool initialValue)
        {
            // Arrange
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.WhenTrueFor(scheduler, TimeSpan.FromMinutes(1));

            var results = new List<bool>();
            memoryObservable.Subscribe(results.Add);

            // Act
            subject.OnNext(initialValue);
            subject.OnNext(initialValue);

            // Assert
            CollectionAssert.AreEqual(new[] { false }, results);
        }

        [TestMethod]
        public void WhenTrueFor_TrueAfterTimeSpan()
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.WhenTrueFor(scheduler, TimeSpan.FromTicks(2));

            bool? result = null;
            memoryObservable.Subscribe(b => result = b);
            
            subject.OnNext(true);
            Assert.AreEqual(false, result);

            scheduler.AdvanceBy(1);
            Assert.AreEqual(false, result);

            scheduler.AdvanceBy(1);
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void WhenTrueFor_FalseIsImmediate()
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.WhenTrueFor(scheduler, TimeSpan.FromTicks(1));

            bool? result = null;
            memoryObservable.Subscribe(b => result = b);

            subject.OnNext(true);
            scheduler.AdvanceBy(1);
            Assert.AreEqual(true, result);

            subject.OnNext(false);
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void WhenTrueFor_FalseResetsTimer()
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.WhenTrueFor(scheduler, TimeSpan.FromTicks(2));

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
        public void WhenTrueFor_TrueDoesNotResetTimer()
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.WhenTrueFor(scheduler, TimeSpan.FromTicks(2));

            bool? result = null;
            memoryObservable.Subscribe(b => result = b);

            subject.OnNext(true);

            scheduler.AdvanceBy(1);
            subject.OnNext(true);

            scheduler.AdvanceBy(1);
            Assert.AreEqual(true, result);
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void LimitTrueDuration_InitialValue(bool initialValue)
        {
            // Arrange
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.LimitTrueDuration(scheduler, TimeSpan.FromMinutes(1));

            bool? result = null;
            memoryObservable.Subscribe(b => result = b);

            // Act
            subject.OnNext(initialValue);

            // Assert
            Assert.AreEqual(initialValue, result);
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void LimitTrueDuration_InitialValue_IsDistinct(bool initialValue)
        {
            // Arrange
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.LimitTrueDuration(scheduler, TimeSpan.FromMinutes(1));

            bool? result = null;
            memoryObservable.Subscribe(b => result = b);

            // Act
            subject.OnNext(initialValue);
            subject.OnNext(initialValue);

            // Assert
            Assert.AreEqual(initialValue, result);
        }

        [TestMethod]
        public void LimitTrueDuration_FalseAfterTimeSpan()
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.LimitTrueDuration(scheduler, TimeSpan.FromTicks(2));

            bool? result = null;
            memoryObservable.Subscribe(b => result = b);

            subject.OnNext(true);
            Assert.AreEqual(true, result);

            scheduler.AdvanceBy(1);
            Assert.AreEqual(true, result);

            scheduler.AdvanceBy(1);
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void LimitTrueDuration_FalseIsImmediate()
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var memoryObservable = subject.LimitTrueDuration(scheduler, TimeSpan.FromTicks(2));

            bool? result = null;
            memoryObservable.Subscribe(b => result = b);

            subject.OnNext(true);
            Assert.AreEqual(true, result);

            scheduler.AdvanceBy(1);
            Assert.AreEqual(true, result);

            subject.OnNext(false);
            Assert.AreEqual(false, result);
        }
    }
}
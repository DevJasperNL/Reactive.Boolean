using Microsoft.Reactive.Testing;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Reactive.Boolean.Tests
{
    [TestClass]
    public class BooleanObservableExtensionsSchedulingPulseTrueForTests
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
        public void PulseTrueFor_InitialValue(bool distinctUntilChanged, bool resetTimerOnConsecutiveTrue, bool initialValue)
        {
            // Arrange
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var pulseObservable = subject.PulseTrueFor(TimeSpan.FromMinutes(1), scheduler, distinctUntilChanged, resetTimerOnConsecutiveTrue);

            bool? result = null;
            pulseObservable.Subscribe(b => result = b);

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
        public void PulseTrueFor_EndsAfterTimeSpanWhileSourceStillTrue(bool distinctUntilChanged, bool resetTimerOnConsecutiveTrue)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var pulseObservable = subject.PulseTrueFor(TimeSpan.FromTicks(2), scheduler, distinctUntilChanged, resetTimerOnConsecutiveTrue);

            var results = new List<bool>();
            pulseObservable.Subscribe(results.Add);

            subject.OnNext(true);
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
        public void PulseTrueFor_FalseDuringPulseIsWithheld(bool distinctUntilChanged, bool resetTimerOnConsecutiveTrue)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var pulseObservable = subject.PulseTrueFor(TimeSpan.FromTicks(2), scheduler, distinctUntilChanged, resetTimerOnConsecutiveTrue);

            var results = new List<bool>();
            pulseObservable.Subscribe(results.Add);

            subject.OnNext(true);
            subject.OnNext(false);
            CollectionAssert.AreEqual(new[] { true }, results);

            scheduler.AdvanceBy(1);
            subject.OnNext(false);
            CollectionAssert.AreEqual(new[] { true }, results);

            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(new[] { true, false }, results);
        }

        [TestMethod]
        [DataRow(false, false)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(true, true)]
        public void PulseTrueFor_FalseAfterPulse(bool distinctUntilChanged, bool resetTimerOnConsecutiveTrue)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var pulseObservable = subject.PulseTrueFor(TimeSpan.FromTicks(2), scheduler, distinctUntilChanged, resetTimerOnConsecutiveTrue);

            var results = new List<bool>();
            pulseObservable.Subscribe(results.Add);

            subject.OnNext(true);
            scheduler.AdvanceBy(2);
            CollectionAssert.AreEqual(new[] { true, false }, results);

            subject.OnNext(false);
            CollectionAssert.AreEqual(distinctUntilChanged ? new[] { true, false } : new[] { true, false, false }, results);
        }

        [TestMethod]
        [DataRow(false, false)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(true, true)]
        public void PulseTrueFor_TrueAfterFalseDuringPulse_RestartsPulse(bool distinctUntilChanged, bool resetTimerOnConsecutiveTrue)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var pulseObservable = subject.PulseTrueFor(TimeSpan.FromTicks(2), scheduler, distinctUntilChanged, resetTimerOnConsecutiveTrue);

            var results = new List<bool>();
            pulseObservable.Subscribe(results.Add);

            subject.OnNext(true);
            scheduler.AdvanceBy(1);
            subject.OnNext(false);
            subject.OnNext(true);
            var expected = distinctUntilChanged ? new[] { true } : new[] { true, true };
            CollectionAssert.AreEqual(expected, results);

            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(expected, results);

            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(expected.Append(false).ToArray(), results);
        }

        [TestMethod]
        public void PulseTrueFor_TimerNotResetOnConsecutiveTrue_Distinct()
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var pulseObservable = subject.PulseTrueFor(TimeSpan.FromTicks(2), scheduler);

            var results = new List<bool>();
            pulseObservable.Subscribe(results.Add);

            subject.OnNext(true);
            CollectionAssert.AreEqual(new[] { true }, results);

            scheduler.AdvanceBy(1);
            subject.OnNext(true); // Should not reset timer
            CollectionAssert.AreEqual(new[] { true }, results);

            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(new[] { true, false }, results);
        }

        [TestMethod]
        public void PulseTrueFor_TimerNotResetOnConsecutiveTrue_NotDistinct()
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var pulseObservable = subject.PulseTrueFor(TimeSpan.FromTicks(2), scheduler, distinctUntilChanged: false);

            var results = new List<bool>();
            pulseObservable.Subscribe(results.Add);

            subject.OnNext(true);
            CollectionAssert.AreEqual(new[] { true }, results);

            scheduler.AdvanceBy(1);
            subject.OnNext(true); // Should not reset timer
            CollectionAssert.AreEqual(new[] { true, true }, results);

            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(new[] { true, true, false }, results);
        }

        [TestMethod]
        public void PulseTrueFor_TimerResetOnConsecutiveTrue_Distinct()
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var pulseObservable = subject.PulseTrueFor(TimeSpan.FromTicks(2), scheduler, resetTimerOnConsecutiveTrue: true);

            var results = new List<bool>();
            pulseObservable.Subscribe(results.Add);

            subject.OnNext(true);
            CollectionAssert.AreEqual(new[] { true }, results);

            scheduler.AdvanceBy(1);
            subject.OnNext(true); // Should reset timer
            CollectionAssert.AreEqual(new[] { true }, results);

            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(new[] { true }, results);

            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(new[] { true, false }, results);
        }

        [TestMethod]
        public void PulseTrueFor_TimerResetOnConsecutiveTrue_NotDistinct()
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var pulseObservable = subject.PulseTrueFor(TimeSpan.FromTicks(2), scheduler, distinctUntilChanged: false, resetTimerOnConsecutiveTrue: true);

            var results = new List<bool>();
            pulseObservable.Subscribe(results.Add);

            subject.OnNext(true);
            CollectionAssert.AreEqual(new[] { true }, results);

            scheduler.AdvanceBy(1);
            subject.OnNext(true); // Should reset timer
            CollectionAssert.AreEqual(new[] { true, true }, results);

            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(new[] { true, true }, results);

            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(new[] { true, true, false }, results);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void PulseTrueFor_RepeatedTrueAfterPulse_IgnoredWithoutReset(bool distinctUntilChanged)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var pulseObservable = subject.PulseTrueFor(TimeSpan.FromTicks(2), scheduler, distinctUntilChanged);

            var results = new List<bool>();
            pulseObservable.Subscribe(results.Add);

            subject.OnNext(true);
            scheduler.AdvanceBy(2);
            CollectionAssert.AreEqual(new[] { true, false }, results);

            subject.OnNext(true); // The source is still "true" after the pulse ended.
            scheduler.AdvanceBy(2);
            CollectionAssert.AreEqual(new[] { true, false }, results);

            subject.OnNext(false);
            subject.OnNext(true); // Only a real "false" to "true" transition starts a new pulse.
            CollectionAssert.AreEqual(distinctUntilChanged ? new[] { true, false, true } : new[] { true, false, false, true }, results);

            scheduler.AdvanceBy(2);
            CollectionAssert.AreEqual(distinctUntilChanged ? new[] { true, false, true, false } : new[] { true, false, false, true, false }, results);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void PulseTrueFor_RepeatedTrueAfterPulse_RetriggersWithReset(bool distinctUntilChanged)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var pulseObservable = subject.PulseTrueFor(TimeSpan.FromTicks(2), scheduler, distinctUntilChanged, resetTimerOnConsecutiveTrue: true);

            var results = new List<bool>();
            pulseObservable.Subscribe(results.Add);

            subject.OnNext(true);
            scheduler.AdvanceBy(2);
            CollectionAssert.AreEqual(new[] { true, false }, results);

            subject.OnNext(true);
            CollectionAssert.AreEqual(new[] { true, false, true }, results);

            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(new[] { true, false, true }, results);

            scheduler.AdvanceBy(1);
            CollectionAssert.AreEqual(new[] { true, false, true, false }, results);
        }

        [TestMethod]
        [DataRow(false, false)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(true, true)]
        public void PulseTrueFor_CompleteIsImmediate(bool distinctUntilChanged, bool resetTimerOnConsecutiveTrue)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var pulseObservable = subject.PulseTrueFor(TimeSpan.FromTicks(2), scheduler, distinctUntilChanged, resetTimerOnConsecutiveTrue);

            bool? result = null;
            var completed = false;
            pulseObservable.Subscribe(b => result = b, _ => { }, () => completed = true);

            subject.OnNext(true);

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
        public void PulseTrueFor_Error(bool distinctUntilChanged, bool resetTimerOnConsecutiveTrue)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var pulseObservable = subject.PulseTrueFor(TimeSpan.FromTicks(2), scheduler, distinctUntilChanged, resetTimerOnConsecutiveTrue);

            bool? result = null;
            Exception? receivedException = null;
            pulseObservable.Subscribe(b => result = b, e => receivedException = e);

            subject.OnNext(true);

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
        public void PulseTrueFor_TrueThenFalse_SameForEverySourceShape(SourceShape shape, bool distinctUntilChanged, bool resetTimerOnConsecutiveTrue)
        {
            var scheduler = new TestScheduler();
            var (source, start) = SourceShapes.Create(shape, true, false);
            var pulseObservable = source.PulseTrueFor(TimeSpan.FromTicks(2), scheduler, distinctUntilChanged, resetTimerOnConsecutiveTrue, CompletionBehavior.CompleteAfterTimer);

            var results = new List<bool>();
            pulseObservable.Subscribe(results.Add);
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
        public void PulseTrueFor_SubscribesToSourceOnce(bool distinctUntilChanged, bool resetTimerOnConsecutiveTrue)
        {
            var subscriptions = 0;
            var source = Observable.Defer(() =>
            {
                subscriptions++;
                return Observable.Never<bool>();
            });

            source.PulseTrueFor(TimeSpan.FromTicks(2), new TestScheduler(), distinctUntilChanged, resetTimerOnConsecutiveTrue).Subscribe();

            Assert.AreEqual(1, subscriptions);
        }

        [TestMethod]
        [DataRow(false, false)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(true, true)]
        public void PulseTrueFor_CompleteImmediately_DoesNotEmitClosingFalse(bool distinctUntilChanged, bool resetTimerOnConsecutiveTrue)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var pulseObservable = subject.PulseTrueFor(TimeSpan.FromTicks(2), scheduler, distinctUntilChanged, resetTimerOnConsecutiveTrue, CompletionBehavior.CompleteImmediately);

            var results = new List<bool>();
            var completed = false;
            pulseObservable.Subscribe(results.Add, () => completed = true);

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
        public void PulseTrueFor_CompleteAfterTimer_EmitsClosingFalseThenCompletes(bool distinctUntilChanged, bool resetTimerOnConsecutiveTrue)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var pulseObservable = subject.PulseTrueFor(TimeSpan.FromTicks(2), scheduler, distinctUntilChanged, resetTimerOnConsecutiveTrue, CompletionBehavior.CompleteAfterTimer);

            var results = new List<bool>();
            var completed = false;
            pulseObservable.Subscribe(results.Add, () => completed = true);

            subject.OnNext(true);
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
        public void PulseTrueFor_CompleteAfterTimer_NoTimerRunning_CompletesImmediately(bool distinctUntilChanged, bool resetTimerOnConsecutiveTrue)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var pulseObservable = subject.PulseTrueFor(TimeSpan.FromTicks(2), scheduler, distinctUntilChanged, resetTimerOnConsecutiveTrue, CompletionBehavior.CompleteAfterTimer);

            var results = new List<bool>();
            var completed = false;
            pulseObservable.Subscribe(results.Add, () => completed = true);

            subject.OnNext(true);
            scheduler.AdvanceBy(2); // The pulse has ended.
            subject.OnCompleted();
            Assert.IsTrue(completed);

            CollectionAssert.AreEqual(new[] { true, false }, results);
        }

        [TestMethod]
        [DataRow(false, false)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(true, true)]
        public void PulseTrueFor_DisposeCancelsTimer(bool distinctUntilChanged, bool resetTimerOnConsecutiveTrue)
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var pulseObservable = subject.PulseTrueFor(TimeSpan.FromTicks(2), scheduler, distinctUntilChanged, resetTimerOnConsecutiveTrue);

            var results = new List<bool>();
            var subscription = pulseObservable.Subscribe(results.Add);

            subject.OnNext(true);
            subscription.Dispose();

            scheduler.AdvanceBy(2);
            CollectionAssert.AreEqual(new[] { true }, results);
        }

        [TestMethod]
        public void PulseTrueFor_ZeroTimeSpan_ReturnsSource()
        {
            var subject = new Subject<bool>();

            Assert.AreSame(subject, subject.PulseTrueFor(TimeSpan.Zero, new TestScheduler()));
        }

        [TestMethod]
        public void PulseFalseFor_IsInverse()
        {
            var subject = new Subject<bool>();
            var scheduler = new TestScheduler();
            var pulseObservable = subject.PulseFalseFor(TimeSpan.FromTicks(2), scheduler);

            var results = new List<bool>();
            pulseObservable.Subscribe(results.Add);

            subject.OnNext(false);
            subject.OnNext(true);
            CollectionAssert.AreEqual(new[] { false }, results);

            scheduler.AdvanceBy(2);
            CollectionAssert.AreEqual(new[] { false, true }, results);
        }
    }
}

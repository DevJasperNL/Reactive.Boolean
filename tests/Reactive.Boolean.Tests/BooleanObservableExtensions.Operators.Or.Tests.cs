using System.Reactive.Subjects;

namespace Reactive.Boolean.Tests
{
    [TestClass]
    public class BooleanObservableExtensionsOperatorsOrTests
    {
        [DataTestMethod]
        [DataRow(false, false, false)]
        [DataRow(false, true, true)]
        [DataRow(true, false, true)]
        [DataRow(true, true, true)]
        public void Or(bool input1, bool input2, bool expectedOutput)
        {
            // Arrange
            var subject1 = new Subject<bool>();
            var subject2 = new Subject<bool>();
            var or = subject1.Or(subject2);

            bool? result = null;
            or.Subscribe(b => result = b);

            // Act
            subject1.OnNext(input1);
            subject2.OnNext(input2);

            // Assert
            Assert.AreEqual(expectedOutput, result);
        }

        [DataTestMethod]
        [DataRow(false, false, false, false)]
        [DataRow(false, false, true, true)]
        [DataRow(false, true, false, true)]
        [DataRow(false, true, true, true)]
        [DataRow(true, false, false, true)]
        [DataRow(true, false, true, true)]
        [DataRow(true, true, false, true)]
        [DataRow(true, true, true, true)]
        public void Or_Multiple(bool input1, bool input2, bool input3, bool expectedOutput)
        {
            // Arrange
            var subject1 = new Subject<bool>();
            var subject2 = new Subject<bool>();
            var subject3 = new Subject<bool>();
            var or = subject1.Or(subject2, subject3);

            bool? result = null;
            or.Subscribe(b => result = b);

            // Act
            subject1.OnNext(input1);
            subject2.OnNext(input2);
            subject3.OnNext(input3);

            // Assert
            Assert.AreEqual(expectedOutput, result);
        }

        [TestMethod]
        public void Or_OutputDistinctUntilChanged()
        {
            var subject1 = new Subject<bool>();
            var subject2 = new Subject<bool>();
            var or = subject1.Or(subject2);

            var results = new List<bool>();
            or.Subscribe(results.Add);

            subject1.OnNext(false);
            subject2.OnNext(true);
            CollectionAssert.AreEqual(new[] { true }, results);

            subject1.OnNext(false);
            CollectionAssert.AreEqual(new[] { true }, results);

            subject1.OnNext(true);
            CollectionAssert.AreEqual(new[] { true }, results);

            subject2.OnNext(true);
            CollectionAssert.AreEqual(new[] { true }, results);

            subject2.OnNext(false);
            CollectionAssert.AreEqual(new[] { true }, results);

            subject1.OnNext(false);
            CollectionAssert.AreEqual(new[] { true, false }, results);

            subject1.OnNext(false);
            CollectionAssert.AreEqual(new[] { true, false }, results);
        }

        [TestMethod]
        public void Or_InputDistinctUntilChanged()
        {
            var subject1 = new Subject<bool>();
            var subject2 = new Subject<bool>();
            var or = subject1.Or(subject2, OperatorDistinctness.InputDistinctUntilChanged);

            var results = new List<bool>();
            or.Subscribe(results.Add);

            subject1.OnNext(false);
            subject2.OnNext(true);
            CollectionAssert.AreEqual(new[] { true }, results);

            subject1.OnNext(false);
            CollectionAssert.AreEqual(new[] { true }, results);

            subject1.OnNext(true);
            CollectionAssert.AreEqual(new[] { true, true }, results);

            subject2.OnNext(true);
            CollectionAssert.AreEqual(new[] { true, true }, results);

            subject2.OnNext(false);
            CollectionAssert.AreEqual(new[] { true, true, true }, results);

            subject1.OnNext(false);
            CollectionAssert.AreEqual(new[] { true, true, true, false }, results);

            subject1.OnNext(false);
            CollectionAssert.AreEqual(new[] { true, true, true, false }, results);
        }

        [TestMethod]
        public void Or_NotDistinct()
        {
            var subject1 = new Subject<bool>();
            var subject2 = new Subject<bool>();
            var or = subject1.Or(subject2, OperatorDistinctness.NotDistinct);

            var results = new List<bool>();
            or.Subscribe(results.Add);

            subject1.OnNext(false);
            subject2.OnNext(true);
            CollectionAssert.AreEqual(new[] { true }, results);

            subject1.OnNext(false);
            CollectionAssert.AreEqual(new[] { true, true }, results);

            subject1.OnNext(true);
            CollectionAssert.AreEqual(new[] { true, true, true }, results);

            subject2.OnNext(true);
            CollectionAssert.AreEqual(new[] { true, true, true, true }, results);

            subject2.OnNext(false);
            CollectionAssert.AreEqual(new[] { true, true, true, true, true }, results);

            subject1.OnNext(false);
            CollectionAssert.AreEqual(new[] { true, true, true, true, true, false }, results);

            subject1.OnNext(false);
            CollectionAssert.AreEqual(new[] { true, true, true, true, true, false, false }, results);
        }

        [TestMethod]
        public void Or_Multiple_OutputDistinctUntilChanged()
        {
            var subject1 = new Subject<bool>();
            var subject2 = new Subject<bool>();
            var subject3 = new Subject<bool>();
            var or = subject1.Or(subject2, subject3);

            var results = new List<bool>();
            or.Subscribe(results.Add);

            subject1.OnNext(true);
            subject2.OnNext(true);
            subject3.OnNext(false);
            CollectionAssert.AreEqual(new[] { true }, results);

            subject1.OnNext(true);
            CollectionAssert.AreEqual(new[] { true }, results);

            subject1.OnNext(false);
            CollectionAssert.AreEqual(new[] { true }, results);

            subject2.OnNext(false);
            CollectionAssert.AreEqual(new[] { true, false }, results);

            subject3.OnNext(false);
            CollectionAssert.AreEqual(new[] { true, false }, results);

            subject1.OnNext(true);
            CollectionAssert.AreEqual(new[] { true, false, true }, results);

            subject2.OnNext(true);
            CollectionAssert.AreEqual(new[] { true, false, true }, results);
        }

        [TestMethod]
        public void Or_Multiple_InputDistinctUntilChanged()
        {
            var subject1 = new Subject<bool>();
            var subject2 = new Subject<bool>();
            var subject3 = new Subject<bool>();
            var or = subject1.Or(subject2, subject3, OperatorDistinctness.InputDistinctUntilChanged);

            var results = new List<bool>();
            or.Subscribe(results.Add);

            subject1.OnNext(true);
            subject2.OnNext(true);
            subject3.OnNext(false);
            CollectionAssert.AreEqual(new[] { true }, results);

            subject1.OnNext(true);
            CollectionAssert.AreEqual(new[] { true }, results);

            subject1.OnNext(false);
            CollectionAssert.AreEqual(new[] { true, true }, results);

            subject2.OnNext(false);
            CollectionAssert.AreEqual(new[] { true, true, false }, results);

            subject3.OnNext(false);
            CollectionAssert.AreEqual(new[] { true, true, false }, results);

            subject1.OnNext(true);
            CollectionAssert.AreEqual(new[] { true, true, false, true }, results);

            subject2.OnNext(true);
            CollectionAssert.AreEqual(new[] { true, true, false, true, true }, results);
        }

        [TestMethod]
        public void Or_Multiple_NotDistinct()
        {
            var subject1 = new Subject<bool>();
            var subject2 = new Subject<bool>();
            var subject3 = new Subject<bool>();
            var or = subject1.Or(subject2, subject3, OperatorDistinctness.NotDistinct);

            var results = new List<bool>();
            or.Subscribe(results.Add);

            subject1.OnNext(true);
            subject2.OnNext(true);
            subject3.OnNext(false);
            CollectionAssert.AreEqual(new[] { true }, results);

            subject1.OnNext(true);
            CollectionAssert.AreEqual(new[] { true, true }, results);

            subject1.OnNext(false);
            CollectionAssert.AreEqual(new[] { true, true, true }, results);

            subject2.OnNext(false);
            CollectionAssert.AreEqual(new[] { true, true, true, false }, results);

            subject3.OnNext(false);
            CollectionAssert.AreEqual(new[] { true, true, true, false, false }, results);

            subject1.OnNext(true);
            CollectionAssert.AreEqual(new[] { true, true, true, false, false, true }, results);

            subject2.OnNext(true);
            CollectionAssert.AreEqual(new[] { true, true, true, false, false, true, true }, results);
        }
    }
}
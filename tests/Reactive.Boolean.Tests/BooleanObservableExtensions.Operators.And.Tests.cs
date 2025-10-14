using System.Reactive.Subjects;

namespace Reactive.Boolean.Tests
{
    [TestClass]
    public class BooleanObservableExtensionsOperatorsAndTests
    {
        [TestMethod]
        [DataRow(false, false, false)]
        [DataRow(false, true, false)]
        [DataRow(true, false, false)]
        [DataRow(true, true, true)]
        public void And(bool input1, bool input2, bool expectedOutput)
        {
            // Arrange
            var subject1 = new Subject<bool>();
            var subject2 = new Subject<bool>();
            var and = subject1.And(subject2);

            bool? result = null;
            and.Subscribe(b => result = b);

            // Act
            subject1.OnNext(input1);
            subject2.OnNext(input2);

            // Assert
            Assert.AreEqual(expectedOutput, result);
        }

        [TestMethod]
        [DataRow(false, false, false, false)]
        [DataRow(false, false, true, false)]
        [DataRow(false, true, false, false)]
        [DataRow(false, true, true, false)]
        [DataRow(true, false, false, false)]
        [DataRow(true, false, true, false)]
        [DataRow(true, true, false, false)]
        [DataRow(true, true, true, true)]
        public void And_Multiple(bool input1, bool input2, bool input3, bool expectedOutput)
        {
            // Arrange
            var subject1 = new Subject<bool>();
            var subject2 = new Subject<bool>();
            var subject3 = new Subject<bool>();
            var and = subject1.And(subject2, subject3);

            bool? result = null;
            and.Subscribe(b => result = b);

            // Act
            subject1.OnNext(input1);
            subject2.OnNext(input2);
            subject3.OnNext(input3);

            // Assert
            Assert.AreEqual(expectedOutput, result);
        }

        [TestMethod]
        public void And_OutputDistinctUntilChanged()
        {
            var subject1 = new Subject<bool>();
            var subject2 = new Subject<bool>();
            var and = subject1.And(subject2);

            var results = new List<bool>();
            and.Subscribe(results.Add);

            subject1.OnNext(false);
            subject2.OnNext(true);
            CollectionAssert.AreEqual(new[] { false }, results);

            subject1.OnNext(false);
            CollectionAssert.AreEqual(new[] { false }, results);

            subject1.OnNext(true);
            CollectionAssert.AreEqual(new[] { false, true }, results);

            subject2.OnNext(true);
            CollectionAssert.AreEqual(new[] { false, true }, results);

            subject2.OnNext(false);
            CollectionAssert.AreEqual(new[] { false, true, false }, results);

            subject1.OnNext(false);
            CollectionAssert.AreEqual(new[] { false, true, false }, results);
        }

        [TestMethod]
        public void And_InputDistinctUntilChanged()
        {
            var subject1 = new Subject<bool>();
            var subject2 = new Subject<bool>();
            var and = subject1.And(subject2, OperatorDistinctness.InputDistinctUntilChanged);

            var results = new List<bool>();
            and.Subscribe(results.Add);

            subject1.OnNext(false);
            subject2.OnNext(true);
            CollectionAssert.AreEqual(new[] { false }, results);

            subject1.OnNext(false);
            CollectionAssert.AreEqual(new[] { false }, results);

            subject1.OnNext(true);
            CollectionAssert.AreEqual(new[] { false, true }, results);

            subject2.OnNext(true);
            CollectionAssert.AreEqual(new[] { false, true }, results);

            subject2.OnNext(false);
            CollectionAssert.AreEqual(new[] { false, true, false }, results);

            subject1.OnNext(false);
            CollectionAssert.AreEqual(new[] { false, true, false, false }, results);
        }

        [TestMethod]
        public void And_NotDistinct()
        {
            var subject1 = new Subject<bool>();
            var subject2 = new Subject<bool>();
            var and = subject1.And(subject2, OperatorDistinctness.NotDistinct);

            var results = new List<bool>();
            and.Subscribe(results.Add);

            subject1.OnNext(false);
            subject2.OnNext(true);
            CollectionAssert.AreEqual(new[] { false }, results);

            subject1.OnNext(false);
            CollectionAssert.AreEqual(new[] { false, false }, results);

            subject1.OnNext(true);
            CollectionAssert.AreEqual(new[] { false, false, true }, results);

            subject2.OnNext(true);
            CollectionAssert.AreEqual(new[] { false, false, true, true }, results);

            subject2.OnNext(false);
            CollectionAssert.AreEqual(new[] { false, false, true, true, false }, results);

            subject1.OnNext(false);
            CollectionAssert.AreEqual(new[] { false, false, true, true, false, false }, results);
        }

        [TestMethod]
        public void And_Multiple_OutputDistinctUntilChanged()
        {
            var subject1 = new Subject<bool>();
            var subject2 = new Subject<bool>();
            var subject3 = new Subject<bool>();
            var and = subject1.And(subject2, subject3);

            var results = new List<bool>();
            and.Subscribe(results.Add);

            subject1.OnNext(false);
            subject2.OnNext(false);
            subject3.OnNext(true);
            CollectionAssert.AreEqual(new[] { false }, results);

            subject1.OnNext(false);
            CollectionAssert.AreEqual(new[] { false }, results);

            subject1.OnNext(true);
            CollectionAssert.AreEqual(new[] { false }, results);

            subject2.OnNext(true);
            CollectionAssert.AreEqual(new[] { false, true }, results);

            subject3.OnNext(true);
            CollectionAssert.AreEqual(new[] { false, true }, results);

            subject1.OnNext(false);
            CollectionAssert.AreEqual(new[] { false, true, false }, results);

            subject2.OnNext(false);
            CollectionAssert.AreEqual(new[] { false, true, false }, results);
        }

        [TestMethod]
        public void And_Multiple_InputDistinctUntilChanged()
        {
            var subject1 = new Subject<bool>();
            var subject2 = new Subject<bool>();
            var subject3 = new Subject<bool>();
            var and = subject1.And(subject2, subject3, OperatorDistinctness.InputDistinctUntilChanged);

            var results = new List<bool>();
            and.Subscribe(results.Add);

            subject1.OnNext(false);
            subject2.OnNext(false);
            subject3.OnNext(true);
            CollectionAssert.AreEqual(new[] { false }, results);

            subject1.OnNext(false);
            CollectionAssert.AreEqual(new[] { false }, results);

            subject1.OnNext(true);
            CollectionAssert.AreEqual(new[] { false, false }, results);

            subject2.OnNext(true);
            CollectionAssert.AreEqual(new[] { false, false, true }, results);

            subject3.OnNext(true);
            CollectionAssert.AreEqual(new[] { false, false, true }, results);

            subject1.OnNext(false);
            CollectionAssert.AreEqual(new[] { false, false, true, false }, results);

            subject2.OnNext(false);
            CollectionAssert.AreEqual(new[] { false, false, true, false, false }, results);
        }

        [TestMethod]
        public void And_Multiple_NotDistinct()
        {
            var subject1 = new Subject<bool>();
            var subject2 = new Subject<bool>();
            var subject3 = new Subject<bool>();
            var and = subject1.And(subject2, subject3, OperatorDistinctness.NotDistinct);

            var results = new List<bool>();
            and.Subscribe(results.Add);

            subject1.OnNext(false);
            subject2.OnNext(false);
            subject3.OnNext(true);
            CollectionAssert.AreEqual(new[] { false }, results);

            subject1.OnNext(false);
            CollectionAssert.AreEqual(new[] { false, false }, results);

            subject1.OnNext(true);
            CollectionAssert.AreEqual(new[] { false, false, false }, results);

            subject2.OnNext(true);
            CollectionAssert.AreEqual(new[] { false, false, false, true }, results);

            subject3.OnNext(true);
            CollectionAssert.AreEqual(new[] { false, false, false, true, true }, results);

            subject1.OnNext(false);
            CollectionAssert.AreEqual(new[] { false, false, false, true, true, false }, results);

            subject2.OnNext(false);
            CollectionAssert.AreEqual(new[] { false, false, false, true, true, false, false }, results);
        }
    }
}
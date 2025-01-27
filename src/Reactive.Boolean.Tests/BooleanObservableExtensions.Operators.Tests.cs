using System.Reactive.Subjects;

namespace Reactive.Boolean.Tests
{
    [TestClass]
    public class BooleanObservableExtensionsOperatorsTests
    {
        [DataTestMethod]
        [DataRow(false, true)]
        [DataRow(true, false)]
        public void Not(bool input, bool expectedOutput)
        {
            // Arrange
            var subject = new Subject<bool>();
            var not = subject.Not();
            
            bool? result = null;
            not.Subscribe(b => result = b);

            // Act
            subject.OnNext(input);

            // Assert
            Assert.AreEqual(expectedOutput, result);
        }

        [DataTestMethod]
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
        public void And_DistinctUntilChanged()
        {
            // Arrange
            var subject1 = new Subject<bool>();
            var subject2 = new Subject<bool>();
            var and = subject1.And(subject2, distinctUntilChanged: true);

            var results = new List<bool>();
            and.Subscribe(results.Add);

            // Initially make sure we have false as a result.
            subject1.OnNext(false);
            subject2.OnNext(true);
            CollectionAssert.AreEqual(new[] { false }, results);

            // Act
            subject2.OnNext(false);

            // Assert
            CollectionAssert.AreEqual(new[] { false }, results);
        }

        [TestMethod]
        public void And_NotDistinctUntilChanged()
        {
            // Arrange
            var subject1 = new Subject<bool>();
            var subject2 = new Subject<bool>();
            var and = subject1.And(subject2, distinctUntilChanged: false);

            var results = new List<bool>();
            and.Subscribe(results.Add);

            // Initially make sure we have false as a result.
            subject1.OnNext(false);
            subject2.OnNext(true);
            CollectionAssert.AreEqual(new[] { false }, results);

            // Act
            subject2.OnNext(false);

            // Assert
            CollectionAssert.AreEqual(new[] { false, false }, results);
        }

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
            var and = subject1.Or(subject2);

            bool? result = null;
            and.Subscribe(b => result = b);

            // Act
            subject1.OnNext(input1);
            subject2.OnNext(input2);

            // Assert
            Assert.AreEqual(expectedOutput, result);
        }

        [TestMethod]
        public void Or_DistinctUntilChanged()
        {
            // Arrange
            var subject1 = new Subject<bool>();
            var subject2 = new Subject<bool>();
            var and = subject1.Or(subject2, distinctUntilChanged: true);

            var results = new List<bool>();
            and.Subscribe(results.Add);

            // Initially make sure we have true as a result.
            subject1.OnNext(false);
            subject2.OnNext(true);
            CollectionAssert.AreEqual(new[] { true }, results);

            // Act
            subject1.OnNext(true);

            // Assert
            CollectionAssert.AreEqual(new[] { true }, results);
        }

        [TestMethod]
        public void Or_NotDistinctUntilChanged()
        {
            // Arrange
            var subject1 = new Subject<bool>();
            var subject2 = new Subject<bool>();
            var and = subject1.Or(subject2, distinctUntilChanged: false);

            var results = new List<bool>();
            and.Subscribe(results.Add);

            // Initially make sure we have true as a result.
            subject1.OnNext(false);
            subject2.OnNext(true);
            CollectionAssert.AreEqual(new[] { true }, results);

            // Act
            subject1.OnNext(true);

            // Assert
            CollectionAssert.AreEqual(new[] { true, true }, results);
        }

        [DataTestMethod]
        [DataRow(false, false, false)]
        [DataRow(false, true, true)]
        [DataRow(true, false, true)]
        [DataRow(true, true, false)]
        public void XOr(bool input1, bool input2, bool expectedOutput)
        {
            // Arrange
            var subject1 = new Subject<bool>();
            var subject2 = new Subject<bool>();
            var xor = subject1.XOr(subject2);

            bool? result = null;
            xor.Subscribe(b => result = b);

            // Act
            subject1.OnNext(input1);
            subject2.OnNext(input2);

            // Assert
            Assert.AreEqual(expectedOutput, result);
        }
    }
}
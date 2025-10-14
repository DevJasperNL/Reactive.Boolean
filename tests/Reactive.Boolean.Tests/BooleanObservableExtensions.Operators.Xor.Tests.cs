using System.Reactive.Subjects;

namespace Reactive.Boolean.Tests
{
    [TestClass]
    public class BooleanObservableExtensionsOperatorsXorTests
    {
        [TestMethod]
        [DataRow(false, false, false)]
        [DataRow(false, true, true)]
        [DataRow(true, false, true)]
        [DataRow(true, true, false)]
        public void Xor(bool input1, bool input2, bool expectedOutput)
        {
            // Arrange
            var subject1 = new Subject<bool>();
            var subject2 = new Subject<bool>();
            var xor = subject1.Xor(subject2);

            bool? result = null;
            xor.Subscribe(b => result = b);

            // Act
            subject1.OnNext(input1);
            subject2.OnNext(input2);

            // Assert
            Assert.AreEqual(expectedOutput, result);
        }

        [TestMethod]
        public void Xor_DistinctUntilChanged()
        {
            // Arrange
            var subject1 = new Subject<bool>();
            var subject2 = new Subject<bool>();
            var xor = subject1.Xor(subject2);

            var results = new List<bool>();
            xor.Subscribe(results.Add);

            subject1.OnNext(false);
            subject2.OnNext(false);
            CollectionAssert.AreEqual(new[] { false }, results);

            subject1.OnNext(false);
            CollectionAssert.AreEqual(new[] { false }, results);

            subject1.OnNext(true);
            CollectionAssert.AreEqual(new[] { false, true }, results);

            subject2.OnNext(true);
            CollectionAssert.AreEqual(new[] { false, true, false }, results);

            subject1.OnNext(true);
            CollectionAssert.AreEqual(new[] { false, true, false }, results);

            subject1.OnNext(false);
            CollectionAssert.AreEqual(new[] { false, true, false, true }, results);

            subject2.OnNext(true);
            CollectionAssert.AreEqual(new[] { false, true, false, true }, results);
        }

        [TestMethod]
        public void Xor_NotDistinctUntilChanged()
        {
            // Arrange
            var subject1 = new Subject<bool>();
            var subject2 = new Subject<bool>();
            var xor = subject1.Xor(subject2, distinctUntilChanged: false);

            var results = new List<bool>();
            xor.Subscribe(results.Add);

            subject1.OnNext(false);
            subject2.OnNext(false);
            CollectionAssert.AreEqual(new[] { false }, results);

            subject1.OnNext(false);
            CollectionAssert.AreEqual(new[] { false, false }, results);

            subject1.OnNext(true);
            CollectionAssert.AreEqual(new[] { false, false, true }, results);

            subject2.OnNext(true);
            CollectionAssert.AreEqual(new[] { false, false, true, false }, results);

            subject1.OnNext(true);
            CollectionAssert.AreEqual(new[] { false, false, true, false, false }, results);

            subject1.OnNext(false);
            CollectionAssert.AreEqual(new[] { false, false, true, false, false, true }, results);

            subject2.OnNext(true);
            CollectionAssert.AreEqual(new[] { false, false, true, false, false, true, true }, results);
        }
    }
}
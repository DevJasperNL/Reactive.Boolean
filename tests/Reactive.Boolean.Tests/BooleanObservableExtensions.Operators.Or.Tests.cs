using System.Reactive.Subjects;

namespace Reactive.Boolean.Tests
{
    [TestClass]
    public class BooleanObservableExtensionsOperatorsOrTests
    {
        [TestMethod]
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

        [TestMethod]
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
        [TestMethod]
        [DataRow(false, false, true)]
        [DataRow(false, true, false)]
        [DataRow(true, false, false)]
        [DataRow(true, true, false)]
        public void Nor(bool input1, bool input2, bool expectedOutput)
        {
            var subject1 = new Subject<bool>();
            var subject2 = new Subject<bool>();
            var nor = subject1.Nor(subject2);

            bool? result = null;
            nor.Subscribe(b => result = b);

            subject1.OnNext(input1);
            subject2.OnNext(input2);

            Assert.AreEqual(expectedOutput, result);
        }

        [TestMethod]
        public void Nor_EveryOverload_IsInverseOfOr()
        {
            for (var bits = 0; bits < 16; bits++)
            {
                var inputs = Enumerable.Range(0, 4).Select(i => (bits & (1 << i)) != 0).ToArray();
                var subjects = inputs.Select(_ => new Subject<bool>()).ToArray();
                var expectedThree = !(inputs[0] || inputs[1] || inputs[2]);
                var expectedFour = !inputs.Any(v => v);

                var overloads = new (string Name, IObservable<bool> Nor, bool Expected)[]
                {
                    ("3-arity", subjects[0].Nor(subjects[1], subjects[2], OperatorDistinctness.NotDistinct), expectedThree),
                    ("4-arity", subjects[0].Nor(subjects[1], subjects[2], subjects[3], OperatorDistinctness.NotDistinct), expectedFour),
                    ("params", subjects[0].Nor(subjects[1], subjects[2], subjects[3]), expectedFour),
                    ("enumerable", subjects[0].Nor(subjects.Skip(1), OperatorDistinctness.NotDistinct), expectedFour),
                    ("collection", subjects.Nor(), expectedFour),
                };
                var results = new bool?[overloads.Length];
                for (var i = 0; i < overloads.Length; i++)
                {
                    var index = i;
                    overloads[i].Nor.Subscribe(b => results[index] = b);
                }

                for (var i = 0; i < subjects.Length; i++)
                {
                    subjects[i].OnNext(inputs[i]);
                }

                for (var i = 0; i < overloads.Length; i++)
                {
                    Assert.AreEqual(overloads[i].Expected, results[i], $"{overloads[i].Name} with inputs {string.Join(",", inputs)}");
                }
            }
        }

        [TestMethod]
        public void Or_EmptyCollection_EmitsFalseAndCompletes()
        {
            var results = new List<bool>();
            var completed = false;

            Array.Empty<IObservable<bool>>().Or().Subscribe(results.Add, () => completed = true);

            CollectionAssert.AreEqual(new[] { false }, results);
            Assert.IsTrue(completed);
        }

        [TestMethod]
        public void Or_NullObservable_Throws()
        {
            var subject = new Subject<bool>();

            Assert.ThrowsExactly<ArgumentNullException>(() => ((IObservable<bool>)null!).Or(subject, subject));
            Assert.ThrowsExactly<ArgumentNullException>(() => subject.Or((IEnumerable<IObservable<bool>>)null!));
            Assert.ThrowsExactly<ArgumentException>(() => subject.Or(new IObservable<bool>[] { null! }));
            Assert.ThrowsExactly<ArgumentException>(() => subject.Or(subject, null!, subject, OperatorDistinctness.NotDistinct));
        }

    }
}
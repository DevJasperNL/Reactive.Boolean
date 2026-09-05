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
        [TestMethod]
        [DataRow(false, false, true)]
        [DataRow(false, true, true)]
        [DataRow(true, false, true)]
        [DataRow(true, true, false)]
        public void Nand(bool input1, bool input2, bool expectedOutput)
        {
            var subject1 = new Subject<bool>();
            var subject2 = new Subject<bool>();
            var nand = subject1.Nand(subject2);

            bool? result = null;
            nand.Subscribe(b => result = b);

            subject1.OnNext(input1);
            subject2.OnNext(input2);

            Assert.AreEqual(expectedOutput, result);
        }

        [TestMethod]
        public void Nand_EveryOverload_IsInverseOfAnd()
        {
            for (var bits = 0; bits < 16; bits++)
            {
                var inputs = Enumerable.Range(0, 4).Select(i => (bits & (1 << i)) != 0).ToArray();
                var subjects = inputs.Select(_ => new Subject<bool>()).ToArray();
                var expectedThree = !(inputs[0] && inputs[1] && inputs[2]);
                var expectedFour = !inputs.All(v => v);

                var overloads = new (string Name, IObservable<bool> Nand, bool Expected)[]
                {
                    ("3-arity", subjects[0].Nand(subjects[1], subjects[2], OperatorDistinctness.NotDistinct), expectedThree),
                    ("4-arity", subjects[0].Nand(subjects[1], subjects[2], subjects[3], OperatorDistinctness.NotDistinct), expectedFour),
                    ("params", subjects[0].Nand(subjects[1], subjects[2], subjects[3]), expectedFour),
                    ("enumerable", subjects[0].Nand(subjects.Skip(1), OperatorDistinctness.NotDistinct), expectedFour),
                    ("collection", subjects.Nand(), expectedFour),
                };
                var results = new bool?[overloads.Length];
                for (var i = 0; i < overloads.Length; i++)
                {
                    var index = i;
                    overloads[i].Nand.Subscribe(b => results[index] = b);
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
        public void And_EmptyCollection_EmitsTrueAndCompletes()
        {
            var results = new List<bool>();
            var completed = false;

            Array.Empty<IObservable<bool>>().And().Subscribe(results.Add, () => completed = true);

            CollectionAssert.AreEqual(new[] { true }, results);
            Assert.IsTrue(completed);
        }

        [TestMethod]
        public void And_NullObservable_Throws()
        {
            var subject = new Subject<bool>();

            Assert.ThrowsExactly<ArgumentNullException>(() => ((IObservable<bool>)null!).And(subject, subject));
            Assert.ThrowsExactly<ArgumentNullException>(() => subject.And((IEnumerable<IObservable<bool>>)null!));
            Assert.ThrowsExactly<ArgumentException>(() => subject.And(new IObservable<bool>[] { null! }));
            Assert.ThrowsExactly<ArgumentException>(() => subject.And(subject, null!, subject, OperatorDistinctness.NotDistinct));
        }

    }
}
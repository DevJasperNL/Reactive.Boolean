using System.Reactive.Subjects;

namespace Reactive.Boolean.Tests
{
    [TestClass]
    public class BooleanObservableExtensionsOperatorsNotTests
    {
        [TestMethod]
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
    }
}
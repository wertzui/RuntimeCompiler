using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RuntimeCompiler.Tests
{
    [TestClass]
    public class CompilerDelegateTests
    {
        [TestMethod]
        public void Custom_delegate_can_be_executed()
        {
            // Arrange
            var expectedResult = "someValue";

            // Act
            var func = Compiler.CompileDelegate<MyDelegate>("return value;", new[] { "value" });
            var actualResult = func(expectedResult);

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        delegate string MyDelegate(string value);
    }
}
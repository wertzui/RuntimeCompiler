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

        [TestMethod]
        public void Custom_delegate_supports_in_modifier()
        {
            // Arrange
            var expectedValue = 13;

            // Act
            var func = Compiler.CompileDelegate<MyInDelegate>("value", new[] { "value" });
            var result = func(in expectedValue);

            // Assert
            Assert.AreEqual(expectedValue, result);
        }

        [TestMethod]
        public void Custom_delegate_supports_out_modifier()
        {
            // Arrange
            var expectedValue = 13;

            // Act
            var func = Compiler.CompileDelegate<MyOutDelegate>("value = 13;", new[] { "value" });
            func(out var result);

            // Assert
            Assert.AreEqual(expectedValue, result);
        }

        [TestMethod]
        public void Custom_delegate_supports_ref_modifier()
        {
            // Arrange
            var expectedValue = 13;
            var result = 12;

            // Act
            var func = Compiler.CompileDelegate<MyRefDelegate>("value++;", new[] { "value" });
            func(ref result);

            // Assert
            Assert.AreEqual(expectedValue, result);
        }


        delegate string MyDelegate(string value);

        delegate int MyInDelegate(in int value);

        delegate void MyOutDelegate(out int value);

        delegate void MyRefDelegate(ref int value);
    }
}
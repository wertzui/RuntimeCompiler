using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RuntimeCompiler.Tests
{
    [TestClass]
    public class CompilerFunctionTests
    {
        [TestMethod]
        public void Function_with_0_parameters_returns_constant_value()
        {
            // Arrange
            var expectedResult = 1;

            // Act
            var func = Compiler.CompileFunction<int>(expectedResult.ToString());
            var actualResult = func();

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void Function_with_1_parameters_returns_given_value()
        {
            // Arrange
            var expectedResult = 1;

            // Act
            var func = Compiler.CompileFunction<int, int>("it");
            var actualResult = func(expectedResult);

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void Function_with_2_parameters_returns_sum()
        {
            // Arrange
            var expectedResult = 3;

            // Act
            var func = Compiler.CompileFunction<int, int, int>("a+b", new[] { "a", "b" });
            var actualResult = func(1, 2);

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void Function_with_generic_return_type_can_be_executed()
        {
            // Act
            var func = Compiler.CompileFunction<List<string>>("new List<string>()");
            var actualResult = func();

            // Assert
            Assert.IsNotNull(actualResult);
        }

        [DataTestMethod]
        [DataRow("it", 1)]
        [DataRow("it", "foo")]
        [DataRow("return it;", 1)]
        [DataRow("return it;", "foo")]
        public void Idempotent_function_returns_the_same_value_as_given(string methodBody, object expectedResult)
        {
            // Arrange

            // Act
            var func = Compiler.CompileFunction<object, object>(methodBody);
            var actualResult = func(expectedResult);

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}
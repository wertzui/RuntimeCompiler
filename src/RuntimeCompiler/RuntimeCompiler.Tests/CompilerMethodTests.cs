using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RuntimeCompiler.Tests;

[TestClass]
public class CompilerMethodTests
{
    [DataTestMethod]
    [DataRow("int", "it", 1)]
    [DataRow("string", "it", "foo")]
    [DataRow("int", "return it;", 1)]
    [DataRow("string", "return it;", "foo")]
    public void Idempotent_method_returns_the_same_value_as_given(string returnType, string methodBody, object expectedResult)
    {
        // Arrange
        var arguments = $"{returnType} {SourceDefaults.ARGUMENT_NAME}";

        // Act
        var method = Compiler.CompileMethod(methodBody, arguments, returnType);
        var actualResult = method.Invoke(null, new[] { expectedResult });

        // Assert
        Assert.AreEqual(expectedResult, actualResult);
    }

    [TestMethod]
    public void Method_is_compiled()
    {
        // Arrange

        // Act
        var method = Compiler.CompileMethod("var x = 1+2;");

        // Assert
        Assert.IsNotNull(method);
    }

    [DataTestMethod]
    [DataRow("int", "1", 1)]
    [DataRow("string", "\"foo\"", "foo")]
    public void Method_without_parameter_returns_the_expected_value(string returnType, string methodBody, object expectedResult)
    {
        // Arrange

        // Act
        var method = Compiler.CompileMethod(methodBody, returnType: returnType);
        var actualResult = method.Invoke(null, null);

        // Assert
        Assert.AreEqual(expectedResult, actualResult);
    }
}
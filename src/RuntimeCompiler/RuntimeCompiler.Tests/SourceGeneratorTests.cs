using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RuntimeCompiler.Tests;

[TestClass]
public class SourceGeneratorTests
{
    [DataTestMethod]
    [DataRow("methodBodyWithoutSemicolon")]
    [DataRow(";methodBodyWithSemicolonAtTheBeginning")]
    [DataRow("methodBodyWithSemicolon;InTheMiddle")]
    public void Arguments_are_correctly_inserted_into_the_source_code_without_semicolon(string methodBody)
    {
        // Arrange
        var arguments = "argumentType argumentName";
        var returnType = "somethingNotVoid";
        var expectedSource = @$"
using System;


namespace DynamicCompilation
{{
    public static class Executer
    {{
        public static {returnType} Execute({arguments})
        {{
            return {methodBody};
        }}
    }}
}}";

        // Act
        var actualSource = SourceGenerator.GenerateSource(methodBody, arguments, returnType);

        // Assert
        Assert.AreEqual(expectedSource, actualSource);
    }

    [TestMethod]
    public void Arguments_are_correctly_inserted_into_the_source_code_with_semicolon()
    {
        // Arrange
        const string expectedSource = @"
using System;


namespace DynamicCompilation
{
    public static class Executer
    {
        public static void Execute(argumentType argumentName)
        {
            methodBodyWithSemicolon;
        }
    }
}";


        // Act
        var actualSource = SourceGenerator.GenerateSource("methodBodyWithSemicolon;", "argumentType argumentName");

        // Assert
        Assert.AreEqual(expectedSource, actualSource);
    }
}

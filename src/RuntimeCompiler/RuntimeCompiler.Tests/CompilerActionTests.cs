using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RuntimeCompiler.Tests
{
    [TestClass]
    public class CompilerActionTests
    {
        [TestMethod]
        public void Empty_action_with_0_parameters_can_be_executed()
        {
            // Arrange

            // Act
            var action = Compiler.CompileAction(";");

            // Assert
            action();
        }

        [TestMethod]
        public void Empty_action_with_1_parameters_can_be_executed()
        {
            // Arrange

            // Act
            var action = Compiler.CompileAction<int>(";");

            // Assert
            action(1);
        }

        [TestMethod]
        public void Empty_action_with_2_parameters_can_be_executed()
        {
            // Arrange

            // Act
            var action = Compiler.CompileAction<int, string>(";", new[] { "argument1", "argument2" });

            // Assert
            action(1, "foo");
        }

        [TestMethod]
        public void Action_with_custom_type_can_be_executed()
        {
            // Arrange
            var methodBody = @"
MyClass x = new MyClass();
x.Foo = 3;
Console.WriteLine(x.Foo);
";

            // Act
            var action = Compiler.CompileAction(methodBody, "using RuntimeCompiler.Tests;");

            // Assert
            action();
        }
    }

    public class MyClass
    {
        public int Foo { get; set; }
    }
}
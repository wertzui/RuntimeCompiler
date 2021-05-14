using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

namespace RuntimeCompiler.Tests
{
    [TestClass]
    public class ReflectionHelperTests
    {
        [TestMethod]
        public void ReflectionHelper_Finds_Object_Equals()
        {
            // Arrange
            var assembly = typeof(object).Assembly;
            var expectedMethod = typeof(object).GetMethod(nameof(object.Equals), BindingFlags.Public | BindingFlags.Static);

            // Act
            var actualMethod = ReflectionHelper.GetMethodFromAssembly(assembly, nameof(object.Equals), nameof(Object), nameof(System));

            // Assert
            Assert.AreEqual(expectedMethod, actualMethod);
        }
    }
}
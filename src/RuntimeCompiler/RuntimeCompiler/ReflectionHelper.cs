using System;
using System.Reflection;

namespace RuntimeCompiler;

/// <summary>
/// Contains Helper Methods for Reflection.
/// </summary>
public static class ReflectionHelper
{
    /// <summary>
    /// Gets the method with the <paramref name="methodName"/> in the class <paramref name="className"/> in the namespace <paramref name="namespaceName"/> from the given <paramref name="compiledAssembly"/>.
    /// </summary>
    /// <param name="compiledAssembly">The compiled assembly.</param>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="className">Name of the class.</param>
    /// <param name="namespaceName">Name of the namespace.</param>
    /// <returns></returns>
    /// <exception cref="System.NullReferenceException">
    /// Cannot find the class {className} in the given assembly.
    /// or
    /// Cannot find the method {methodName} of the class {className} in the given assembly, although the class is present.
    /// </exception>
    public static MethodInfo GetMethodFromAssembly(
        Assembly compiledAssembly,
        string methodName = SourceDefaults.METHOD,
        string className = SourceDefaults.CLASS,
        string namespaceName = SourceDefaults.NAMESPACE)
    {
        var fullTypeName = string.Concat(namespaceName, ".", className);
        var compiledClass = compiledAssembly.GetType(fullTypeName);
        if (compiledClass is null)
            throw new NullReferenceException($"Cannot find the class {className} in the given assembly.");

        var compiledMethod = compiledClass.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);
        if(compiledMethod is null)
            throw new NullReferenceException($"Cannot find the method {methodName} of the class {className} in the given assembly, although the class is present.");

        return compiledMethod;
    }
}
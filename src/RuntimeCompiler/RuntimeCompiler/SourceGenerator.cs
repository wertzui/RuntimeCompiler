using System;

namespace RuntimeCompiler;

/// <summary>
/// This class generates the source code which is later compiled.
/// </summary>
public static class SourceGenerator
{
    /// <summary>
    /// Generates the source code from the given parameters.
    /// This is the code that is later compiled in the <see cref="Compiler"/>.
    /// </summary>
    /// <param name="methodBody">The method body.</param>
    /// <param name="arguments">The arguments.</param>
    /// <param name="returnType">Type of the return.</param>
    /// <param name="additionalUsings">The additional usings.</param>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="className">Name of the class.</param>
    /// <param name="namespaceName">Name of the namespace.</param>
    /// <param name="sourceSkeleton">The source skeleton.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">methodBody</exception>
    public static string GenerateSource(
        string methodBody,
        string arguments = SourceDefaults.NO_ARGUMENTS,
        string returnType = SourceDefaults.RETURN_TYPE,
        string additionalUsings = SourceDefaults.USINGS,
        string methodName = SourceDefaults.METHOD,
        string className = SourceDefaults.CLASS,
        string namespaceName = SourceDefaults.NAMESPACE,
        string sourceSkeleton = SourceDefaults.SOURCE_SKELETON)
    {
        if (methodBody is null)
            throw new ArgumentNullException(nameof(methodBody));

        methodBody = AddReturnIfNeeded(methodBody, returnType);

        var source = string.Format(
            sourceSkeleton,
            additionalUsings,
            namespaceName,
            className,
            returnType,
            methodName,
            arguments,
            methodBody);

        return source;
    }

    private static string AddReturnIfNeeded(string methodBody, string returnType)
        => returnType != SourceDefaults.RETURN_TYPE && IsSingleExpression(methodBody) ? string.Concat("return ", methodBody, ";") : methodBody;

    private static bool IsSingleExpression(string methodBody) => !methodBody.EndsWith(';');
}
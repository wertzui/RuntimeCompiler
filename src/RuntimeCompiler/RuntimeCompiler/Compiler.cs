using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RuntimeCompiler;

/// <summary>
/// The Compiler is the main entry point and offers methods to compile C# code at runtime.
/// </summary>
public class Compiler
{
    private static readonly IReadOnlyCollection<PortableExecutableReference> _defaultReferences;

    static Compiler()
    {
        object? trustedPlatFormAssemblies = AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES");
        if (trustedPlatFormAssemblies is not string trustedPlatFormAssembliesString)
            throw new PlatformNotSupportedException("AppContext.GetData(\"TRUSTED_PLATFORM_ASSEMBLIES\") did not return the list of core assembly paths which indicates an unsupported platform. This library is intended to be used on net8.0.");

        _defaultReferences = trustedPlatFormAssembliesString
            .Split(Path.PathSeparator)
            .Select(r => MetadataReference.CreateFromFile(r))
            .ToArray();
    }

    /// <summary>
    /// Compiles the <paramref name="methodBody" /> to an <see cref="Action" />.
    /// </summary>
    /// <param name="methodBody">The method body.</param>
    /// <param name="additionalUsings">The additional usings.</param>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="className">Name of the class.</param>
    /// <param name="namespaceName">Name of the namespace.</param>
    /// <param name="sourceSkeleton">The source skeleton.</param>
    /// <param name="additionalAssembliesToReference">The additional assemblies to reference.</param>
    /// <returns></returns>
    public static Action CompileAction(
        string methodBody,
        string additionalUsings = SourceDefaults.USINGS,
        string methodName = SourceDefaults.METHOD,
        string className = SourceDefaults.CLASS,
        string namespaceName = SourceDefaults.NAMESPACE,
        string sourceSkeleton = SourceDefaults.SOURCE_SKELETON,
        params Assembly[] additionalAssembliesToReference)
        => CompileDelegate<Action>(methodBody, Array.Empty<string>(), additionalUsings, methodName, className, namespaceName, sourceSkeleton, additionalAssembliesToReference);

    /// <summary>
    /// Compiles the <paramref name="methodBody" /> to an <see cref="Action{T}" />.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter.</typeparam>
    /// <param name="methodBody">The method body.</param>
    /// <param name="argumentName">Name of the argument.</param>
    /// <param name="additionalUsings">The additional usings.</param>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="className">Name of the class.</param>
    /// <param name="namespaceName">Name of the namespace.</param>
    /// <param name="sourceSkeleton">The source skeleton.</param>
    /// <param name="additionalAssembliesToReference">The additional assemblies to reference.</param>
    /// <returns></returns>
    public static Action<TParam> CompileAction<TParam>(
        string methodBody,
        string argumentName = SourceDefaults.ARGUMENT_NAME,
        string additionalUsings = SourceDefaults.USINGS,
        string methodName = SourceDefaults.METHOD,
        string className = SourceDefaults.CLASS,
        string namespaceName = SourceDefaults.NAMESPACE,
        string sourceSkeleton = SourceDefaults.SOURCE_SKELETON,
        params Assembly[] additionalAssembliesToReference)
        => CompileDelegate<Action<TParam>>(methodBody, new[] { argumentName }, additionalUsings, methodName, className, namespaceName, sourceSkeleton, additionalAssembliesToReference);

    /// <summary>
    /// Compiles the <paramref name="methodBody" /> to an <see cref="Action{T1, T2}" />.
    /// </summary>
    /// <typeparam name="TParam1">The type of the param1.</typeparam>
    /// <typeparam name="TParam2">The type of the param2.</typeparam>
    /// <param name="methodBody">The method body.</param>
    /// <param name="argumentNames">The argument names.</param>
    /// <param name="additionalUsings">The additional usings.</param>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="className">Name of the class.</param>
    /// <param name="namespaceName">Name of the namespace.</param>
    /// <param name="sourceSkeleton">The source skeleton.</param>
    /// <param name="additionalAssembliesToReference">The additional assemblies to reference.</param>
    /// <returns></returns>
    public static Action<TParam1, TParam2> CompileAction<TParam1, TParam2>(
        string methodBody,
        IReadOnlyCollection<string> argumentNames,
        string additionalUsings = SourceDefaults.USINGS,
        string methodName = SourceDefaults.METHOD,
        string className = SourceDefaults.CLASS,
        string namespaceName = SourceDefaults.NAMESPACE,
        string sourceSkeleton = SourceDefaults.SOURCE_SKELETON,
        params Assembly[] additionalAssembliesToReference)
        => CompileDelegate<Action<TParam1, TParam2>>(methodBody, argumentNames, additionalUsings, methodName, className, namespaceName, sourceSkeleton, additionalAssembliesToReference);

    /// <summary>
    /// Compiles the <paramref name="methodBody" /> to an <see cref="Action{T1, T2, T3}" />.
    /// </summary>
    /// <typeparam name="TParam1">The type of the param1.</typeparam>
    /// <typeparam name="TParam2">The type of the param2.</typeparam>
    /// <typeparam name="TParam3">The type of the param3.</typeparam>
    /// <param name="methodBody">The method body.</param>
    /// <param name="argumentNames">The argument names.</param>
    /// <param name="additionalUsings">The additional usings.</param>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="className">Name of the class.</param>
    /// <param name="namespaceName">Name of the namespace.</param>
    /// <param name="sourceSkeleton">The source skeleton.</param>
    /// <param name="additionalAssembliesToReference">The additional assemblies to reference.</param>
    /// <returns></returns>
    public static Action<TParam1, TParam2, TParam3> CompileAction<TParam1, TParam2, TParam3>(
        string methodBody,
        IReadOnlyCollection<string> argumentNames,
        string additionalUsings = SourceDefaults.USINGS,
        string methodName = SourceDefaults.METHOD,
        string className = SourceDefaults.CLASS,
        string namespaceName = SourceDefaults.NAMESPACE,
        string sourceSkeleton = SourceDefaults.SOURCE_SKELETON,
        params Assembly[] additionalAssembliesToReference)
        => CompileDelegate<Action<TParam1, TParam2, TParam3>>(methodBody, argumentNames, additionalUsings, methodName, className, namespaceName, sourceSkeleton, additionalAssembliesToReference);

    /// <summary>
    /// Compiles the <paramref name="methodBody" /> to an <see cref="Action{T1, T2, T3, T4}" />.
    /// </summary>
    /// <typeparam name="TParam1">The type of the param1.</typeparam>
    /// <typeparam name="TParam2">The type of the param2.</typeparam>
    /// <typeparam name="TParam3">The type of the param3.</typeparam>
    /// <typeparam name="TParam4">The type of the param4.</typeparam>
    /// <param name="methodBody">The method body.</param>
    /// <param name="argumentNames">The argument names.</param>
    /// <param name="additionalUsings">The additional usings.</param>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="className">Name of the class.</param>
    /// <param name="namespaceName">Name of the namespace.</param>
    /// <param name="sourceSkeleton">The source skeleton.</param>
    /// <param name="additionalAssembliesToReference">The additional assemblies to reference.</param>
    /// <returns></returns>
    public static Action<TParam1, TParam2, TParam3, TParam4> CompileAction<TParam1, TParam2, TParam3, TParam4>(
        string methodBody,
        IReadOnlyCollection<string> argumentNames,
        string additionalUsings = SourceDefaults.USINGS,
        string methodName = SourceDefaults.METHOD,
        string className = SourceDefaults.CLASS,
        string namespaceName = SourceDefaults.NAMESPACE,
        string sourceSkeleton = SourceDefaults.SOURCE_SKELETON,
        params Assembly[] additionalAssembliesToReference)
        => CompileDelegate<Action<TParam1, TParam2, TParam3, TParam4>>(methodBody, argumentNames, additionalUsings, methodName, className, namespaceName, sourceSkeleton, additionalAssembliesToReference);

    /// <summary>
    /// Compiles the <paramref name="methodBody" /> to an <see cref="Action{T1, T2, T3, T4, T5}" />.
    /// </summary>
    /// <typeparam name="TParam1">The type of the param1.</typeparam>
    /// <typeparam name="TParam2">The type of the param2.</typeparam>
    /// <typeparam name="TParam3">The type of the param3.</typeparam>
    /// <typeparam name="TParam4">The type of the param4.</typeparam>
    /// <typeparam name="TParam5">The type of the param5.</typeparam>
    /// <param name="methodBody">The method body.</param>
    /// <param name="argumentNames">The argument names.</param>
    /// <param name="additionalUsings">The additional usings.</param>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="className">Name of the class.</param>
    /// <param name="namespaceName">Name of the namespace.</param>
    /// <param name="sourceSkeleton">The source skeleton.</param>
    /// <param name="additionalAssembliesToReference">The additional assemblies to reference.</param>
    /// <returns></returns>
    public static Action<TParam1, TParam2, TParam3, TParam4, TParam5> CompileAction<TParam1, TParam2, TParam3, TParam4, TParam5>(
        string methodBody,
        IReadOnlyCollection<string> argumentNames,
        string additionalUsings = SourceDefaults.USINGS,
        string methodName = SourceDefaults.METHOD,
        string className = SourceDefaults.CLASS,
        string namespaceName = SourceDefaults.NAMESPACE,
        string sourceSkeleton = SourceDefaults.SOURCE_SKELETON,
        params Assembly[] additionalAssembliesToReference)
        => CompileDelegate<Action<TParam1, TParam2, TParam3, TParam4, TParam5>>(methodBody, argumentNames, additionalUsings, methodName, className, namespaceName, sourceSkeleton, additionalAssembliesToReference);

    /// <summary>
    /// Compiles the <paramref name="methodBody" /> to an <see cref="Action{T1, T2, T3, T4, T5, T6}" />.
    /// </summary>
    /// <typeparam name="TParam1">The type of the param1.</typeparam>
    /// <typeparam name="TParam2">The type of the param2.</typeparam>
    /// <typeparam name="TParam3">The type of the param3.</typeparam>
    /// <typeparam name="TParam4">The type of the param4.</typeparam>
    /// <typeparam name="TParam5">The type of the param5.</typeparam>
    /// <typeparam name="TParam6">The type of the param6.</typeparam>
    /// <param name="methodBody">The method body.</param>
    /// <param name="argumentNames">The argument names.</param>
    /// <param name="additionalUsings">The additional usings.</param>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="className">Name of the class.</param>
    /// <param name="namespaceName">Name of the namespace.</param>
    /// <param name="sourceSkeleton">The source skeleton.</param>
    /// <param name="additionalAssembliesToReference">The additional assemblies to reference.</param>
    /// <returns></returns>
    public static Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6> CompileAction<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>(
        string methodBody,
        IReadOnlyCollection<string> argumentNames,
        string additionalUsings = SourceDefaults.USINGS,
        string methodName = SourceDefaults.METHOD,
        string className = SourceDefaults.CLASS,
        string namespaceName = SourceDefaults.NAMESPACE,
        string sourceSkeleton = SourceDefaults.SOURCE_SKELETON,
        params Assembly[] additionalAssembliesToReference)
        => CompileDelegate<Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>>(methodBody, argumentNames, additionalUsings, methodName, className, namespaceName, sourceSkeleton, additionalAssembliesToReference);

    /// <summary>
    /// Compiles the <paramref name="methodBody" /> to an <see cref="Action{T1, T2, T3, T4, T5, T6, T7}" />.
    /// </summary>
    /// <typeparam name="TParam1">The type of the param1.</typeparam>
    /// <typeparam name="TParam2">The type of the param2.</typeparam>
    /// <typeparam name="TParam3">The type of the param3.</typeparam>
    /// <typeparam name="TParam4">The type of the param4.</typeparam>
    /// <typeparam name="TParam5">The type of the param5.</typeparam>
    /// <typeparam name="TParam6">The type of the param6.</typeparam>
    /// <typeparam name="TParam7">The type of the param7.</typeparam>
    /// <param name="methodBody">The method body.</param>
    /// <param name="argumentNames">The argument names.</param>
    /// <param name="additionalUsings">The additional usings.</param>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="className">Name of the class.</param>
    /// <param name="namespaceName">Name of the namespace.</param>
    /// <param name="sourceSkeleton">The source skeleton.</param>
    /// <param name="additionalAssembliesToReference">The additional assemblies to reference.</param>
    /// <returns></returns>
    public static Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7> CompileAction<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7>(
        string methodBody,
        IReadOnlyCollection<string> argumentNames,
        string additionalUsings = SourceDefaults.USINGS,
        string methodName = SourceDefaults.METHOD,
        string className = SourceDefaults.CLASS,
        string namespaceName = SourceDefaults.NAMESPACE,
        string sourceSkeleton = SourceDefaults.SOURCE_SKELETON,
        params Assembly[] additionalAssembliesToReference)
        => CompileDelegate<Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7>>(methodBody, argumentNames, additionalUsings, methodName, className, namespaceName, sourceSkeleton, additionalAssembliesToReference);

    /// <summary>
    /// Compiles the <paramref name="methodBody" /> to an <see cref="Action{T1, T2, T3, T4, T5, T6, T7, T8}" />.
    /// </summary>
    /// <typeparam name="TParam1">The type of the param1.</typeparam>
    /// <typeparam name="TParam2">The type of the param2.</typeparam>
    /// <typeparam name="TParam3">The type of the param3.</typeparam>
    /// <typeparam name="TParam4">The type of the param4.</typeparam>
    /// <typeparam name="TParam5">The type of the param5.</typeparam>
    /// <typeparam name="TParam6">The type of the param6.</typeparam>
    /// <typeparam name="TParam7">The type of the param7.</typeparam>
    /// <typeparam name="TParam8">The type of the param8.</typeparam>
    /// <param name="methodBody">The method body.</param>
    /// <param name="argumentNames">The argument names.</param>
    /// <param name="additionalUsings">The additional usings.</param>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="className">Name of the class.</param>
    /// <param name="namespaceName">Name of the namespace.</param>
    /// <param name="sourceSkeleton">The source skeleton.</param>
    /// <param name="additionalAssembliesToReference">The additional assemblies to reference.</param>
    /// <returns></returns>
    public static Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8> CompileAction<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8>(
        string methodBody,
        IReadOnlyCollection<string> argumentNames,
        string additionalUsings = SourceDefaults.USINGS,
        string methodName = SourceDefaults.METHOD,
        string className = SourceDefaults.CLASS,
        string namespaceName = SourceDefaults.NAMESPACE,
        string sourceSkeleton = SourceDefaults.SOURCE_SKELETON,
        params Assembly[] additionalAssembliesToReference)
        => CompileDelegate<Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8>>(methodBody, argumentNames, additionalUsings, methodName, className, namespaceName, sourceSkeleton, additionalAssembliesToReference);

    /// <summary>
    /// Compiles the <paramref name="source"/> to and <see cref="Assembly"/>.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="additionalAssembliesToReference">The additional assemblies to reference.</param>
    /// <returns></returns>
    public static Assembly CompileAssembly(
        string source,
        params Assembly[] additionalAssembliesToReference)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        string assemblyName = Path.GetRandomFileName();

        var references = GenerateReferences(additionalAssembliesToReference);

        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName,
            syntaxTrees: new[] { syntaxTree },
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var compiledAssembly = CompileAssembly(compilation);
        return compiledAssembly;
    }

    /// <summary>
    /// Compiles the <paramref name="methodBody" /> to a <see cref="Delegate" />.
    /// </summary>
    /// <typeparam name="TDelegate">The type of the delegate. Must either be <see cref="Action"/>, <see cref="Func{TResult}"/> or one of their generic overloads.</typeparam>
    /// <param name="methodBody">The method body.</param>
    /// <param name="argumentNames">The argument names.</param>
    /// <param name="additionalUsings">The additional usings.</param>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="className">Name of the class.</param>
    /// <param name="namespaceName">Name of the namespace.</param>
    /// <param name="sourceSkeleton">The source skeleton.</param>
    /// <param name="additionalAssembliesToReference">The additional assemblies to reference.</param>
    /// <returns></returns>
    public static TDelegate CompileDelegate<TDelegate>(
        string methodBody,
        IReadOnlyCollection<string> argumentNames,
        string additionalUsings = SourceDefaults.USINGS,
        string methodName = SourceDefaults.METHOD,
        string className = SourceDefaults.CLASS,
        string namespaceName = SourceDefaults.NAMESPACE,
        string sourceSkeleton = SourceDefaults.SOURCE_SKELETON,
        params Assembly[] additionalAssembliesToReference)
        where TDelegate : Delegate
    {
        var delegateType = typeof(TDelegate);
        var parametersAndResult = GetParametersAndResultTypes(delegateType);
        var isAction = parametersAndResult.result == typeof(void);

        var parameterTypeDeclarations = GetParameterTypeDeclarations(parametersAndResult.parameters);
        var parameters = GetParameters(argumentNames, parameterTypeDeclarations);

        var returnType = isAction ? "void" : GetFullTypeName(parametersAndResult.result);

        var assembliesForTypeArguments = delegateType.GenericTypeArguments
            .Select(a => a.Assembly)
            .Distinct();


        var allSignitureTypes = parameterTypeDeclarations.Select(x => x.type);
        if (!isAction)
            allSignitureTypes = allSignitureTypes.Append(parametersAndResult.result);

        var usingsForSignatureTypes = string.Join(Environment.NewLine,
            allSignitureTypes.Select(d => $"using {d.Namespace};"));

        var method = CompileMethod(
            methodBody,
            parameters,
            returnType,
            additionalUsings + Environment.NewLine + usingsForSignatureTypes,
            methodName,
            className,
            namespaceName,
            sourceSkeleton,
            additionalAssembliesToReference.Concat(assembliesForTypeArguments).Distinct().ToArray());

        var @delegate = (TDelegate)Delegate.CreateDelegate(delegateType, method);

        return @delegate;
    }

    /// <summary>
    /// Compiles the <paramref name="methodBody"/> to a <see cref="Func{TResult}"/>.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="methodBody">The method body.</param>
    /// <param name="additionalUsings">The additional usings.</param>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="className">Name of the class.</param>
    /// <param name="namespaceName">Name of the namespace.</param>
    /// <param name="sourceSkeleton">The source skeleton.</param>
    /// <param name="additionalAssembliesToReference">The additional assemblies to reference.</param>
    /// <returns></returns>
    public static Func<TResult> CompileFunction<TResult>(
        string methodBody,
        string additionalUsings = SourceDefaults.USINGS,
        string methodName = SourceDefaults.METHOD,
        string className = SourceDefaults.CLASS,
        string namespaceName = SourceDefaults.NAMESPACE,
        string sourceSkeleton = SourceDefaults.SOURCE_SKELETON,
        params Assembly[] additionalAssembliesToReference)
        => CompileDelegate<Func<TResult>>(methodBody, Array.Empty<string>(), additionalUsings, methodName, className, namespaceName, sourceSkeleton, additionalAssembliesToReference);

    /// <summary>
    /// Compiles the <paramref name="methodBody"/> to a <see cref="Func{T, TResult}"/>.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="methodBody">The method body.</param>
    /// <param name="argumentName">Name of the argument.</param>
    /// <param name="additionalUsings">The additional usings.</param>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="className">Name of the class.</param>
    /// <param name="namespaceName">Name of the namespace.</param>
    /// <param name="sourceSkeleton">The source skeleton.</param>
    /// <param name="additionalAssembliesToReference">The additional assemblies to reference.</param>
    /// <returns></returns>
    public static Func<TParam, TResult> CompileFunction<TParam, TResult>(
        string methodBody,
        string argumentName = SourceDefaults.ARGUMENT_NAME,
        string additionalUsings = SourceDefaults.USINGS,
        string methodName = SourceDefaults.METHOD,
        string className = SourceDefaults.CLASS,
        string namespaceName = SourceDefaults.NAMESPACE,
        string sourceSkeleton = SourceDefaults.SOURCE_SKELETON,
        params Assembly[] additionalAssembliesToReference)
        => CompileDelegate<Func<TParam, TResult>>(methodBody, new[] { argumentName }, additionalUsings, methodName, className, namespaceName, sourceSkeleton, additionalAssembliesToReference);

    /// <summary>
    /// Compiles the <paramref name="methodBody" /> to a <see cref="Func{T1, T2, TResult}" />.
    /// </summary>
    /// <typeparam name="TParam1">The type of the param1.</typeparam>
    /// <typeparam name="TParam2">The type of the param2.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="methodBody">The method body.</param>
    /// <param name="argumentNames">The argument names.</param>
    /// <param name="additionalUsings">The additional usings.</param>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="className">Name of the class.</param>
    /// <param name="namespaceName">Name of the namespace.</param>
    /// <param name="sourceSkeleton">The source skeleton.</param>
    /// <param name="additionalAssembliesToReference">The additional assemblies to reference.</param>
    /// <returns></returns>
    public static Func<TParam1, TParam2, TResult> CompileFunction<TParam1, TParam2, TResult>(
        string methodBody,
        IReadOnlyCollection<string> argumentNames,
        string additionalUsings = SourceDefaults.USINGS,
        string methodName = SourceDefaults.METHOD,
        string className = SourceDefaults.CLASS,
        string namespaceName = SourceDefaults.NAMESPACE,
        string sourceSkeleton = SourceDefaults.SOURCE_SKELETON,
        params Assembly[] additionalAssembliesToReference)
        => CompileDelegate<Func<TParam1, TParam2, TResult>>(methodBody, argumentNames, additionalUsings, methodName, className, namespaceName, sourceSkeleton, additionalAssembliesToReference);

    /// <summary>
    /// Compiles the <paramref name="methodBody" /> to a <see cref="Func{T1, T2, T3, TResult}" />.
    /// </summary>
    /// <typeparam name="TParam1">The type of the param1.</typeparam>
    /// <typeparam name="TParam2">The type of the param2.</typeparam>
    /// <typeparam name="TParam3">The type of the param3.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="methodBody">The method body.</param>
    /// <param name="argumentNames">The argument names.</param>
    /// <param name="additionalUsings">The additional usings.</param>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="className">Name of the class.</param>
    /// <param name="namespaceName">Name of the namespace.</param>
    /// <param name="sourceSkeleton">The source skeleton.</param>
    /// <param name="additionalAssembliesToReference">The additional assemblies to reference.</param>
    /// <returns></returns>
    public static Func<TParam1, TParam2, TParam3, TResult> CompileFunction<TParam1, TParam2, TParam3, TResult>(
        string methodBody,
        IReadOnlyCollection<string> argumentNames,
        string additionalUsings = SourceDefaults.USINGS,
        string methodName = SourceDefaults.METHOD,
        string className = SourceDefaults.CLASS,
        string namespaceName = SourceDefaults.NAMESPACE,
        string sourceSkeleton = SourceDefaults.SOURCE_SKELETON,
        params Assembly[] additionalAssembliesToReference)
        => CompileDelegate<Func<TParam1, TParam2, TParam3, TResult>>(methodBody, argumentNames, additionalUsings, methodName, className, namespaceName, sourceSkeleton, additionalAssembliesToReference);

    /// <summary>
    /// Compiles the <paramref name="methodBody" /> to a <see cref="Func{T1, T2, T3, T4, TResult}" />.
    /// </summary>
    /// <typeparam name="TParam1">The type of the param1.</typeparam>
    /// <typeparam name="TParam2">The type of the param2.</typeparam>
    /// <typeparam name="TParam3">The type of the param3.</typeparam>
    /// <typeparam name="TParam4">The type of the param4.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="methodBody">The method body.</param>
    /// <param name="argumentNames">The argument names.</param>
    /// <param name="additionalUsings">The additional usings.</param>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="className">Name of the class.</param>
    /// <param name="namespaceName">Name of the namespace.</param>
    /// <param name="sourceSkeleton">The source skeleton.</param>
    /// <param name="additionalAssembliesToReference">The additional assemblies to reference.</param>
    /// <returns></returns>
    public static Func<TParam1, TParam2, TParam3, TParam4, TResult> CompileFunction<TParam1, TParam2, TParam3, TParam4, TResult>(
        string methodBody,
        IReadOnlyCollection<string> argumentNames,
        string additionalUsings = SourceDefaults.USINGS,
        string methodName = SourceDefaults.METHOD,
        string className = SourceDefaults.CLASS,
        string namespaceName = SourceDefaults.NAMESPACE,
        string sourceSkeleton = SourceDefaults.SOURCE_SKELETON,
        params Assembly[] additionalAssembliesToReference)
        => CompileDelegate<Func<TParam1, TParam2, TParam3, TParam4, TResult>>(methodBody, argumentNames, additionalUsings, methodName, className, namespaceName, sourceSkeleton, additionalAssembliesToReference);

    /// <summary>
    /// Compiles the <paramref name="methodBody" /> to a <see cref="Func{T1, T2, T3, T4, T5, TResult}" />.
    /// </summary>
    /// <typeparam name="TParam1">The type of the param1.</typeparam>
    /// <typeparam name="TParam2">The type of the param2.</typeparam>
    /// <typeparam name="TParam3">The type of the param3.</typeparam>
    /// <typeparam name="TParam4">The type of the param4.</typeparam>
    /// <typeparam name="TParam5">The type of the param5.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="methodBody">The method body.</param>
    /// <param name="argumentNames">The argument names.</param>
    /// <param name="additionalUsings">The additional usings.</param>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="className">Name of the class.</param>
    /// <param name="namespaceName">Name of the namespace.</param>
    /// <param name="sourceSkeleton">The source skeleton.</param>
    /// <param name="additionalAssembliesToReference">The additional assemblies to reference.</param>
    /// <returns></returns>
    public static Func<TParam1, TParam2, TParam3, TParam4, TParam5, TResult> CompileFunction<TParam1, TParam2, TParam3, TParam4, TParam5, TResult>(
        string methodBody,
        IReadOnlyCollection<string> argumentNames,
        string additionalUsings = SourceDefaults.USINGS,
        string methodName = SourceDefaults.METHOD,
        string className = SourceDefaults.CLASS,
        string namespaceName = SourceDefaults.NAMESPACE,
        string sourceSkeleton = SourceDefaults.SOURCE_SKELETON,
        params Assembly[] additionalAssembliesToReference)
        => CompileDelegate<Func<TParam1, TParam2, TParam3, TParam4, TParam5, TResult>>(methodBody, argumentNames, additionalUsings, methodName, className, namespaceName, sourceSkeleton, additionalAssembliesToReference);

    /// <summary>
    /// Compiles the <paramref name="methodBody" /> to a <see cref="Func{T1, T2, T3, T4, T5, T6, TResult}" />.
    /// </summary>
    /// <typeparam name="TParam1">The type of the param1.</typeparam>
    /// <typeparam name="TParam2">The type of the param2.</typeparam>
    /// <typeparam name="TParam3">The type of the param3.</typeparam>
    /// <typeparam name="TParam4">The type of the param4.</typeparam>
    /// <typeparam name="TParam5">The type of the param5.</typeparam>
    /// <typeparam name="TParam6">The type of the param6.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="methodBody">The method body.</param>
    /// <param name="argumentNames">The argument names.</param>
    /// <param name="additionalUsings">The additional usings.</param>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="className">Name of the class.</param>
    /// <param name="namespaceName">Name of the namespace.</param>
    /// <param name="sourceSkeleton">The source skeleton.</param>
    /// <param name="additionalAssembliesToReference">The additional assemblies to reference.</param>
    /// <returns></returns>
    public static Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TResult> CompileFunction<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TResult>(
        string methodBody,
        IReadOnlyCollection<string> argumentNames,
        string additionalUsings = SourceDefaults.USINGS,
        string methodName = SourceDefaults.METHOD,
        string className = SourceDefaults.CLASS,
        string namespaceName = SourceDefaults.NAMESPACE,
        string sourceSkeleton = SourceDefaults.SOURCE_SKELETON,
        params Assembly[] additionalAssembliesToReference)
        => CompileDelegate<Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TResult>>(methodBody, argumentNames, additionalUsings, methodName, className, namespaceName, sourceSkeleton, additionalAssembliesToReference);

    /// <summary>
    /// Compiles the <paramref name="methodBody" /> to a <see cref="Func{T1, T2, T3, T4, T5, T6, T7, TResult}" />.
    /// </summary>
    /// <typeparam name="TParam1">The type of the param1.</typeparam>
    /// <typeparam name="TParam2">The type of the param2.</typeparam>
    /// <typeparam name="TParam3">The type of the param3.</typeparam>
    /// <typeparam name="TParam4">The type of the param4.</typeparam>
    /// <typeparam name="TParam5">The type of the param5.</typeparam>
    /// <typeparam name="TParam6">The type of the param6.</typeparam>
    /// <typeparam name="TParam7">The type of the param7.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="methodBody">The method body.</param>
    /// <param name="argumentNames">The argument names.</param>
    /// <param name="additionalUsings">The additional usings.</param>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="className">Name of the class.</param>
    /// <param name="namespaceName">Name of the namespace.</param>
    /// <param name="sourceSkeleton">The source skeleton.</param>
    /// <param name="additionalAssembliesToReference">The additional assemblies to reference.</param>
    /// <returns></returns>
    public static Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TResult> CompileFunction<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TResult>(
        string methodBody,
        IReadOnlyCollection<string> argumentNames,
        string additionalUsings = SourceDefaults.USINGS,
        string methodName = SourceDefaults.METHOD,
        string className = SourceDefaults.CLASS,
        string namespaceName = SourceDefaults.NAMESPACE,
        string sourceSkeleton = SourceDefaults.SOURCE_SKELETON,
        params Assembly[] additionalAssembliesToReference)
        => CompileDelegate<Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TResult>>(methodBody, argumentNames, additionalUsings, methodName, className, namespaceName, sourceSkeleton, additionalAssembliesToReference);

    /// <summary>
    /// Compiles the <paramref name="methodBody" /> to a <see cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, TResult}" />.
    /// </summary>
    /// <typeparam name="TParam1">The type of the param1.</typeparam>
    /// <typeparam name="TParam2">The type of the param2.</typeparam>
    /// <typeparam name="TParam3">The type of the param3.</typeparam>
    /// <typeparam name="TParam4">The type of the param4.</typeparam>
    /// <typeparam name="TParam5">The type of the param5.</typeparam>
    /// <typeparam name="TParam6">The type of the param6.</typeparam>
    /// <typeparam name="TParam7">The type of the param7.</typeparam>
    /// <typeparam name="TParam8">The type of the param8.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="methodBody">The method body.</param>
    /// <param name="argumentNames">The argument names.</param>
    /// <param name="additionalUsings">The additional usings.</param>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="className">Name of the class.</param>
    /// <param name="namespaceName">Name of the namespace.</param>
    /// <param name="sourceSkeleton">The source skeleton.</param>
    /// <param name="additionalAssembliesToReference">The additional assemblies to reference.</param>
    /// <returns></returns>
    public static Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TResult> CompileFunction<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TResult>(
        string methodBody,
        IReadOnlyCollection<string> argumentNames,
        string additionalUsings = SourceDefaults.USINGS,
        string methodName = SourceDefaults.METHOD,
        string className = SourceDefaults.CLASS,
        string namespaceName = SourceDefaults.NAMESPACE,
        string sourceSkeleton = SourceDefaults.SOURCE_SKELETON,
        params Assembly[] additionalAssembliesToReference)
        => CompileDelegate<Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TResult>>(methodBody, argumentNames, additionalUsings, methodName, className, namespaceName, sourceSkeleton, additionalAssembliesToReference);

    /// <summary>
    /// Compiles the <paramref name="methodBody"/> to a <see cref="MethodInfo"/>.
    /// </summary>
    /// <param name="methodBody">The method body.</param>
    /// <param name="arguments">The arguments.</param>
    /// <param name="returnType">Type of the return.</param>
    /// <param name="additionalUsings">The additional usings.</param>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="className">Name of the class.</param>
    /// <param name="namespaceName">Name of the namespace.</param>
    /// <param name="sourceSkeleton">The source skeleton.</param>
    /// <param name="additionalAssembliesToReference">The additional assemblies to reference.</param>
    /// <returns></returns>
    public static MethodInfo CompileMethod(
        string methodBody,
        string arguments = SourceDefaults.NO_ARGUMENTS,
        string returnType = SourceDefaults.RETURN_TYPE,
        string additionalUsings = SourceDefaults.USINGS,
        string methodName = SourceDefaults.METHOD,
        string className = SourceDefaults.CLASS,
        string namespaceName = SourceDefaults.NAMESPACE,
        string sourceSkeleton = SourceDefaults.SOURCE_SKELETON,
        params Assembly[] additionalAssembliesToReference)
    {
        var source = SourceGenerator.GenerateSource(
            methodBody,
            arguments,
            returnType,
            additionalUsings,
            methodName,
            className,
            namespaceName,
            sourceSkeleton);

        var compiledAssembly = CompileAssembly(source, additionalAssembliesToReference);
        return ReflectionHelper.GetMethodFromAssembly(compiledAssembly, methodName, className, namespaceName);
    }

    private static Assembly CompileAssembly(CSharpCompilation compilation)
    {
        Assembly compiledAssembly;
        using (var ms = new MemoryStream())
        {
            EmitResult result = compilation.Emit(ms);

            if (!result.Success)
            {
                var failures = result.Diagnostics
                .Where(diagnostic =>
                    diagnostic.IsWarningAsError ||
                    diagnostic.Severity == DiagnosticSeverity.Error)
                .Select(diagnostic => $"{diagnostic.Id}: {diagnostic.GetMessage()}");

                throw new Exception($"One or more errors happened during compilation:" + Environment.NewLine + string.Join(Environment.NewLine, failures));
            }
            else
            {
                ms.Seek(0, SeekOrigin.Begin);
                compiledAssembly = Assembly.Load(ms.ToArray());
            }
        }

        return compiledAssembly;
    }

    private static IReadOnlyCollection<PortableExecutableReference> GenerateReferences(Assembly[] additionalAssembliesToReference)
    {
        var additionalAssemblyLocations = additionalAssembliesToReference
            .Select(a => a.Location)
            .Select(r => MetadataReference.CreateFromFile(r));

        var references = _defaultReferences
            .Concat(additionalAssemblyLocations)
            .ToList();

        return references;
    }

    private static string GetParameters(IEnumerable<string> names, IEnumerable<(string? modifier, string fullyTypeName, Type type)> parameterTypeDeclarations)
        => string.Join(", ", parameterTypeDeclarations.Zip(names).Select(zipped => zipped.First.modifier + " " + zipped.First.fullyTypeName + " " + zipped.Second));

    private static string GetFullTypeName<T>() => GetFullTypeName(typeof(T));

    private static string GetFullTypeName(Type type)
    {
        var fullName = type.FullName?.Replace('+', '.') ?? throw new ArgumentException($"The FullName of {type} must not be null.", nameof(type));

        if (!type.IsGenericType)
            return fullName;

        string genericArguments = string.Join(", ", type.GetGenericArguments().Select(GetFullTypeName));
        return $"{fullName[..fullName.IndexOf('`')]}<{genericArguments}>";
    }

    private static IEnumerable<(string? modifier, string fullyTypeName, Type type)> GetParameterTypeDeclarations(IEnumerable<ParameterInfo> parameters)
        => parameters.Select(GetParameterTypeDeclaration);

    private static (string? modifier, string fullyTypeName, Type type) GetParameterTypeDeclaration(ParameterInfo parameter)
    {
        if (!parameter.ParameterType.IsByRef)
            return (null, GetFullTypeName(parameter.ParameterType), parameter.ParameterType);

        return (GetModifier(parameter), GetFullTypeName(parameter.ParameterType.GetElementType()!), parameter.ParameterType);

        static string GetModifier(ParameterInfo parameter)
         => parameter switch
         {
             { IsOut: true } => "out",
             { IsIn: true } => "in",
             _ => "ref"
         };
    }

    private static (ParameterInfo[] parameters, Type result) GetParametersAndResultTypes(Type delegateType)
    {
        var invokeMethod = delegateType.GetMethod("Invoke") ?? throw new InvalidOperationException($"Unable to retrieve Invoke method of the type '{delegateType}'. Make sure the type is a delegate.");
        return (invokeMethod.GetParameters(), invokeMethod.ReturnType);
    }
}
namespace RuntimeCompiler;

/// <summary>
/// Contains default values which are used if not all parameters to methods of <see cref="Compiler"/> are provided.
/// </summary>
public static class SourceDefaults
{
    /// <summary>
    /// The default argument name.
    /// </summary>
    public const string ARGUMENT_NAME = "it";

    /// <summary>
    /// The default class name.
    /// </summary>
    public const string CLASS = "Executer";

    /// <summary>
    /// The default method name.
    /// </summary>
    public const string METHOD = "Execute";

    /// <summary>
    /// The default namespace.
    /// </summary>
    public const string NAMESPACE = "DynamicCompilation";

    /// <summary>
    /// An empty argument list.
    /// </summary>
    public const string NO_ARGUMENTS = "";

    /// <summary>
    /// The default type which is void.
    /// </summary>
    public const string RETURN_TYPE = "void";

    /// <summary>
    /// The source skeleton that is used to generate the complete source code from the supplied parameters.
    /// </summary>
    public const string SOURCE_SKELETON = @"
using System;
{0}

namespace {1}
{{
    public static class {2}
    {{
        public static {3} {4}({5})
        {{
            {6}
        }}
    }}
}}";

    /// <summary>
    /// An empty list of using statements.
    /// </summary>
    public const string USINGS = "";
}
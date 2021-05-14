# RuntimeCompiler
`RuntimeCompiler` affers easy to use Methods which will compile C# code to an `Action` or `Func` at runtime.
It uses the Roslyn Compiler platform to acchieve the outcome, so the result is just the same as if you had written code and compiled it down to a normal `Assembly`.

## Usage
### Basic
```
var action = RuntimeCompiler.CompileAction("Console.WriteLine(\"Hello from my dynamic action!\")");
action(); // Output: Hello from my dynamic action!

var func = RuntimeCompiler.CompileFunction<int>("1");
var result = func(1); // result is 1

var greeterFunc = Runtimpiler.CompileFunction<string, string>("\"Hello \" + it");
var greeting = func("World") // greeting is "Hello World"

var sumFunc = RuntimeCompiler.CompileFunction<int, int, int>("x + y", new { "x", "y"});
var sum = func(1, 2); // sum is 3
```

### With method body
```
var methodBody = @"
for (int i = 0; i < times; i++)
{
    Console.WriteLine(""Hello "" + name);
}
";
var multiGreeter = RuntimeCompiler.CompileAction<string, int>(methodBody, new [] { "name", "times" });
multiGreeter("World", 10); // outputs "Hello World" ten times
```

### Custom types
The `System` namespace and all namespaces from parameter or return types are automatically added as using statements and the appropriate Assemblies are automatically imported too.
If you need additional types (and usings), you can supply these as parameters.
```
namespace MyNamespace
{
    public class MyClass
    {
        public int Foo { get; set; }
    }
}

// ...

var methodBody = @"
var x = new MyClass();
x.Foo = 3;
Console.WriteLine(x.Foo);
";

var action = Compiler.CompileAction(methodBody, "using MyNamespace;");
action();

```

### Going deeper
If you ned even more control, you can use `Compiler.CompileMethod(...)` or `Compiler.CompileAssembly(...)`.

## Warning
With great power comes great responsibility!
This will inject code into your running application so it opens up a very high risk for attackers to compromise your systems.
Use it only in environments where you have full control over the inputs given.
You must never allow any unverified user input to be passed in!

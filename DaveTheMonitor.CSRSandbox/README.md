<h1 id="what">What is CSRScript?</h1>

CSRScript is a custom script language for Total Miner heavily based on the existing TMScript. CSRScript has many similarities in syntax and some new functionality that makes it easy for mods to extend its capabilities. Because the syntax is similar, users already faimilar with TMScript should have little difficulty learning CSRScript.

<h1 id="why">Why does CSRScript exist?</h1>

CSRScript was created as a mod-friendly alternative to TMScript. CSRScript was created for use with CSR to make it easier for mods to add custom functionality to the features introduced by the mod without knowlegde of C#.

<h1 id="how">How to use CSRScript</h1>

CSRScript was designed to be similar to TMScript while still adding new features to make it more powerful. Because of this, if you already know TMScript, learning CSRScript should be easy.

If you don't know TMScript, don't worry. The basics below explain everything CSRScript has to offer. If you already know TMScript, you can skip ahead to see exactly what's different here.

<h1 id="index">Index</h1>

| Index                                     |
|-------------------------------------------|
| [What is CSRScript?](#what)               |
| [Why does CSRScript exist?](#why)         |
| [How to use CSRScript?](#how)             |
| [Tokens & Data Types](#tokens)            |
| [Statements & Commands](#statements)      |
| [Comments](#comments)                     |
| [Variables](#variables)                   |
| [- Declaring Variables](#declaring)       |
| [- Initializing Variables](#init)         |
| [- Input Variables](#invars)              |
| [- Binary Operators](#bin-op)             |
| [- Logical Binary Operators](#log-op)     |
| [- Concatenation](#concatenation)         |
| [- Deleting Variables](#deleting)         |
| [If Statements](#if)                      |
| [- Multiple Conditions](#multi-if)        |
| [- If Statement Nesting](#if-nesting)     |
| [While Loops](#while)                     |
| [- While Loop Nesting](#while-nesting)    |
| [Context](#context)                       |
| [- Static Methods](#static)               |
| [- Arrays](#arrays)                       |
| [Differences From TMScript](#differences) |
| [Known Issues](#issues)                   |

<h2 id="tokens">Tokens & Data Types</h2>

Tokens are identifiers or values that can be used in various places. For example, tokens are used to pass values to commands. Tokens are indicated by square brackets around a value. Tokens can hold several types of values:

- **float**: Any number. Can hold integral and decimal numbers.
- **string**: Text, such as "Hello, world!". Strings are always surrounded in double quotation marks.
- **booleans**: True/false values.
- **context**: Any object that can interact with a script, like a player or NPC. Unlike other value types, contexts are usually passed to a script when it is run, rather than created by the script itself.
- **null**: Absence of a value.
- **identifier**: Any value that does not fall into the above categories. Used to access variables, methods, and properties.

| Token Type | Example 1           | Example 2                |
|------------|---------------------|--------------------------|
| Float      | `[4]`               | `[10.6]`                 |
| String     | `["Hello, world!"]` | `["This is CSRScript!"]` |
| Boolean    | `[true]`            | `[false]`                |
| Context    | `[world]`           | `[self]`                 |
| Null       | `[null]`            |                          |
| Identifier | `[players]`         | `[x]`                    |

All of the above examples, with the exception of `Context` and `Identifier`, are **literals**. Literals are static - cannot change - and not associated with any variable. `[6]` is a literal, as is `["John"]`. The identifier `[world]` is not a literal.

<h2 id="statements">Statements & Commands</h2>

CSRScript works with statements. Statements are usually separated by a new line and contain a command to run, and any arguments passed to it. For example, `Print` is a command, and the statement below executes it, passing "Hello, world!".

```c
Print ["Hello, world!"]
```

In the below example, the `Invoke` command is executed with five arguments. In this case, the argument types are, in order, `identifier`, `string`, `float`, `float`, `float`

```c
Invoke [Notify] ["This is a notification."] [255] [0] [255]
```

<h2 id="comments">Comments</h2>

Any text after double slashes on a line is a **comment**. Comments do not affect the execution of the script, and can be used to document how a script works. Below is an example of a comment after a command, and a comment on a separate line.

```c
// We print the float literal [5] here...
Print [5]

Print [true] // .. and the boolean literal [true] here.
```

<h2 id="variables">Variables</h2>

Variables can store values for use later, as well as change them, and can be used anywhere a literal can. Variables can also be used to perform mathmatical calculations on floats.

There are two types of variables: input variables and local variables.
- **Input variables** are passed to the script based on the execution. In most cases, scripts will have access to the `[world]` input variable, and usually the `[self]` variable. Input variables are almost always **context** variables. Input variables are declared with the `In` keyword.
- **Local variables** are variables declared, set, and used by the script itself. These variables are available exclusively to the script, hence the name "local variables".

Below is an example of declaring the input variables `[world]` and `[self]`

```c
In [world] [self]
```

<h3 id="declaring">Declaring Variables</h3>

Local variables can be declared with null values, or initialized with a default value. Variable types are dynamic, meaning they can change during the script's execution. A variable initialized with a float value can later be changed to a string with no issues.

Below is an example of declaring variables with null values, and initializing variables with default values:

```c
// Declaring variables with null values:
Var [target] [attacker]

Print [target] // Output: null
```

The above example outputs null because the variable has been declared, but not initialized. Uninitialized variables are always null by default.

<h3 id="init">Initializing Variables</h3>

Variables can be initialized and given a default value by assigning a value with `=`. Variables, declared or initialized, can also be changed later this way.

```c
// Initializing variables with default values:
// Notice that brackets are not require around operators.
Var [gold] = [100]

Print [gold] // Output: 100
```

Variables can also be initialized by the `Invoke` command by specifying the variable identifier with `[var:]` as the first argument:

```c
Invoke [var:result] [GetBlock] [100] [100] [100]

Print [result] // Output: the result of GetBlock, eg. "Grass"
```

<h3 id="invars">Input Variables</h3>

Input Variables are variables passed to a script on execution. For example, an input variable could be the actor that triggered the script, or the target of an attack. Scripts do not need to set these variables, only declare them. Input variables are declared using the `In` keyword:

```c
In [self]

Print [self] // Output: The string representation of [self], eg. "Player" or "Goblin"
```

<h3 id="bin-op">Binary Operators</h3>

**Binary Operators** can be used to perform operations on two or more values. Addition and subtraction are examples of binary operations. Below is an example of the subtraction binary operator:

```c
Var [damage] = [30]
Var [defense] = [20]

// Again, brackets are not requires around any operators.
Var [total] = [damage] - [defense]

Print [total] // Output: 10
```

The following arithmatic binary operators are available:
- \+ : Addition or string concatenation
- \- : Subtraction
- \* : Multiplication
- / : Division
- % : Modulus (Remainder)

Binary operators are evaluated from right to left. That means the right most operators will be evaluated first, regardless of what they are. See the below example:

```c
Var [x] = [5] + [10] * [2]
// This is evaluated as 5 + (10 * 2)

Print [x] // Output: 25

Var [y] = [10] * [2] + [5]
// This is evaluated as 10 * (2 + 5)

Print [y] // Output: 70
```

<h3 id="log-op">Logical Operators</h3>

**Logical Operators** are binary operators that can be used to compare values and return either true of false depending on the result. Below is an example of the greater than binary operator:

```c
Var [ground] = [100]
Var [height] = [120]

Var [aboveGround] = [height] > [ground]

Print [aboveGround] // Output: true
```

The following logical operators are available:
- == : Equality
- != : Inequality
- \> : Greater than
- \>= : Greater than or equal to
- < : Less than
- <= : Less than or equal to
- && : And
- || : Or

Unlike other binary operators, **and** and **or** operators group sides together before comparing. In other words, they are evaluated last. See the below example:

```c
Var [result] = [5] < [10] && [8] > [4]
// This is evaluated as (5 < 10) && (8 > 4)

Print [result] // Output: true
```

<h3 id="concatenation">Concatenation</h3>

Adding two or more strings together is called **concatenation**. Data types will be automatically converted to strings when they are concatenated. Below is an example of concatenation:

```c
Var [firstName] = ["John"]
Var [lastName] = ["Smith"]

// Note the added space here. Concatenation does not add spaces by default.
Var [fullName] = [firstName] + [" "] + [lastName]

Print [fullName] // Output: "John Smith"

Var [x] = [200]
Var [y] = [300]
Var [z] = [200]

Var [location] = ["Location: "] + [x] + [", "] + [y] + [", "] + [z]
Print [location] // Output: "Location: 200, 300, 200"
```

<h3 id="deleting">Deleting Variables</h3>

If a variable is no longer needed, it can be deleted with the `Delete` command. Most scripts won't need to do this, but for scripts approaching the variable limit, this is a good way to make sure it isn't exceeded:

```c
Var [x] = [100]

Print [x] // Output: 100

Delete [x] // Deletes [x].
// [x] can no longer be used, but can be declared or reinitialized later.
```

It's also possible to delete multiple variables at once:

```c
Var [x] = [100]
Var [y] = [200]
Var [z] = [300]

Var [message] = ["Position: "] + [x] + [", "] + [y] + [", "] + [z]
Print [message]

Delete [x] [y] [z] [message]
```

<h2 id="if">If Statements</h2>

If statements can be used to execute commands conditionally. If statements consist of three parts:
- The condition: This is the condition to test for the If statement.
- The statement: Code here will only execute if the condition was true.
- (optional) The else statement: Code here will only execute if the condition was false.

The end of the condition is signified by `Then`, and all If statements must end with an `Else`, `ElseIf`, or `EndIf`. Below is an example of an If statement comparing the variables `[seaLevel]` and `[groundHeight]` to execute commands conditionally:

```c
Var [x] = [100]
Var [y] = [100]
Var [z] = [100]

Invoke [var:seaLevel] [GetSeaLevel]
Invoke [var:groundHeight] [GetGroundHeight] [x] [y] [z]

If
    // This tests if [groundHeight] < [seaLevel]
    [groundHeight] < [seaLevel]
Then
    // This code only executes if [groundHeight] < [seaLevel]
    Var [message] = ["Block at "] + [x] + [", "] + [y] + [", "] + [z] + [" is underwater"]
    Print [message]
Else
    // This code only executes if the above condition failed.
    Var [message] = ["Block at "] + [x] + [", "] + [y] + [", "] + [z] + [" is not underwater"]
    Print [message]
EndIf
```

<h3 id="multi-if">Testing Multiple Conditions</h3>

Multiple conditions can be tested using logical operators. The following logical operators are available:

- And (&&): Tests if both of the two conditions are true.
- Or (||): Tests if either of the two conditions are true.
- Not (!): Inverts a boolean value. Also called the negation operator.

Unlike most other statements, If conditions can span multiple lines. This allows long queries to be separated for better readability, as shown below:

```c
Var [x] = [100]
Var [y] = [200]
Var [z] = [50]
Var [w] = [75]

If
    // 'or' and '||' are both valid here, as they are equivalent in CSRScript.
    // 'and' and '&&' are equivalent,
    // and 'not' and '!' are equivalent
    [x] > [y] or [z] < [w]
Then
    Print ["Condition true"]
EndIf
// Because no Else is present, nothing will execute only if the condition is false.

// This is also valid, because If conditions can span several lines.
If
    [x] > [y] or
    [z] < [w]
Then
    Print ["Condition true"]
EndIf

// And this is also valid
If
    [x] > [y]
    || [z] < [w]
Then
    Print ["Condition true"]
EndIf

// Output: Condition true
//         Condition true
//         Condition true
```

<h3 id="if-nesting">If Statement Nesting</h3>

If statements can also be nested as many times as needed, as shown below:

```c
Var [condition1] = [true]
Var [condition2] = [false]

If
    // This is equivalent to [condition] == [true]
    [condition1]
Then
    Print ["Condition1 is true"]
    If
        // This is equivalent to [condition] != [true], or [condition] == [false]
        ![condition2]
    Then
        Print ["Condition2 is false"]
    EndIf
EndIf

// Output: Condition1 is true
//         Condition2 is false
```

<h2 id="while">While Loops</h2>

While loops can be used to repeat code several times without having to write it several times. In the below example, the value of `[count]` is printed, then decremented (decreased by 1) until it reaches 0. While loops consist of a Condition and a Statement, like If statements do.

```c
Var [count] = [10]

While
    [count] > [0]
Do
    Print [count]
    Var [count] = [count] - [1]
End

// Output: 10
//         9
//         8
//         7
//         6
//         5
//         4
//         3
//         2
//         1
```

<h3 id="while-nesting">While Loop Nesting</h3>

Like If statements, While loops can be nested:

```c
Var [x] = [0]
Var [y] = [0]

While
    [x] <= [2]
Do
    While
        [y] <= [2]
    Do
        Var [position] = ["Pos: "] + [x] + [", "] + [y]
        Print [position]
        Var [y] = [y] + [1]
    End
    Var [y] = [0]
    Var [x] = [x] + [1]
End

// Output: Pos: 0, 0
//         Pos: 0, 1
//         Pos: 0, 2
//         Pos: 1, 0
//         Pos: 1, 1
//         Pos: 1, 2
//         Pos: 2, 0
//         Pos: 2, 1
//         Pos: 2, 2
```

<h2 id="context">Context</h2>

CSRScripts have an active context that can be switched with the `Context` command. By default, scripts have a null context, and must switch context to use `Invoke`. The context determines what the `Invoke` command targets. In the below example, the "self" context is passed to the script, and the script invokes the 'AddEffect' method.

```c
In [self]

// Switch to self context
Context [self]

// Invoke 'AddEffect' on [self]
Invoke [AddEffect] ["Fire"] [5] [10]

// Switch back to null context
Context [null]

// You can also invoke methods using this alternate syntax,
// which doesn't require you to switch context:

Invoke [self:AddEffect] ["Fire"] [5] [10]
```

Contexts also have properties that can be obtained using the `GetProperty` command:

```c
In [world]

Context [world]
GetProperty [var:players] [Players] // Gets the value of the 'Players' property and stores it in the 'players' variable.
Context [null]

// You can also get properties using this alternate syntax,
// which doesn't require you to switch context:

Var [gameMode] = [world:GameMode]
```

<h3 id="static">Static Methods</h3>

Static methods can be invoked by adding `[static]` as the first argument of the Invoke command. Static methods can be invoked regardless of the current context.

```c
Var [name] = ["John Smith"]

Invoke [static] [var:length] [GetLength] [name]

Print [length] // Output: 10
```

<h3 id="arrays">Arrays</h3>

Arrays are a special type of context variable that can contain several values. Arrays are typically returned by Invoke methods, but can also be created and used exclusively by scripts. Some arrays may be read-only, meaning they cannot be modified. Any attempts to modify read-only arrays will typically throw a runtime error. The following properties are available for arrays:

| PropertyName | Type    |
|--------------|---------|
| Length       | `float` |
| ReadOnly     | `bool`  |

The following invoke methods are available for arrays. Some methods either take or return a type determined by the type of the array. Those types are specified as `T`. Arrays created by scripts use dynamic types.

| Method Name | Arguments                  | Returns        |
|-------------|----------------------------|----------------|
| GetItem     | index: `float`             | item: `T`      |
| SetItem     | index: `float`, item: `T`  |                |
| Insert      | index: `float`, item: `T`  |                |
| Clear       |                            |                |
| Remove      | item: `T`                  |                |
| RemoveAt    | index: `float`             |                |
| Add         | item: `T`                  |                |
| IndexOf     | item: `T`,                 | index: `float` |
| Contains    | item: `T`,                 | index: `bool`  |

New arrays can be created using the Static Invoke method `[CreateArray]`:

```c
Invoke [static] [var:players] [CreateArray]

Print [players] // Output: Array<ScriptVar>
```

<h2 id="differences">Differences From TMScript</h2>

This is a list of major differences between CSRScript and TMScript. For more information on a specific topic, see the relevant link.

- Variables can now store [floats, strings, bools, contexts, and null](#tokens) instead of just doubles. Variable types can change at runtime.
- All string values must be surrounded by double quotation marks.
- Input variables are now defined with the [`In` keyword](#invars)
- Arrays are available as [context](#arrays) variables.
- Most commands are no longer available. Instead, the world or actors have equivalent [Invoke](#context) methods.
- If statements no longer test conditions from commands, instead [comparing variables directly](#if).
- If statements *must* end with EndIf.
- If statements can be nested.
- [While Loops are available.](#while)

<h2 id="issues">Known Issues</h2>

CSRScript is still in early development, so it isn't perfect. There's some issues that affect the execution of the script, whether it be in the runtime or the compiler. Because CSRScript is in early development, anything can change at any time, without any warning. You may have to rewrite some scripts in the future, but I'll try to keep the that to a minimum.

<h3>Issues</h3>

Operator compilation does not handle boolean logical and arithmetic operators together very well. This can result in compiler errors or unexpected behavior in some situations. Until this is fixed, you can use intermediate variables to avoid it and reduce ambiguity. Take the below example:
```c
// This results in a compiler error
Var [result] = [5] + [4] < [8] && [8] > [4]

// Instead, do this:
Var [operand1] = [5] + [4]
Var [result] = [operand1] < [8] && [8] > [4]
// Optionally, Delete [operand1]
```

Some static IDs are parsed as local variables. This has no affect on the execution of the script, but is not intended. You can see this by viewing the generated opcode, some local variable indexes may be skipped.

The compiler does not properly handle errors in some situations (namely when it continues parsing tokens until it reaches one that doesn't exist, like forgetting a `Then` after an `If`). This will be fixed in the future.

Your feedback is appreciated! If you have any suggestions or encounter any issues, let me know and I'll look into them.
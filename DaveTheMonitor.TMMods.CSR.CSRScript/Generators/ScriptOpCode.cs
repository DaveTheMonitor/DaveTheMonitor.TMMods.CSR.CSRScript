using System.Collections.Generic;

namespace DaveTheMonitor.TMMods.CSR.CSRScript.Generators
{
    /// <summary>
    /// Defines an operation for executing a <see cref="Script"/>.
    /// </summary>
    public enum ScriptOpCode : byte
    {
        /// <summary>
        /// No operation.
        /// </summary>
        Nop,
        /// <summary>
        /// Immediately exits the script.
        /// </summary>
        Exit,
        /// <summary>
        /// Pops the value on top of the stack and does nothing with it.
        /// </summary>
        Pop,
        /// <summary>
        /// Pushes a constant float on to the stack.
        /// </summary>
        FloatLiteral,
        /// <summary>
        /// Pushes a constant string on to the stack.
        /// </summary>
        StringLiteral,
        /// <summary>
        /// Pushes true on to the stack.
        /// </summary>
        TrueLiteral,
        /// <summary>
        /// Pushes false on to the stack.
        /// </summary>
        FalseLiteral,
        /// <summary>
        /// Pushes null on to the stack.
        /// </summary>
        Null,
        /// <summary>
        /// Loads a variable by index on to the stack.
        /// </summary>
        LoadVar,
        /// <summary>
        /// Stores the value on top of the stack to a local variable by index.
        /// </summary>
        StoreVar,
        /// <summary>
        /// Deletes a local variable by index.
        /// </summary>
        Delete,
        /// <summary>
        /// Prints the value on top of the stack for debugging.
        /// </summary>
        Print,
        /// <summary>
        /// Adds the two values on top of the stack and pushes the result on to the stack.
        /// </summary>
        Add,
        /// <summary>
        /// Subtracts the two values on top of the stack and pushes the result on to the stack.
        /// </summary>
        Sub,
        /// <summary>
        /// Multiplies the two values on top of the stack and pushes the result on to the stack.
        /// </summary>
        Multi,
        /// <summary>
        /// Divides the two values on top of the stack and pushes the result on to the stack.
        /// </summary>
        Div,
        /// <summary>
        /// Computes the modulus of the two values on top of the stack and pushes the result on to the stack.
        /// </summary>
        Mod,
        /// <summary>
        /// Compares the two values on top of the stack and pushes true if they are equal, otherwise false.
        /// </summary>
        Equal,
        /// <summary>
        /// Compares the two values on top of the stack and pushes true if they are not equal, otherwise false.
        /// </summary>
        NotEqual,
        /// <summary>
        /// Tests if both of the two values on top of the stack are true and pushes true if they are, otherwise false.
        /// </summary>
        And,
        /// <summary>
        /// Tests if either of the two values on top of the stack are true and pushes true if they are, otherwise false.
        /// </summary>
        Or,
        /// <summary>
        /// Inverts the value on top of the stack. Only works on bools.
        /// </summary>
        Invert,
        /// <summary>
        /// Compares the two values on top of the stack and pushes true if the first value is greater than the second value, otherwise false.
        /// </summary>
        Gt,
        /// <summary>
        /// Compares the two values on top of the stack and pushes true if the first value is greater than or equal to the second value, otherwise false.
        /// </summary>
        Gte,
        /// <summary>
        /// Compares the two values on top of the stack and pushes true if the first value is less than the second value, otherwise false.
        /// </summary>
        Lt,
        /// <summary>
        /// Compares the two values on top of the stack and pushes true if the first value is less than or equal to the second value, otherwise false.
        /// </summary>
        Lte,
        /// <summary>
        /// Unconditionally jumps to the specified position of the op list.
        /// </summary>
        Jump,
        /// <summary>
        /// Jumps to the specified position of the op list if the value on top of the stack is true.
        /// </summary>
        JT,
        /// <summary>
        /// Jumps to the specified position of the op list if the value on top of the stack is false.
        /// </summary>
        JF,
        /// <summary>
        /// Jumps to the specified position of the op list if the two values on top of the stack are equal.
        /// </summary>
        JE,
        /// <summary>
        /// Jumps to the specified position of the op list if the two values on top of the stack are not equal.
        /// </summary>
        JNe,
        /// <summary>
        /// Jumps to the specified position of the op list if the first value is greater than the second value.
        /// </summary>
        JGt,
        /// <summary>
        /// Jumps to the specified position of the op list if the first value is greater than or equal to the second value.
        /// </summary>
        JGte,
        /// <summary>
        /// Jumps to the specified position of the op list if the first value is less than the second value.
        /// </summary>
        JLt,
        /// <summary>
        /// Jumps to the specified position of the op list if the first value is less than or equal to the second value.
        /// </summary>
        JLte,
        /// <summary>
        /// Switches the script's execution context to the value on top of the stack.
        /// </summary>
        Context,
        /// <summary>
        /// Pushes the current context on to the stack. Does not switch context.
        /// </summary>
        PushContext,
        /// <summary>
        /// Invokes a method of the current context. Calls <see cref="IScriptContext.Invoke(IScriptRuntime, string, IList{ScriptVar})"/>
        /// </summary>
        Invoke,
        /// <summary>
        /// Invokes a method of the current context and pushes the value on to the stack. Calls <see cref="IScriptContext.Invoke(IScriptRuntime, string, IList{ScriptVar})"/>
        /// </summary>
        InvokeRet,
        /// <summary>
        /// Invokes a static method. Calls <see cref="IScriptRuntime.InvokeStatic(string, IList{ScriptVar})"/>
        /// </summary>
        InvokeStatic,
        /// <summary>
        /// Invokes a static method and pushes the value on to the stack. Calls <see cref="IScriptRuntime.InvokeStatic(string, IList{ScriptVar})"/>
        /// </summary>
        InvokeStaticRet,
        /// <summary>
        /// Gets a property of the current context and pushes the value on to the stack. Calls <see cref="IScriptContext.GetProperty(IScriptRuntime, string)"/>
        /// </summary>
        GetProperty
    }
}

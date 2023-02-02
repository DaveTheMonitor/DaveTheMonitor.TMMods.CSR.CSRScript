using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace DaveTheMonitor.TMMods.CSR.CSRScript
{
    /// <summary>
    /// A script error for use with a script runtime.
    /// </summary>
    public sealed class ScriptError
    {
        /// <summary>
        /// The error header.
        /// </summary>
        public string Header { get; private set; }
        /// <summary>
        /// The error message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Creates a new script error for an invalid argument count. Used by invoke methods.
        /// </summary>
        /// <param name="expected">The number of arguments expected.</param>
        /// <param name="received">The number of arguments received.</param>
        /// <returns>A new script error.</returns>
        public static ScriptError InvalidArgCountError(int expected, int received)
        {
            return new ScriptError("Invalid Argument Count", $"Expected {expected} arguments, received {received}");
        }

        /// <summary>
        /// Creates a new script error for an invalid argument count. Used by invoke methods.
        /// </summary>
        /// <param name="expected">The number of arguments expected.</param>
        /// <param name="received">The number of arguments received.</param>
        /// <param name="type">The type defining the method where the error occurred.</param>
        /// <param name="method">The method where the error occurred.</param>
        /// <returns>A new script error.</returns>
        public static ScriptError InvalidArgCount(int expected, int received, Type type, string method)
        {
            return new ScriptError("Invalid Argument Count", $"{type.Name}:{method} expected {expected} arguments, received {received}.");
        }

        /// <summary>
        /// Creates a new script error for an invalid type. Used by variable type checks.
        /// </summary>
        /// <param name="expected">The type expected.</param>
        /// <param name="received">The type received.</param>
        /// <returns>A new script error.</returns>
        public static ScriptError InvalidTypeError(Type expected, Type received)
        {
            return new ScriptError("Invalid Type", $"Type is {received.Name}, expected {expected.Name}");
        }

        /// <summary>
        /// Creates a new script error for an invalid type. Used by variable type checks.
        /// </summary>
        /// <param name="expected">The type expected.</param>
        /// <param name="received">The type received.</param>
        /// <returns>A new script error.</returns>
        public static ScriptError InvalidTypeError(ScriptVarType expected, ScriptVarType received)
        {
            return new ScriptError("Invalid Type", $"Type is {received}, expected {expected}");
        }

        /// <summary>
        /// Creates a new script error for an invalid property. Used when a property does not exist.
        /// </summary>
        /// <param name="type">The type where the error occurred.</param>
        /// <param name="property">The name of the property.</param>
        /// <returns>A new script error.</returns>
        public static ScriptError InvalidPropertyError(Type type, string property)
        {
            return new ScriptError("Invalid Property", $"Property {property} does not exist on {type.Name}");
        }

        /// <summary>
        /// Creates a new script error for an invalid method. Used when a method does not exist.
        /// </summary>
        /// <param name="type">The type where the error occurred.</param>
        /// <param name="method">The name of the method.</param>
        /// <returns>A new script error.</returns>
        public static ScriptError InvalidMethodError(Type type, string method)
        {
            return new ScriptError("Invalid Method", $"Invoke method {method} does not exist on {type.Name}");
        }

        /// <summary>
        /// Creates a new script error for invalid operation types. Used by <see cref="ScriptVar"/> operations.
        /// </summary>
        /// <param name="type">The type where the error occurred.</param>
        /// <returns>A new script error.</returns>
        public static ScriptError InvalidOperaionTypeError(ScriptVarType type)
        {
            return new ScriptError("Invalid Operation Types", $"{type} values cannot be operated on.");
        }

        /// <summary>
        /// Creates a new script error for script timeouts.
        /// </summary>
        /// <param name="time">The time, in milliseconds.</param>
        /// <returns>A new script error.</returns>
        public static ScriptError TimeoutError(int time)
        {
            return new ScriptError("Script Timeout", $"Script execution timed out ({time} ms)");
        }

        /// <summary>
        /// Creates a new script error for stack overflow.
        /// </summary>
        /// <param name="stackSize">The stack size.</param>
        /// <returns>A new script error.</returns>
        public static ScriptError StackOverflowError(int stackSize)
        {
            return new ScriptError("Stack Overflow", $"Maximum stack size {stackSize} exceeded.");
        }

        /// <summary>
        /// Creates a new script error for exceeding the maximum local count.
        /// </summary>
        /// <param name="locals">The maximum local count.</param>
        /// <returns>A new script error.</returns>
        public static ScriptError MaxLocalsError(int locals)
        {
            return new ScriptError("Maximum Local Count Exceeded", $"Exceeded maximum local variable count {locals}. Delete unneeded variables with the delete command.");
        }

        /// <summary>
        /// Creates a new script error for an undefined operation
        /// </summary>
        /// <returns>A new script error.</returns>
        public static ScriptError UndefinedOperation()
        {
            return new ScriptError("Undefined Operation", "Undefined operation.");
        }

        /// <summary>
        /// Creates a new script error with the specified header and message.
        /// </summary>
        /// <param name="header">The error header.</param>
        /// <param name="message">The error message.</param>
        public ScriptError(string header, string message)
        {
            Header = header;
            Message = message;
        }
    }
}

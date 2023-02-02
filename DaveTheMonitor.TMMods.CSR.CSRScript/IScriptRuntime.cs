using System;
using System.Collections.Generic;

namespace DaveTheMonitor.TMMods.CSR.CSRScript
{
    /// <summary>
    /// Defines a runtime that can run <see cref="Script"/>s.
    /// </summary>
    public interface IScriptRuntime
    {
        /// <summary>
        /// Causes a runtime error.
        /// </summary>
        /// <param name="error">The error that occurred.</param>
        void Error(ScriptError error);

        /// <summary>
        /// Runs a script.
        /// </summary>
        /// <param name="script">The script to run.</param>
        /// <param name="init">The action called when the script is initialized. This is used to pass in vars to the script, such as contexts (eg. world).</param>
        /// <param name="timeout">The time, in milliseconds, before execution forcefully stops.</param>
        void Run(Script script, Action<IScriptRuntime> init, int timeout);

        /// <summary>
        /// Sets an in variable. Should only be called on script initialization.
        /// </summary>
        /// <param name="name">The name of the variable to set.</param>
        /// <param name="value">The value to set.</param>
        void InitVar(string name, ScriptVar value);

        /// <summary>
        /// Invokes a method of the current context.
        /// </summary>
        /// <param name="name">The name of the method to invoke.</param>
        /// <param name="args">The arguments passed to the method.</param>
        /// <returns>A script variable, or null if the method does not return a value.</returns>
        ScriptVar Invoke(string name, IList<ScriptVar> args);

        /// <summary>
        /// Invokes a static runtime method.
        /// </summary>
        /// <param name="name">The name of the method to invoke.</param>
        /// <param name="args">The arguments passed to the method.</param>
        /// <returns>A script variable, or null if the method does not return a value.</returns>
        ScriptVar InvokeStatic(string name, IList<ScriptVar> args);
    }
}

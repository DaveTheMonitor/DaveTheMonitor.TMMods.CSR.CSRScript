using System.Collections.Generic;

namespace DaveTheMonitor.TMMods.CSR.CSRScript
{
    /// <summary>
    /// A context that can be used in a <see cref="Script"/>.
    /// </summary>
    public interface IScriptContext
    {
        /// <summary>
        /// Returns a property with the specified <paramref name="name"/> as a <see cref="ScriptVar"/>.
        /// </summary>
        /// <param name="runtime">The <see cref="IScriptRuntime"/> executing the <see cref="Script"/> calling this method.</param>
        /// <param name="name">The name of the property to get.</param>
        /// <returns>A <see cref="ScriptVar"/> with the value of the property.</returns>
        ScriptVar GetProperty(IScriptRuntime runtime, string name);

        /// <summary>
        /// Sets a property with the specified <paramref name="name"/> to <paramref name="value"/>.
        /// </summary>
        /// <param name="runtime">The <see cref="IScriptRuntime"/> executing the <see cref="Script"/> calling this method.</param>
        /// <param name="name">The name of the property to set.</param>
        /// <param name="value">The value to set the property to.</param>
        void SetProperty(IScriptRuntime runtime, string name, ScriptVar value);

        /// <summary>
        /// Gets the friendly name of this context. Used when converting a context to a string or getting the "Name" property.
        /// </summary>
        /// <returns>The name of this context.</returns>
        string GetName();

        /// <summary>
        /// Invokes a method of the current context.
        /// </summary>
        /// <param name="runtime">The <see cref="IScriptRuntime"/> executing the <see cref="Script"/> calling this method.</param>
        /// <param name="name">The name of the method to invoke.</param>
        /// <param name="args">The arguments passed to the method. Do not store this list as it can change when invoking other methods.</param>
        /// <returns>A <see cref="ScriptVar"/> with the returned value of the method, or <see cref="ScriptVar.Null"/> if no value is returned.</returns>
        ScriptVar Invoke(IScriptRuntime runtime, string name, IList<ScriptVar> args);
    }
}

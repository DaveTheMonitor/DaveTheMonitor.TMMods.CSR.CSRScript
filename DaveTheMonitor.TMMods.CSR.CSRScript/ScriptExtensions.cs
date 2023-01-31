using System.Collections.Generic;

namespace DaveTheMonitor.TMMods.CSR.CSRScript
{
    /// <summary>
    /// Extensions to help with script integration.
    /// </summary>
    public static class ScriptExtensions
    {
        /// <summary>
        /// Creates a new Script Array instance for script access. The Script Array uses the same <paramref name="list"/> that is passed to it; the <paramref name="list"/> is not copied. It is recommended to call this only once and return the same instance for all calls.
        /// </summary>
        /// <param name="list">The list to create the Script Array from.</param>
        /// <param name="readOnly">If true, the Script Array cannot be modified. Attempts to modify the Script Array will throw a runtime error.</param>
        /// <returns>A new Script Array instance for the <paramref name="list"/>.</returns>
        public static IScriptArray CreateScriptArray(this IList<float> list, bool readOnly)
        {
            return new ScriptArrayFloat(list, readOnly);
        }

        /// <inheritdoc cref="CreateScriptArray(IList{float}, bool)"/>
        public static IScriptArray CreateScriptArray(this IList<bool> list, bool readOnly)
        {
            return new ScriptArrayBool(list, readOnly);
        }

        /// <inheritdoc cref="CreateScriptArray(IList{float}, bool)"/>
        public static IScriptArray CreateScriptArray(this IList<string> list, bool readOnly)
        {
            return new ScriptArrayString(list, readOnly);
        }

        /// <inheritdoc cref="CreateScriptArray(IList{float}, bool)"/>
        /// <typeparam name="T">The type in the array. Must be an <see cref="IScriptContext"/>.</typeparam>
        public static IScriptArray CreateScriptArray<T>(this IList<T> list, bool readOnly) where T : IScriptContext
        {
            return new ScriptArrayContext<T>(list, readOnly);
        }

        /// <inheritdoc cref="CreateScriptArray(IList{float}, bool)"/>
        public static IScriptArray CreateScriptArray(this IList<ScriptVar> list, bool readOnly)
        {
            return new ScriptArrayScriptVar(list, readOnly);
        }
    }
}

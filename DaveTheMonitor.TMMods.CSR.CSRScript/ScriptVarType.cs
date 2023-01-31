namespace DaveTheMonitor.TMMods.CSR.CSRScript
{
    /// <summary>
    /// Defines the type of a <see cref="ScriptVar"/>.
    /// </summary>
    public enum ScriptVarType : byte
    {
        /// <summary>
        /// Specifies that a <see cref="ScriptVar"/> is null.
        /// </summary>
        Null,
        /// <summary>
        /// Specifies that a <see cref="ScriptVar"/> is a <see cref="float"/>.
        /// </summary>
        Float,
        /// <summary>
        /// Specifies that a <see cref="ScriptVar"/> is a <see cref="bool"/>.
        /// </summary>
        Bool,
        /// <summary>
        /// Specifies that a <see cref="ScriptVar"/> is a <see cref="string"/>.
        /// </summary>
        String,
        /// <summary>
        /// Specifies that a <see cref="ScriptVar"/> is a <see cref="IScriptContext"/>.
        /// </summary>
        Context
    }
}

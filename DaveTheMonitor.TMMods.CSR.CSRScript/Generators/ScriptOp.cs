namespace DaveTheMonitor.TMMods.CSR.CSRScript.Generators
{
    /// <summary>
    /// Defines an operation for a <see cref="Script"/>. Some operations may have several arguments. This class may be casted to the appropriate derived class based on the value of <see cref="Op"/>;
    /// </summary>
    public class ScriptOp
    {
        /// <summary>
        /// The operation performed by this ScriptOp.
        /// </summary>
        public ScriptOpCode Op { get; private set; }

        /// <summary>
        /// Gets the argument at the specified index. For <see cref="ScriptOpCode"/>s with no string args, this always returns null.
        /// </summary>
        /// <param name="index">The index of the argument to get.</param>
        /// <returns>The specified argument.</returns>
        public virtual string GetArgString(int index)
        {
            return null;
        }

        /// <summary>
        /// Gets the argument at the specified index. For <see cref="ScriptOpCode"/>s with no float args, this always returns 0.
        /// </summary>
        /// <param name="index">The index of the argument to get.</param>
        /// <returns>The specified argument.</returns>
        public virtual float GetArgFloat(int index)
        {
            return 0;
        }

        /// <summary>
        /// Gets the argument at the specified index. For <see cref="ScriptOpCode"/>s with no int args, this always returns 0.
        /// </summary>
        /// <param name="index">The index of the argument to get.</param>
        /// <returns>The specified argument.</returns>
        public virtual int GetArgInt(int index)
        {
            return 0;
        }

        /// <summary>
        /// Gets the argument at the specified index. For <see cref="ScriptOpCode"/>s with no bool args, this always returns false.
        /// </summary>
        /// <param name="index">The index of the argument to get.</param>
        /// <returns>The specified argument.</returns>
        public virtual bool GetArgBool(int index)
        {
            return false;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Op.ToString();
        }

        /// <summary>
        /// Creates a new ScriptOp with the specified <see cref="ScriptOpCode"/>.
        /// </summary>
        /// <param name="op">The operation this ScriptOp specifies.</param>
        /// <remarks>
        /// <para>This should only be used with <see cref="ScriptOpCode"/>s with no additional parameters, ie. <see cref="ScriptOpCode.Nop"/>, <see cref="ScriptOpCode.Null"/> or <see cref="ScriptOpCode.Pop"/>.</para>
        /// </remarks>
        public ScriptOp(ScriptOpCode op)
        {
            Op = op;
        }
    }
}

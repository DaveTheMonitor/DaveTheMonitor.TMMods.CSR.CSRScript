using DaveTheMonitor.TMMods.CSR.CSRScript.Generators;
using System.Collections.Generic;

namespace DaveTheMonitor.TMMods.CSR.CSRScript
{
    /// <summary>
    /// Defines a script that can be run by a <see cref="IScriptRuntime"/>.
    /// </summary>
    public sealed class Script
    {
        /// <summary>
        /// True if this script was compiled. False if this script was generated.
        /// </summary>
        public bool Compiled { get; private set; }
        /// <summary>
        /// The name of this script.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// A read-only collection of operations of this script.
        /// </summary>
        public ScriptOp[] Ops { get; private set; }
        /// <summary>
        /// A dictionary that maps input variable names to local in vars, eg. world.
        /// </summary>
        public Dictionary<string, int> InVars { get; internal set; }

        internal void SetOps(ScriptOp[] ops, bool compiled)
        {
            Ops = ops;
            Compiled = compiled;
        }

        internal Script(string scriptName)
        {
            Ops = null;
            Name = scriptName;
            InVars = new Dictionary<string, int>();
        }
    }
}

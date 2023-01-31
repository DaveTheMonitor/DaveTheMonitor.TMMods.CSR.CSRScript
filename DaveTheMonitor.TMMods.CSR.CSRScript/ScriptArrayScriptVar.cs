using System.Collections.Generic;

namespace DaveTheMonitor.TMMods.CSR.CSRScript
{
    internal sealed class ScriptArrayScriptVar : ScriptArrayInternal<ScriptVar>
    {
        protected override bool TryGetT(ScriptVar var, IScriptRuntime runtime, out ScriptVar value)
        {
            value = var;
            return true;
        }

        protected override ScriptVar GetScriptVar(ScriptVar value)
        {
            return value;
        }

        public ScriptArrayScriptVar(IList<ScriptVar> list, bool readOnly) : base(list, readOnly)
        {

        }
    }
}

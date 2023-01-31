using System.Collections.Generic;

namespace DaveTheMonitor.TMMods.CSR.CSRScript
{
    internal sealed class ScriptArrayBool : ScriptArrayInternal<bool>
    {
        protected override bool TryGetT(ScriptVar var, IScriptRuntime runtime, out bool value)
        {
            value = var.GetBoolValue(runtime);
            return var.IsFloat;
        }

        protected override ScriptVar GetScriptVar(bool value)
        {
            return new ScriptVar(value);
        }

        public ScriptArrayBool(IList<bool> list, bool readOnly) : base(list, readOnly)
        {

        }
    }
}

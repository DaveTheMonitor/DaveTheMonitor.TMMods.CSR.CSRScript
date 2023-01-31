using System.Collections.Generic;

namespace DaveTheMonitor.TMMods.CSR.CSRScript
{
    internal sealed class ScriptArrayFloat : ScriptArrayInternal<float>
    {
        protected override bool TryGetT(ScriptVar var, IScriptRuntime runtime, out float value)
        {
            value = var.GetFloatValue(runtime);
            return var.IsFloat;
        }

        protected override ScriptVar GetScriptVar(float value)
        {
            return new ScriptVar(value);
        }

        public ScriptArrayFloat(IList<float> list, bool readOnly) : base(list, readOnly)
        {

        }
    }
}

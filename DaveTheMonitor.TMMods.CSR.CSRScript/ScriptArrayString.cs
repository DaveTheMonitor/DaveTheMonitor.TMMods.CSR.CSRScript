using System.Collections.Generic;

namespace DaveTheMonitor.TMMods.CSR.CSRScript
{
    internal sealed class ScriptArrayString : ScriptArrayInternal<string>
    {
        protected override bool TryGetT(ScriptVar var, IScriptRuntime runtime, out string value)
        {
            value = var.GetStringValue(runtime);
            return var.IsString;
        }

        protected override ScriptVar GetScriptVar(string value)
        {
            return new ScriptVar(value);
        }

        public ScriptArrayString(IList<string> list, bool readOnly) : base(list, readOnly)
        {

        }
    }
}

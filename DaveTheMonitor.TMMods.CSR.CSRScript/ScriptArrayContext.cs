using System.Collections.Generic;

namespace DaveTheMonitor.TMMods.CSR.CSRScript
{
    internal sealed class ScriptArrayContext<T> : ScriptArrayInternal<T> where T : IScriptContext
    {
        protected override bool TryGetT(ScriptVar var, IScriptRuntime runtime, out T value)
        {
            IScriptContext context = var.GetContextValue(runtime);
            if (context is T v)
            {
                value = v;
                return true;
            }
            else
            {
                runtime?.Error(ScriptError.InvalidTypeError(typeof(T), context.GetType()));
                value = default(T);
                return false;
            }
        }

        protected override ScriptVar GetScriptVar(T value)
        {
            return new ScriptVar(value);
        }

        public ScriptArrayContext(IList<T> list, bool readOnly) : base(list, readOnly)
        {

        }
    }
}

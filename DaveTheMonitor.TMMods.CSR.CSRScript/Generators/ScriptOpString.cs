namespace DaveTheMonitor.TMMods.CSR.CSRScript.Generators
{
    internal sealed class ScriptOpString : ScriptOp
    {
        private string _value;

        public override string GetArgString(int index)
        {
            return index == 0 ? _value : null;
        }

        public override string ToString()
        {
            return $"{Op} \"{_value}\"";
        }

        public ScriptOpString(ScriptOpCode op, string value) : base(op)
        {
            _value = value;
        }
    }
}

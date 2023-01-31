namespace DaveTheMonitor.TMMods.CSR.CSRScript.Generators
{
    internal sealed class ScriptOpInt : ScriptOp
    {
        private int _value;

        public override int GetArgInt(int index)
        {
            return index == 0 ? _value : 0;
        }

        public override string ToString()
        {
            return $"{Op} : {_value}";
        }

        public ScriptOpInt(ScriptOpCode op, int value) : base(op)
        {
            _value = value;
        }
    }
}

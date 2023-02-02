namespace DaveTheMonitor.TMMods.CSR.CSRScript.Generators
{
    internal sealed class ScriptOpFloat : ScriptOp
    {
        private float _value;

        public override float GetArgFloat(int index)
        {
            return index == 0 ? _value : 0;
        }

        public override string ToString()
        {
            return $"{Op} {_value}";
        }

        public ScriptOpFloat(ScriptOpCode op, float value) : base(op)
        {
            _value = value;
        }
    }
}

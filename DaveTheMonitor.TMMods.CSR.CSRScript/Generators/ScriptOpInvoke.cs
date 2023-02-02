namespace DaveTheMonitor.TMMods.CSR.CSRScript.Generators
{
    internal sealed class ScriptOpInvoke : ScriptOp
    {
        private string _name;
        private int _args;

        public override string GetArgString(int index)
        {
            return index == 0 ? _name : null;
        }

        public override int GetArgInt(int index)
        {
            return index == 1 ? _args : 0;
        }

        public override string ToString()
        {
            return $"{Op} {_name} {_args}";
        }

        public ScriptOpInvoke(ScriptOpCode op, string name, int args) : base(op)
        {
            _name = name;
            _args = args;
        }
    }
}

namespace DaveTheMonitor.TMMods.CSR.CSRScript.Generators.Compiler
{
    internal sealed class BoolLiteralScriptTreeNode : ScriptTreeNode
    {
        public bool Value { get; private set; }

        public BoolLiteralScriptTreeNode(long cha, bool value) : base(ScriptTreeNodeType.BoolLiteral, cha)
        {
            Value = value;
        }
    }
}

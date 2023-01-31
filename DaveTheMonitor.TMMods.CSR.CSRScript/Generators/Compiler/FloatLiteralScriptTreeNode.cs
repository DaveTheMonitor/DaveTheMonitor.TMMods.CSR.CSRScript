namespace DaveTheMonitor.TMMods.CSR.CSRScript.Generators.Compiler
{
    internal sealed class FloatLiteralScriptTreeNode : ScriptTreeNode
    {
        public float Value { get; private set; }

        public FloatLiteralScriptTreeNode(long cha, float value) : base(ScriptTreeNodeType.FloatLiteral, cha)
        {
            Value = value;
        }
    }
}

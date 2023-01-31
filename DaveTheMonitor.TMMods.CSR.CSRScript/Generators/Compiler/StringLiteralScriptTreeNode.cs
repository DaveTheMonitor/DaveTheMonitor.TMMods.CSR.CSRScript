namespace DaveTheMonitor.TMMods.CSR.CSRScript.Generators.Compiler
{
    internal sealed class StringLiteralScriptTreeNode : ScriptTreeNode
    {
        public string Value { get; private set; }

        public StringLiteralScriptTreeNode(long cha, string value) : base(ScriptTreeNodeType.StringLiteral, cha)
        {
            Value = value;
        }
    }
}

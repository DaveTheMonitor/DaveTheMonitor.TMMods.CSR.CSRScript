namespace DaveTheMonitor.TMMods.CSR.CSRScript.Generators.Compiler
{
    internal sealed class BinExprScriptTreeNode : ScriptTreeNode
    {
        public ScriptTreeNode Left { get; set; }
        public string Op { get; private set; }
        public ScriptTreeNode Right { get; set; }

        public BinExprScriptTreeNode(long cha, string op) : base(ScriptTreeNodeType.BinaryExpression, cha)
        {
            Left = null;
            Op = op;
            Right = null;
        }
    }
}

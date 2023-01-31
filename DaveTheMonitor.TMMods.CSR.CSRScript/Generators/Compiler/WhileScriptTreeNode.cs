namespace DaveTheMonitor.TMMods.CSR.CSRScript.Generators.Compiler
{
    internal sealed class WhileScriptTreeNode : ScriptTreeNode
    {
        public ScriptTreeNode Condition { get; set; }
        public ScriptScriptTreeNode Statement { get; set; }

        public WhileScriptTreeNode(long cha) : base(ScriptTreeNodeType.WhileStatement, cha)
        {
            Condition = null;
            Statement = null;
        }
    }
}

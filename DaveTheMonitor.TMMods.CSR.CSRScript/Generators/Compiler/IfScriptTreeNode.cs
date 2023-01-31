namespace DaveTheMonitor.TMMods.CSR.CSRScript.Generators.Compiler
{
    internal sealed class IfScriptTreeNode : ScriptTreeNode
    {
        public ScriptTreeNode Condition { get; set; }
        public ScriptScriptTreeNode Statement { get; set; }
        public ScriptScriptTreeNode ElseStatement { get; set; }

        public IfScriptTreeNode(long cha) : base(ScriptTreeNodeType.IfStatement, cha)
        {
            Condition = null;
            Statement = null;
            ElseStatement = null;
        }
    }
}

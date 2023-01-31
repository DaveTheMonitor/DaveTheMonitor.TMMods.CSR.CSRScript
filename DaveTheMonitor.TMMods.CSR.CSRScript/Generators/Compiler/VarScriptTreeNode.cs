namespace DaveTheMonitor.TMMods.CSR.CSRScript.Generators.Compiler
{
    internal sealed class VarScriptTreeNode : ScriptTreeNode
    {
        public IDScriptTreeNode ID { get; set; }
        public ScriptTreeNode Value { get; set; }

        public VarScriptTreeNode(long cha) : base(ScriptTreeNodeType.Var, cha)
        {
            ID = null;
            Value = null;
        }
    }
}

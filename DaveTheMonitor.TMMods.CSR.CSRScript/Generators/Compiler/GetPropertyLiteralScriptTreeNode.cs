namespace DaveTheMonitor.TMMods.CSR.CSRScript.Generators.Compiler
{
    internal sealed class GetPropertyScriptTreeNode : ScriptTreeNode
    {
        public IDScriptTreeNode Var { get; private set; }
        public IDScriptTreeNode Property { get; private set; }

        public GetPropertyScriptTreeNode(long cha, IDScriptTreeNode var, IDScriptTreeNode property) : base(ScriptTreeNodeType.GetProperty, cha)
        {
            Var = var;
            Property = property;
        }
    }
}

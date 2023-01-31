namespace DaveTheMonitor.TMMods.CSR.CSRScript.Generators.Compiler
{
    internal sealed class IDScriptTreeNode : ScriptTreeNode
    {
        public string ID { get; internal set; }
        public int Index { get; private set; }
        public bool Store { get; internal set; }

        public void MakeStatic()
        {
            Type = ScriptTreeNodeType.StaticID;
        }

        public void RecalcIndex(ScriptScriptTreeNode tree)
        {
            Index = tree.GetVarIndex(ID);
        }

        public IDScriptTreeNode(long cha, string id, ScriptScriptTreeNode tree) : base(ScriptTreeNodeType.ID, cha)
        {
            ID = id;
            Store = false;
            if (!id.Contains(":"))
            {
                Index = tree.GetVarIndex(id);
            }
        }
    }
}

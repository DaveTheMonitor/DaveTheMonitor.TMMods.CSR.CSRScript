namespace DaveTheMonitor.TMMods.CSR.CSRScript.Generators.Compiler
{
    internal abstract class ScriptTreeNode
    {
        public ScriptTreeNodeType Type { get; protected set; }
        public long Char { get; internal set; }

        public ScriptTreeNode(ScriptTreeNodeType type, long cha)
        {
            Type = type;
            Char = cha;
        }
    }
}

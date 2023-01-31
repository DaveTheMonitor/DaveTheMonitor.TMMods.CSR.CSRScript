using System.Collections.Generic;

namespace DaveTheMonitor.TMMods.CSR.CSRScript.Generators.Compiler
{
    internal sealed class InVarScriptTreeNode : ScriptTreeNode
    {
        public List<IDScriptTreeNode> IDs { get; private set; }

        public InVarScriptTreeNode(long cha) : base(ScriptTreeNodeType.InVar, cha)
        {
            IDs = new List<IDScriptTreeNode>();
        }
    }
}

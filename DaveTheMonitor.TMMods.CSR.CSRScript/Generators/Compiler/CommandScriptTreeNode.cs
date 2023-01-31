using System.Collections.Generic;

namespace DaveTheMonitor.TMMods.CSR.CSRScript.Generators.Compiler
{
    internal sealed class CommandScriptTreeNode : ScriptTreeNode
    {
        public string Value { get; private set; }
        public List<ScriptTreeNode> Args { get; private set; }

        public CommandScriptTreeNode(long cha, string value) : base(ScriptTreeNodeType.Command, cha)
        {
            Value = value;
            Args = new List<ScriptTreeNode>();
        }
    }
}

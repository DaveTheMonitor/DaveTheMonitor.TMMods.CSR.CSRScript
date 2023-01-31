using System.Collections.Generic;

namespace DaveTheMonitor.TMMods.CSR.CSRScript.Generators.Compiler
{
    internal sealed class InvokeScriptTreeNode : ScriptTreeNode
    {
        public string Value { get; private set; }
        public List<ScriptTreeNode> Args { get; set; }
        public bool Ret { get; private set; }
        public bool Static { get; private set; }

        public InvokeScriptTreeNode(long cha, string value, bool ret, bool isStatic) : base(ScriptTreeNodeType.Invoke, cha)
        {
            Value = value;
            Args = new List<ScriptTreeNode>();
            Ret = ret;
            Static = isStatic;
        }
    }
}

using System.Collections.Generic;

namespace DaveTheMonitor.TMMods.CSR.CSRScript.Generators.Compiler
{
    internal sealed class ScriptScriptTreeNode : ScriptTreeNode
    {
        public List<ScriptTreeNode> Statements { get; private set; }
        private List<string> _varIndexes;

        public int GetTempVarIndex()
        {
            return GetVarIndex("#temp", true);
        }

        public int GetVarIndex(string id)
        {
            int index = GetVarIndex(id, false);
            return index;
        }

        private int GetVarIndex(string id, bool temp)
        {
            int index = _varIndexes.IndexOf(id);
            if (index != -1)
            {
                return index;
            }
            else
            {
                index = _varIndexes.IndexOf(null);
                if (index != -1)
                {
                    if (!temp)
                    {
                        _varIndexes[index] = id;
                    }
                    return index;
                }
                else
                {
                    if (!temp)
                    {
                        _varIndexes.Add(id);
                        return _varIndexes.Count - 1;
                    }
                    else
                    {
                        return _varIndexes.Count;
                    }
                }
            }
        }

        public void RemoveVar(string id)
        {
            int index = _varIndexes.IndexOf(id);
            if (index != -1)
            {
                _varIndexes[index] = null;
            }
        }

        public ScriptScriptTreeNode(long cha) : base(ScriptTreeNodeType.Script, cha)
        {
            Statements = new List<ScriptTreeNode>();
            _varIndexes = new List<string>();
        }
    }
}

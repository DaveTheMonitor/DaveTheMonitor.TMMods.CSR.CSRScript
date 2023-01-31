using System;
using System.Collections.Generic;

namespace DaveTheMonitor.TMMods.CSR.CSRScript.Generators.Compiler
{
    internal sealed class ScriptCompilerGenerator
    {
        public Action<string, string> ErrorHandler;
        private ScriptScriptTreeNode _tree;
        private ScriptGenerator _generator;
        private string _raw;
        private int _switchContext;
        private bool _staticIDOverride;
        private Dictionary<string, int> _inVars;
        private string _staticID;
        internal bool _errored;

        public Script Generate(string name, ScriptScriptTreeNode tree, string raw)
        {
            _errored = false;
            _tree = tree;
            _raw = raw;
            _generator = new ScriptGenerator(name);
            _switchContext = -1;
            _inVars.Clear();
            CompileNode(tree);
            _generator.FinalizeScript(_inVars, true);
            return _generator.Script;
        }

        public void CompileNode(ScriptTreeNode node)
        {
            if (_errored)
            {
                return;
            }
            if (_switchContext != -1)
            {
                _generator.InsertIntOp(ScriptOpCode.LoadVar, _switchContext);
                _generator.InsertOp(ScriptOpCode.Context);
                _switchContext = -1;
            }
            switch (node.Type)
            {
                case ScriptTreeNodeType.InVar:
                {
                    InVarScriptTreeNode n = (InVarScriptTreeNode)node;
                    foreach (IDScriptTreeNode id in n.IDs)
                    {
                        _inVars.Add(id.ID, id.Index);
                    }
                    break;
                }
                case ScriptTreeNodeType.Script:
                {
                    ScriptScriptTreeNode n = (ScriptScriptTreeNode)node;
                    foreach (ScriptTreeNode statement in n.Statements)
                    {
                        CompileNode(statement);
                    }
                    break;
                }
                case ScriptTreeNodeType.FloatLiteral:
                {
                    FloatLiteralScriptTreeNode n = (FloatLiteralScriptTreeNode)node;
                    _generator.InsertFloatLiteral(n.Value);
                    break;
                }
                case ScriptTreeNodeType.StringLiteral:
                {
                    StringLiteralScriptTreeNode n = (StringLiteralScriptTreeNode)node;
                    _generator.InsertStringLiteral(n.Value);
                    break;
                }
                case ScriptTreeNodeType.BoolLiteral:
                {
                    BoolLiteralScriptTreeNode n = (BoolLiteralScriptTreeNode)node;
                    _generator.InsertBoolLiteral(n.Value);
                    break;
                }
                case ScriptTreeNodeType.Null:
                {
                    _generator.InsertOp(ScriptOpCode.Null);
                    break;
                }
                case ScriptTreeNodeType.StaticID:
                {
                    IDScriptTreeNode n = (IDScriptTreeNode)node;
                    if (n.Type == ScriptTreeNodeType.StaticID)
                    {
                        if (!n.ID.Contains(":"))
                        {
                            if (_staticIDOverride)
                            {
                                _staticID = n.ID;
                            }
                            else
                            {
                                _generator.InsertStringLiteral(n.ID);
                            }
                            break;
                        }
                        string[] args = n.ID.Split(':');
                        if (_switchContext == -1)
                        {
                            _generator.InsertOp(ScriptOpCode.PushContext);
                            int index = _tree.GetTempVarIndex();
                            _switchContext = index;
                            _generator.InsertIntOp(ScriptOpCode.StoreVar, index);
                        }
                        _generator.InsertIntOp(ScriptOpCode.LoadVar, _tree.GetVarIndex(args[0]));
                        _generator.InsertOp(ScriptOpCode.Context);
                        for (int i = 1; i < args.Length; i++)
                        {
                            string name = args[i];
                            if (_staticIDOverride && i == args.Length - 1)
                            {
                                _staticID = name;
                            }
                            else
                            {
                                _generator.InsertOp(ScriptOpCode.GetProperty, name);
                            }
                            if (i != args.Length - 1)
                            {
                                _generator.InsertOp(ScriptOpCode.Context);
                            }
                        }
                    }
                    break;
                }
                case ScriptTreeNodeType.ID:
                {
                    IDScriptTreeNode n = (IDScriptTreeNode)node;
                    if (n.Store)
                    {
                        _generator.InsertIntOp(ScriptOpCode.StoreVar, n.Index);
                    }
                    else
                    {
                        _generator.InsertIntOp(ScriptOpCode.LoadVar, n.Index);
                    }
                    break;
                }
                case ScriptTreeNodeType.BinaryExpression:
                {
                    BinExprScriptTreeNode n = (BinExprScriptTreeNode)node;
                    if (n.Left != null)
                    {
                        CompileNode(n.Left);
                    }
                    CompileNode(n.Right);
                    switch (n.Op)
                    {
                        case "==": _generator.InsertOp(ScriptOpCode.Equal); break;
                        case "!=": _generator.InsertOp(ScriptOpCode.NotEqual); break;
                        case ">": _generator.InsertOp(ScriptOpCode.Gt); break;
                        case ">=": _generator.InsertOp(ScriptOpCode.Gte); break;
                        case "<": _generator.InsertOp(ScriptOpCode.Lt); break;
                        case "<=": _generator.InsertOp(ScriptOpCode.Lte); break;
                        case "+": _generator.InsertOp(ScriptOpCode.Add); break;
                        case "-": _generator.InsertOp(ScriptOpCode.Sub); break;
                        case "*": _generator.InsertOp(ScriptOpCode.Multi); break;
                        case "/": _generator.InsertOp(ScriptOpCode.Div); break;
                        case "%": _generator.InsertOp(ScriptOpCode.Mod); break;
                        case "&&":
                        case "and": _generator.InsertOp(ScriptOpCode.And); break;
                        case "||":
                        case "or": _generator.InsertOp(ScriptOpCode.Or); break;
                        case "!":
                        case "not": _generator.InsertOp(ScriptOpCode.Invert); break;
                    }
                    break;
                }
                case ScriptTreeNodeType.Invoke:
                {
                    InvokeScriptTreeNode invoke = (InvokeScriptTreeNode)node;
                    int startIndex = 0;
                    IDScriptTreeNode retId = null;
                    if (invoke.Static)
                    {
                        startIndex++;
                    }
                    if (invoke.Ret)
                    {
                        retId = (IDScriptTreeNode)invoke.Args[startIndex];
                        startIndex++;
                    }
                    int total = invoke.Args.Count - startIndex - 1;
                    string name = null;
                    for (int i = invoke.Args.Count - 1; i >= 0; i--)
                    {
                        int first = 0;
                        if (invoke.Ret)
                        {
                            first++;
                        }
                        if (invoke.Static)
                        {
                            first++;
                        }
                        if (i == first)
                        {
                            _staticIDOverride = true;
                            CompileNode(invoke.Args[i]);
                            name = _staticID;
                            _staticIDOverride = false;
                        }
                        else if (!invoke.Ret || i > first)
                        {
                            CompileNode(invoke.Args[i]);
                        }
                    }

                    _generator.InsertInvoke(invoke.Static, invoke.Ret, name, total);
                    if (invoke.Ret)
                    {
                        _generator.InsertIntOp(ScriptOpCode.StoreVar, retId.Index);
                    }
                    break;
                }
                case ScriptTreeNodeType.GetProperty:
                {
                    GetPropertyScriptTreeNode n = (GetPropertyScriptTreeNode)node;
                    int index = n.Var.Index;
                    _staticIDOverride = true;
                    CompileNode(n.Property);
                    _staticIDOverride = false;
                    _generator.InsertStringOp(ScriptOpCode.GetProperty, _staticID);
                    _generator.InsertIntOp(ScriptOpCode.StoreVar, index);
                    break;
                }
                case ScriptTreeNodeType.Command:
                {
                    CommandScriptTreeNode n = (CommandScriptTreeNode)node;
                    switch (n.Value)
                    {
                        case "exit":
                        {
                            _generator.InsertOp(ScriptOpCode.Exit);
                            break;
                        }
                        case "delete":
                        {
                            foreach (ScriptTreeNode argBase in n.Args)
                            {
                                IDScriptTreeNode arg = (IDScriptTreeNode)argBase;
                                _generator.InsertIntOp(ScriptOpCode.Delete, arg.Index);
                            }
                            break;
                        }
                        case "print":
                        {
                            CompileNode(n.Args[0]);
                            _generator.InsertOp(ScriptOpCode.Print);
                            break;
                        }
                        case "context":
                        {
                            CompileNode(n.Args[0]);
                            _generator.InsertOp(ScriptOpCode.Context);
                            break;
                        }
                    }
                    break;
                }
                case ScriptTreeNodeType.Var:
                {
                    VarScriptTreeNode n = (VarScriptTreeNode)node;
                    CompileNode(n.Value);
                    _generator.InsertIntOp(ScriptOpCode.StoreVar, n.ID.Index);
                    break;
                }
                case ScriptTreeNodeType.IfStatement:
                {
                    IfScriptTreeNode n = (IfScriptTreeNode)node;
                    CompileNode(n.Condition);
                    int jumpNodePosition = _generator.Position;
                    _generator.InsertIntOp(ScriptOpCode.JF, 0);
                    CompileNode(n.Statement);
                    if (n.ElseStatement != null)
                    {
                        int endJumpNodePosition = _generator.Position;
                        _generator.InsertIntOp(ScriptOpCode.Jump, 0);
                        _generator.Ops[jumpNodePosition] = new ScriptOpInt(ScriptOpCode.JF, _generator.Position);
                        CompileNode(n.ElseStatement);
                        _generator.Ops[endJumpNodePosition] = new ScriptOpInt(ScriptOpCode.Jump, _generator.Position);
                    }
                    else
                    {
                        _generator.Ops[jumpNodePosition] = new ScriptOpInt(ScriptOpCode.JF, _generator.Position);
                    }
                    break;
                }
                case ScriptTreeNodeType.WhileStatement:
                {
                    WhileScriptTreeNode n = (WhileScriptTreeNode)node;
                    int start = _generator.Position;
                    CompileNode(n.Condition);
                    int jumpNodePosition = _generator.Position;
                    _generator.InsertIntOp(ScriptOpCode.JF, 0);
                    CompileNode(n.Statement);
                    _generator.InsertIntOp(ScriptOpCode.Jump, start);
                    _generator.Ops[jumpNodePosition] = new ScriptOpInt(ScriptOpCode.JF, _generator.Position);
                    break;
                }
            }
        }

        private void Error(string header, string message, long position)
        {
            _errored = true;
            GetLineAndChar(position, out int line, out long cha);
            ErrorHandler(header, message + " at line " + line + ", " + cha);
        }

        private void GetLineAndChar(long position, out int line, out long cha)
        {
            int l = 1;
            int lStart = 0;
            for (int i = 0; i < _raw.Length; i++)
            {
                if (i > position)
                {
                    break;
                }
                else if (_raw[i] == '\n')
                {
                    lStart = i;
                    l++;
                }
            }
            long c = position - lStart;
            line = l;
            cha = c;
        }

        public ScriptCompilerGenerator()
        {
            _inVars = new Dictionary<string, int>();
        }
    }
}

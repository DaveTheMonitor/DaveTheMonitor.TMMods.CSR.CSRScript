using DaveTheMonitor.CSRSandbox;
using DaveTheMonitor.TMMods.CSR.CSRScript.Generators;
using System;
using System.Collections.Generic;

namespace DaveTheMonitor.TMMods.CSR.CSRScript
{
    internal sealed class SandboxScriptRuntime : IScriptRuntime
    {
        public Script CurrentScript { get; private set; }
        private int _position;
        private Stack<ScriptVar> _stack;
        private ScriptVar[] _locals;
        private int _stackSize;
        private int _localSize;
        internal IScriptContext _context;
        private List<ScriptVar> _invokeCache;
        private HashSet<int> _localIndexes;
        private MainWindow _window;
        private bool _errored;

        public void Error(string header, string message)
        {
            _window.LogError(header, message);
            _errored = true;
        }

        public void InitVar(string name, ScriptVar value)
        {
            if (CurrentScript.InVars.TryGetValue(name, out int index))
            {
                SetVar(index, value);
            }
        }

        public void Run(Script script, Action<IScriptRuntime> init)
        {
            _errored = false;
            CurrentScript = script;
            init?.Invoke(this);
            while (_position < CurrentScript.Ops.Length)
            {
                if (_errored)
                {
                    Exit();
                    break;
                }
                ScriptOp op = CurrentScript.Ops[_position];
                switch (op.Op)
                {
                    case ScriptOpCode.Nop: break;
                    case ScriptOpCode.Exit: Exit(); return;
                    case ScriptOpCode.Pop: Pop(); break;
                    case ScriptOpCode.FloatLiteral:
                    {
                        float value = op.GetArgFloat(0);
                        Push(new ScriptVar(value));
                        break;
                    }
                    case ScriptOpCode.StringLiteral:
                    {
                        string value = op.GetArgString(0);
                        Push(new ScriptVar(value));
                        break;
                    }
                    case ScriptOpCode.TrueLiteral: Push(ScriptVar.True); break;
                    case ScriptOpCode.FalseLiteral: Push(ScriptVar.False); break;
                    case ScriptOpCode.Null: Push(ScriptVar.Null); break;
                    case ScriptOpCode.LoadVar:
                    {
                        int index = op.GetArgInt(0);
                        Push(GetVar(index));
                        break;
                    }
                    case ScriptOpCode.StoreVar:
                    {
                        int index = op.GetArgInt(0);
                        SetVar(index, Pop());
                        break;
                    }
                    case ScriptOpCode.Delete:
                    {
                        int index = op.GetArgInt(0);
                        DeleteVar(index);
                        break;
                    }
                    case ScriptOpCode.Print:
                    {
                        _window.Log(Pop().ToString());
                        break;
                    }
                    case ScriptOpCode.Add:
                    {
                        ScriptVar right = Pop();
                        ScriptVar left = Pop();
                        Push(ScriptVar.Add(left, right, this));
                        break;
                    }
                    case ScriptOpCode.Sub:
                    {
                        ScriptVar right = Pop();
                        ScriptVar left = Pop();
                        Push(ScriptVar.Sub(left, right, this));
                        break;
                    }
                    case ScriptOpCode.Multi:
                    {
                        ScriptVar right = Pop();
                        ScriptVar left = Pop();
                        Push(ScriptVar.Multi(left, right, this));
                        break;
                    }
                    case ScriptOpCode.Div:
                    {
                        ScriptVar right = Pop();
                        ScriptVar left = Pop();
                        Push(ScriptVar.Div(left, right, this));
                        break;
                    }
                    case ScriptOpCode.Mod:
                    {
                        ScriptVar right = Pop();
                        ScriptVar left = Pop();
                        Push(ScriptVar.Mod(left, right, this));
                        break;
                    }
                    case ScriptOpCode.Equal:
                    {
                        ScriptVar right = Pop();
                        ScriptVar left = Pop();
                        Push(new ScriptVar(left.Equals(right)));
                        break;
                    }
                    case ScriptOpCode.NotEqual:
                    {
                        ScriptVar right = Pop();
                        ScriptVar left = Pop();
                        Push(new ScriptVar(!left.Equals(right)));
                        break;
                    }
                    case ScriptOpCode.And:
                    {
                        ScriptVar right = Pop();
                        ScriptVar left = Pop();
                        Push(new ScriptVar(left.GetBoolValue(this) && right.GetBoolValue(this)));
                        break;
                    }
                    case ScriptOpCode.Or:
                    {
                        ScriptVar right = Pop();
                        ScriptVar left = Pop();
                        Push(new ScriptVar(left.GetBoolValue(this) || right.GetBoolValue(this)));
                        break;
                    }
                    case ScriptOpCode.Invert:
                    {
                        ScriptVar value = Pop();
                        Push(new ScriptVar(!value.GetBoolValue(this)));
                        break;
                    }
                    case ScriptOpCode.Gt:
                    {
                        ScriptVar right = Pop();
                        ScriptVar left = Pop();
                        Push(new ScriptVar(left.CompareTo(right, this) > 0));
                        break;
                    }
                    case ScriptOpCode.Gte:
                    {
                        ScriptVar right = Pop();
                        ScriptVar left = Pop();
                        Push(new ScriptVar(left.CompareTo(right, this) >= 0));
                        break;
                    }
                    case ScriptOpCode.Lt:
                    {
                        ScriptVar right = Pop();
                        ScriptVar left = Pop();
                        Push(new ScriptVar(left.CompareTo(right, this) < 0));
                        break;
                    }
                    case ScriptOpCode.Lte:
                    {
                        ScriptVar right = Pop();
                        ScriptVar left = Pop();
                        Push(new ScriptVar(left.CompareTo(right, this) <= 0));
                        break;
                    }
                    case ScriptOpCode.Jump:
                    {
                        int position = op.GetArgInt(0);
                        _position = position - 1;
                        break;
                    }
                    case ScriptOpCode.JT:
                    {
                        if (Pop().GetBoolValue(this))
                        {
                            int position = op.GetArgInt(0);
                            _position = position - 1;
                        }
                        break;
                    }
                    case ScriptOpCode.JF:
                    {
                        if (!Pop().GetBoolValue(this))
                        {
                            int position = op.GetArgInt(0);
                            _position = position - 1;
                        }
                        break;
                    }
                    case ScriptOpCode.JE:
                    {
                        ScriptVar right = Pop();
                        ScriptVar left = Pop();
                        if (left.Equals(right))
                        {
                            int position = op.GetArgInt(0);
                            _position = position - 1;
                        }
                        break;
                    }
                    case ScriptOpCode.JNe:
                    {
                        ScriptVar right = Pop();
                        ScriptVar left = Pop();
                        if (!left.Equals(right))
                        {
                            int position = op.GetArgInt(0);
                            _position = position - 1;
                        }
                        break;
                    }
                    case ScriptOpCode.JGt:
                    {
                        ScriptVar right = Pop();
                        ScriptVar left = Pop();
                        if (left.CompareTo(right) > 0)
                        {
                            int position = op.GetArgInt(0);
                            _position = position - 1;
                        }
                        break;
                    }
                    case ScriptOpCode.JGte:
                    {
                        ScriptVar right = Pop();
                        ScriptVar left = Pop();
                        if (left.CompareTo(right) >= 0)
                        {
                            int position = op.GetArgInt(0);
                            _position = position - 1;
                        }
                        break;
                    }
                    case ScriptOpCode.JLt:
                    {
                        ScriptVar right = Pop();
                        ScriptVar left = Pop();
                        if (left.CompareTo(right) < 0)
                        {
                            int position = op.GetArgInt(0);
                            _position = position - 1;
                        }
                        break;
                    }
                    case ScriptOpCode.JLte:
                    {
                        ScriptVar right = Pop();
                        ScriptVar left = Pop();
                        if (left.CompareTo(right) <= 0)
                        {
                            int position = op.GetArgInt(0);
                            _position = position - 1;
                        }
                        break;
                    }
                    case ScriptOpCode.Context:
                    {
                        ScriptVar value = Pop();
                        if (value.IsNull)
                        {
                            _context = null;
                        }
                        else
                        {
                            _context = value.GetContextValue(this);
                        }
                        break;
                    }
                    case ScriptOpCode.PushContext:
                    {
                        Push(new ScriptVar(_context));
                        break;
                    }
                    case ScriptOpCode.Invoke:
                    case ScriptOpCode.InvokeRet:
                    case ScriptOpCode.InvokeStatic:
                    case ScriptOpCode.InvokeStaticRet:
                    {
                        bool isStatic = op.Op == ScriptOpCode.InvokeStatic || op.Op == ScriptOpCode.InvokeStaticRet;
                        if (!isStatic && _context == null)
                        {
                            Error("Null Context", "Context is null in invoke.");
                            break;
                        }
                        string name = op.GetArgString(0);
                        int args = op.GetArgInt(1);
                        _invokeCache.Clear();
                        for (int i = 0; i < args; i++)
                        {
                            _invokeCache.Add(_stack.Pop());
                        }
                        switch (op.Op)
                        {
                            case ScriptOpCode.Invoke:
                            {
                                _ = Invoke(name, _invokeCache);
                                break;
                            }
                            case ScriptOpCode.InvokeRet:
                            {
                                Push(Invoke(name, _invokeCache));
                                break;
                            }
                            case ScriptOpCode.InvokeStatic:
                            {
                                _ = InvokeStatic(name, _invokeCache);
                                break;
                            }
                            case ScriptOpCode.InvokeStaticRet:
                            {
                                Push(InvokeStatic(name, _invokeCache));
                                break;
                            }
                        }
                        break;
                    }
                    case ScriptOpCode.GetProperty:
                    {
                        if (_context == null)
                        {
                            Error("Null Context", "Context is null in property access.");
                            break;
                        }
                        string name = op.GetArgString(0);
                        Push(_context.GetProperty(this, name));
                        break;
                    }
                    default:
                    {
                        Error("Undefined Operation", "Undefined Operation");
                        break;
                    }
                }
                _position++;
            }
            Exit();
        }

        private void Exit()
        {
            _position = 0;
            CurrentScript = null;
            _stack.Clear();
            foreach (int index in _localIndexes)
            {
                _locals[index] = ScriptVar.Null;
            }
            _localIndexes.Clear();
        }


        private void Push(ScriptVar value)
        {
            if (_stack.Count >= _stackSize)
            {
                Error("Stack Overflow", "Maximum stack size exceeded.");
                return;
            }
            _stack.Push(value);
        }

        private ScriptVar Pop()
        {
            return _stack.Pop();
        }

        private ScriptVar GetVar(int index)
        {
            return _locals[index];
        }

        private void SetVar(int index, ScriptVar value)
        {
            if (index > _localSize)
            {
                Error("Maximum Local Size Exceeded", "Exceeded maximum local variable size. Delete unneeded variables with the delete command.");
                return;
            }
            _locals[index] = value;
            _localIndexes.Add(index);
        }

        private void DeleteVar(int index)
        {
            if (!_locals[index].IsNull)
            {
                _locals[index] = ScriptVar.Null;
            }
            _localIndexes.Remove(index);
        }

        public ScriptVar Invoke(string name, IList<ScriptVar> args)
        {
            return _context.Invoke(this, name, args);
        }

        public ScriptVar InvokeStatic(string name, IList<ScriptVar> args)
        {
            switch (name)
            {
                case "createarray":
                {
                    return new ScriptVar(new List<ScriptVar>().CreateScriptArray(false));
                }
                case "getlength":
                {
                    ScriptVar arg = args[0];
                    if (arg.IsString)
                    {
                        return new ScriptVar(arg.GetStringValue(this).Length);
                    }
                    else if (arg.IsContext && arg.GetContextValue(this) is IScriptArray arr)
                    {
                        return new ScriptVar(arr.Count);
                    }
                    break;
                }
            }
            return ScriptVar.Null;
        }

        public SandboxScriptRuntime(int stackSize, int localSize, MainWindow window)
        {
            _stack = new Stack<ScriptVar>(stackSize);
            _locals = new ScriptVar[localSize];
            _stackSize = stackSize;
            _localSize = localSize;
            _invokeCache = new List<ScriptVar>();
            _localIndexes = new HashSet<int>();
            _window = window;
        }
    }
}

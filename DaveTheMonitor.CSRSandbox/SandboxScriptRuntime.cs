using DaveTheMonitor.CSRSandbox;
using DaveTheMonitor.TMMods.CSR.CSRScript.Generators;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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
        private IScriptContext _context;
        private List<ScriptVar> _invokeCache;
        private MainWindow _window;
        private bool _errored;
        private Stopwatch _stopwatch;

        public void Error(ScriptError error)
        {
            _window.LogError(error.Header, error.Message);
            _errored = true;
        }

        public void InitVar(string name, ScriptVar value)
        {
            if (CurrentScript.InVars.TryGetValue(name, out int index))
            {
                SetVar(index, value);
            }
        }

        public void Run(Script script, Action<IScriptRuntime> init, int timeout)
        {
            _window.Log("Script executing...");
            _stopwatch.Restart();
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
                else if (timeout > 0 && _stopwatch.ElapsedMilliseconds > timeout)
                {
                    Error(new ScriptError("Script Timeout", $"Script execution timed out ({timeout} ms)"));
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
                            Error(new ScriptError("Null Context", "Context is null in invoke."));
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
                            Error(new ScriptError("Null Context", "Context is null in property access."));
                            break;
                        }
                        string name = op.GetArgString(0);
                        Push(_context.GetProperty(this, name));
                        break;
                    }
                    default:
                    {
                        Error(ScriptError.UndefinedOperation());
                        break;
                    }
                }
                _position++;
            }
            _stopwatch.Stop();
            _window.Log("Script execution finished in " + _stopwatch.ElapsedMilliseconds + " ms");
            Exit();
        }

        private void Exit()
        {
            _position = 0;
            CurrentScript = null;
            _stack.Clear();
        }


        private void Push(ScriptVar value)
        {
            if (_stack.Count >= _stackSize)
            {
                Error(ScriptError.StackOverflowError(_stackSize));
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
                Error(ScriptError.MaxLocalsError(_localSize));
                return;
            }
            _locals[index] = value;
        }

        private void DeleteVar(int index)
        {
            if (!_locals[index].IsNull)
            {
                _locals[index] = ScriptVar.Null;
            }
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
                    if (args.Count == 0)
                    {
                        return new ScriptVar(new List<ScriptVar>().CreateScriptArray(false));
                    }
                    else if (args.Count == 1)
                    {
                        return new ScriptVar(new List<ScriptVar>(GetInt(args[0])).CreateScriptArray(false));
                    }
                    else
                    {
                        InvalidArgCountError(1, args.Count, name);
                    }
                    break;
                }
                case "getlength":
                {
                    if (args.Count == 1)
                    {
                        ScriptVar arg = args[0];
                        if (arg.IsString)
                        {
                            return new ScriptVar(arg.GetStringValue(this).Length);
                        }
                        else if (arg.IsContext && GetContext(arg) is IScriptArray arr)
                        {
                            return new ScriptVar(arr.Count);
                        }
                    }
                    else
                    {
                        InvalidArgCountError(1, args.Count, name);
                    }
                    break;
                }
                case "random":
                {
                    if (args.Count == 0)
                    {
                        return new ScriptVar(new ScriptRandom(new Random().Next()));
                    }
                    else if (args.Count == 1)
                    {
                        return new ScriptVar(new ScriptRandom(GetInt(args[0])));
                    }
                    else
                    {
                        InvalidArgCountError(1, args.Count, name);
                    }
                    break;
                }
                case "abs":
                {
                    if (args.Count == 1)
                    {
                        return new ScriptVar((float)Math.Abs(GetFloat(args[0])));
                    }
                    else
                    {
                        InvalidArgCountError(args.Count, args.Count, name);
                    }
                    break;
                }
                case "acos":
                {
                    if (args.Count == 1)
                    {
                        return new ScriptVar((float)Math.Acos(GetFloat(args[0])));
                    }
                    else
                    {
                        InvalidArgCountError(args.Count, args.Count, name);
                    }
                    break;
                }
                case "acosh":
                {
                    if (args.Count == 1)
                    {
                        return new ScriptVar((float)Math.Acosh(GetFloat(args[0])));
                    }
                    else
                    {
                        InvalidArgCountError(args.Count, args.Count, name);
                    }
                    break;
                }
                case "asin":
                {
                    if (args.Count == 1)
                    {
                        return new ScriptVar((float)Math.Asin(GetFloat(args[0])));
                    }
                    else
                    {
                        InvalidArgCountError(args.Count, args.Count, name);
                    }
                    break;
                }
                case "asinh":
                {
                    if (args.Count == 1)
                    {
                        return new ScriptVar((float)Math.Asinh(GetFloat(args[0])));
                    }
                    else
                    {
                        InvalidArgCountError(args.Count, args.Count, name);
                    }
                    break;
                }
                case "atan":
                {
                    if (args.Count == 1)
                    {
                        return new ScriptVar((float)Math.Atan(GetFloat(args[0])));
                    }
                    else
                    {
                        InvalidArgCountError(args.Count, args.Count, name);
                    }
                    break;
                }
                case "atan2":
                {
                    if (args.Count == 2)
                    {
                        return new ScriptVar((float)Math.Atan2(GetFloat(args[0]), GetFloat(args[1])));
                    }
                    else
                    {
                        InvalidArgCountError(args.Count, args.Count, name);
                    }
                    break;
                }
                case "atanh":
                {
                    if (args.Count == 1)
                    {
                        return new ScriptVar((float)Math.Atanh(GetFloat(args[0])));
                    }
                    else
                    {
                        InvalidArgCountError(args.Count, args.Count, name);
                    }
                    break;
                }
                case "ceil":
                {
                    if (args.Count == 1)
                    {
                        return new ScriptVar((float)Math.Ceiling(GetFloat(args[0])));
                    }
                    else
                    {
                        InvalidArgCountError(args.Count, args.Count, name);
                    }
                    break;
                }
                case "clamp":
                {
                    if (args.Count == 3)
                    {
                        return new ScriptVar((float)Math.Clamp(GetFloat(args[0]), GetFloat(args[1]), GetFloat(args[2])));
                    }
                    else
                    {
                        InvalidArgCountError(args.Count, args.Count, name);
                    }
                    break;
                }
                case "cos":
                {
                    if (args.Count == 1)
                    {
                        return new ScriptVar((float)Math.Cos(GetFloat(args[0])));
                    }
                    else
                    {
                        InvalidArgCountError(args.Count, args.Count, name);
                    }
                    break;
                }
                case "cosh":
                {
                    if (args.Count == 1)
                    {
                        return new ScriptVar((float)Math.Cosh(GetFloat(args[0])));
                    }
                    else
                    {
                        InvalidArgCountError(args.Count, args.Count, name);
                    }
                    break;
                }
                case "deg":
                {
                    if (args.Count == 1)
                    {
                        return new ScriptVar((float)(args[0].GetFloatValue(this) * (180 / Math.PI)));
                    }
                    else
                    {
                        InvalidArgCountError(1, args.Count, name);
                    }
                    break;
                }
                case "exp":
                {
                    if (args.Count == 1)
                    {
                        return new ScriptVar((float)Math.Exp(GetFloat(args[0])));
                    }
                    else
                    {
                        InvalidArgCountError(args.Count, args.Count, name);
                    }
                    break;
                }
                case "floor":
                {
                    if (args.Count == 1)
                    {
                        return new ScriptVar((float)Math.Floor(GetFloat(args[0])));
                    }
                    else
                    {
                        InvalidArgCountError(args.Count, args.Count, name);
                    }
                    break;
                }
                case "log":
                {
                    if (args.Count == 1)
                    {
                        return new ScriptVar((float)Math.Log(GetFloat(args[0])));
                    }
                    else if (args.Count == 2)
                    {
                        return new ScriptVar((float)Math.Log(GetFloat(args[0]), GetFloat(args[1])));
                    }
                    else
                    {
                        InvalidArgCountError(args.Count, args.Count, name);
                    }
                    break;
                }
                case "log10":
                {
                    if (args.Count == 1)
                    {
                        return new ScriptVar((float)Math.Log10(GetFloat(args[0])));
                    }
                    else
                    {
                        InvalidArgCountError(args.Count, args.Count, name);
                    }
                    break;
                }
                case "max":
                {
                    if (args.Count == 2)
                    {
                        return new ScriptVar((float)Math.Max(GetFloat(args[0]), GetFloat(args[1])));
                    }
                    else
                    {
                        InvalidArgCountError(2, args.Count, name);
                    }
                    break;
                }
                case "min":
                {
                    if (args.Count == 2)
                    {
                        return new ScriptVar((float)Math.Min(GetFloat(args[0]), GetFloat(args[1])));
                    }
                    else
                    {
                        InvalidArgCountError(2, args.Count, name);
                    }
                    break;
                }
                case "pow":
                {
                    if (args.Count == 2)
                    {
                        return new ScriptVar((float)Math.Pow(GetFloat(args[0]), GetFloat(args[1])));
                    }
                    else
                    {
                        InvalidArgCountError(2, args.Count, name);
                    }
                    break;
                }
                case "rad":
                {
                    if (args.Count == 1)
                    {
                        return new ScriptVar((float)(args[0].GetFloatValue(this) * (Math.PI / 180)));
                    }
                    else
                    {
                        InvalidArgCountError(1, args.Count, name);
                    }
                    break;
                }
                case "round":
                {
                    if (args.Count == 1)
                    {
                        return new ScriptVar((float)Math.Round(GetFloat(args[0])));
                    }
                    else
                    {
                        InvalidArgCountError(args.Count, args.Count, name);
                    }
                    break;
                }
                case "sin":
                {
                    if (args.Count == 1)
                    {
                        return new ScriptVar((float)Math.Sin(GetFloat(args[0])));
                    }
                    else
                    {
                        InvalidArgCountError(args.Count, args.Count, name);
                    }
                    break;
                }
                case "sqrt":
                {
                    if (args.Count == 1)
                    {
                        return new ScriptVar((float)Math.Sqrt(GetFloat(args[0])));
                    }
                    else
                    {
                        InvalidArgCountError(args.Count, args.Count, name);
                    }
                    break;
                }
                case "tan":
                {
                    if (args.Count == 1)
                    {
                        return new ScriptVar((float)Math.Tan(GetFloat(args[0])));
                    }
                    else
                    {
                        InvalidArgCountError(args.Count, args.Count, name);
                    }
                    break;
                }
                case "tanh":
                {
                    if (args.Count == 1)
                    {
                        return new ScriptVar((float)Math.Tanh(GetFloat(args[0])));
                    }
                    else
                    {
                        InvalidArgCountError(args.Count, args.Count, name);
                    }
                    break;
                }
            }
            return ScriptVar.Null;
        }

        private float GetFloat(ScriptVar var)
        {
            return var.GetFloatValue(this);
        }

        private int GetInt(ScriptVar var)
        {
            return (int)var.GetFloatValue(this);
        }

        private string GetString(ScriptVar var)
        {
            return var.GetStringValue(this);
        }

        private bool GetBool(ScriptVar var)
        {
            return var.GetBoolValue(this);
        }

        private IScriptContext GetContext(ScriptVar var)
        {
            return var.GetContextValue(this);
        }

        private void InvalidArgCountError(int expected, int received, string method)
        {
            Error(ScriptError.InvalidArgCount(expected, received, GetType(), method));
        }

        public SandboxScriptRuntime(int stackSize, int localSize, MainWindow window)
        {
            _stack = new Stack<ScriptVar>(stackSize);
            _locals = new ScriptVar[localSize];
            _stackSize = stackSize;
            _localSize = localSize;
            _invokeCache = new List<ScriptVar>();
            _stopwatch = new Stopwatch();
            _window = window;
        }
    }
}

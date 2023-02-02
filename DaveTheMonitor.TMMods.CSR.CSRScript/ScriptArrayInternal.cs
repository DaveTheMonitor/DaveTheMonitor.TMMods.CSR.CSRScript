using System;
using System.Collections.Generic;

namespace DaveTheMonitor.TMMods.CSR.CSRScript
{
    internal abstract class ScriptArrayInternal<T> : IScriptArray
    {
        public bool ReadOnly { get; private set; }
        public int Count => _list.Count;
        protected internal IList<T> _list;

        public string GetName()
        {
            return $"Array<{typeof(T).Name}>";
        }

        public ScriptVar GetProperty(IScriptRuntime runtime, string name)
        {
            switch (name)
            {
                case "length": return new ScriptVar(Count);
                case "readonly": return new ScriptVar(ReadOnly);
            }
            runtime.Error(ScriptError.InvalidPropertyError(GetType(), name));
            return ScriptVar.Null;
        }

        public ScriptVar Invoke(IScriptRuntime runtime, string name, IList<ScriptVar> args)
        {
            switch (name)
            {
                case "getitem":
                {
                    ScriptVar arg = args[0];
                    int index = (int)Math.Floor(arg.GetFloatValue(runtime));
                    return GetItem(index, runtime);
                }
                case "setitem":
                {
                    int index = (int)Math.Floor(args[0].GetFloatValue(runtime));
                    SetItem(index, args[1], runtime);
                    break;
                }
                case "insert":
                {
                    int index = (int)Math.Floor(args[0].GetFloatValue(runtime));
                    Insert(index, args[1], runtime);
                    break;
                }
                case "clear":
                {
                    Clear(runtime);
                    break;
                }
                case "remove":
                {
                    Remove(args[0], runtime);
                    break;
                }
                case "removeat":
                {
                    int index = (int)Math.Floor(args[0].GetFloatValue(runtime));
                    RemoveAt(index, runtime);
                    break;
                }
                case "add":
                {
                    Add(args[0], runtime);
                    break;
                }
                case "indexof":
                {
                    return IndexOf(args[0], runtime);
                }
                case "contains":
                {
                    return Contains(args[0], runtime);
                }
            }
            runtime.Error(ScriptError.InvalidMethodError(GetType(), name));
            return ScriptVar.Null;
        }

        protected abstract bool TryGetT(ScriptVar var, IScriptRuntime runtime, out T value);

        protected abstract ScriptVar GetScriptVar(T value);

        public void SetProperty(IScriptRuntime runtime, string name, ScriptVar value)
        {
            runtime?.Error(new ScriptError("Cannot Set Property of Array", "Arrays cannot set properties."));
        }

        public ScriptVar GetItem(int index, IScriptRuntime runtime)
        {
            if (index < 0 || index >= _list.Count)
            {
                IndexError(runtime);
                return ScriptVar.Null;
            }
            else
            {
                T value = _list[index];
                return GetScriptVar(value);
            }
        }

        public void SetItem(int index, ScriptVar value, IScriptRuntime runtime)
        {
            if (ReadOnly)
            {
                ReadOnlyError(runtime);
            }
            else if (TryGetT(value, runtime, out T v))
            {
                _list[index] = v;
            }
        }

        public void Insert(int index, ScriptVar value, IScriptRuntime runtime)
        {
            if (ReadOnly)
            {
                ReadOnlyError(runtime);
            }
            if (TryGetT(value, runtime, out T v))
            {
                _list.Insert(index, v);
            }
        }

        public void Add(ScriptVar value, IScriptRuntime runtime)
        {
            if (ReadOnly)
            {
                ReadOnlyError(runtime);
            }
            else
            {
                if (TryGetT(value, runtime, out T v))
                {
                    _list.Add(v);
                }
            }
        }

        public void Remove(ScriptVar value, IScriptRuntime runtime)
        {
            if (ReadOnly)
            {
                ReadOnlyError(runtime);
            }
            if (TryGetT(value, runtime, out T v))
            {
                _list.Remove(v);
            }
        }

        public void RemoveAt(int index, IScriptRuntime runtime)
        {
            if (ReadOnly)
            {
                ReadOnlyError(runtime);
            }
            else if (index < 0 || index >= _list.Count)
            {
                IndexError(runtime);
            }
            else
            {
                _list.RemoveAt(index);
            }
        }

        public void Clear(IScriptRuntime runtime)
        {
            if (ReadOnly)
            {
                ReadOnlyError(runtime);
            }
            else
            {
                _list.Clear();
            }
        }

        public ScriptVar IndexOf(ScriptVar value, IScriptRuntime runtime)
        {
            if (TryGetT(value, runtime, out T v))
            {
                return new ScriptVar(_list.IndexOf(v));
            }
            return ScriptVar.Null;
        }

        public ScriptVar Contains(ScriptVar value, IScriptRuntime runtime)
        {
            if (TryGetT(value, runtime, out T v) && _list.Contains(v))
            {
                return ScriptVar.True;
            }
            else
            {
                return ScriptVar.False;
            }
        }

        private void ReadOnlyError(IScriptRuntime runtime)
        {
            runtime?.Error(new ScriptError("Cannot Modify Array", "Cannot modify Array; Array is read-only."));
        }

        private void IndexError(IScriptRuntime runtime)
        {
            runtime.Error(new ScriptError("Cannot Index Array", "Cannot index Array; index is out of range."));
        }

        public ScriptArrayInternal(IList<T> list, bool readOnly)
        {
            _list = list;
            ReadOnly = readOnly;
        }
    }
}

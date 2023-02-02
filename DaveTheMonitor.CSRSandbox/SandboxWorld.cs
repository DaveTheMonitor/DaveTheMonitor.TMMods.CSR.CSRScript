using DaveTheMonitor.TMMods.CSR.CSRScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DaveTheMonitor.CSRSandbox
{
    internal sealed class SandboxWorld : IScriptContext
    {
        private MainWindow _window;
        private Dictionary<Point3, string> _savedBlocks;

        private string GetBlock(int x, int y, int z)
        {
            if (_savedBlocks.TryGetValue(new Point3(x, y, z), out string block))
            {
                return block;
            }
            else if (GroundLevel(x, z) < y)
            {
                return "None";
            }

            int mod = (x + y - z) % 5;
            switch (mod)
            {
                case 0: return "Grass";
                case 1: return "Dirt";
                case 2: return "Basalt";
                case 3: return "Sand";
                case 4: return "Clay";
            }
            return "None";
        }

        private void SetBlock(int x, int y, int z, string block)
        {
            _savedBlocks[new Point3(x, y, z)] = block;
        }

        private int GroundLevel(int x, int z)
        {
            return (((x >> 1) * z) << 2) % 300;
        }

        public string GetName()
        {
            return "Sandbox World";
        }

        public ScriptVar GetProperty(IScriptRuntime runtime, string name)
        {
            return ScriptVar.Null;
        }

        public ScriptVar Invoke(IScriptRuntime r, string name, IList<ScriptVar> args)
        {
            switch (name)
            {
                case "getgroundheight":
                {
                    if (args.Count == 2)
                    {
                        return new ScriptVar(GroundLevel((int)args[0].GetFloatValue(r), (int)args[1].GetFloatValue(r)));
                    }
                    else
                    {
                        InvalidArgCountError(r, 2, args.Count, name);
                    }
                    break;
                    
                }
                case "getsealevel":
                {
                    if (args.Count == 0)
                    {
                        return new ScriptVar(200);
                    }
                    else
                    {
                        InvalidArgCountError(r, 0, args.Count, name);
                    }
                    break;
                }
                case "getblock":
                {
                    if (args.Count == 3)
                    {
                        return new ScriptVar(GetBlock((int)args[0].GetFloatValue(r), (int)args[1].GetFloatValue(r), (int)args[2].GetFloatValue(r)));
                    }
                    else
                    {
                        InvalidArgCountError(r, 3, args.Count, name);
                    }
                    break;
                }
                case "setblock":
                {
                    if (args.Count == 4)
                    {
                        SetBlock((int)args[0].GetFloatValue(r), (int)args[1].GetFloatValue(r), (int)args[2].GetFloatValue(r), args[3].GetStringValue(r));
                    }
                    else
                    {
                        InvalidArgCountError(r, 4, args.Count, name);
                    }
                    break;
                }
                case "notify":
                {
                    if (args.Count == 1)
                    {
                        _window.Log(args[0].GetStringValue(r));
                    }
                    else
                    {
                        InvalidArgCountError(r, 1, args.Count, name);
                    }
                    break;
                }
            }
            r.Error(ScriptError.InvalidMethodError(GetType(), name));
            return ScriptVar.Null;
        }

        private static void InvalidArgCountError(IScriptRuntime runtime, int expected, int received, string method)
        {
            runtime.Error(ScriptError.InvalidArgCountError(expected, received));
        }

        public void SetProperty(IScriptRuntime runtime, string name, ScriptVar value)
        {
            throw new NotImplementedException();
        }

        public SandboxWorld(MainWindow window)
        {
            _window = window;
            _savedBlocks = new Dictionary<Point3, string>();
        }
    }
}

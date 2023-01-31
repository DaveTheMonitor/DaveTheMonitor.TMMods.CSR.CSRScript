using DaveTheMonitor.TMMods.CSR.CSRScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public ScriptVar Invoke(IScriptRuntime runtime, string name, IList<ScriptVar> args)
        {
            switch (name)
            {
                case "getgroundheight":
                {
                    int argCount = 2;
                    if (args.Count != argCount)
                    {
                        runtime.Error("Invalid arguments", "Invoke expected " + argCount + " arguments, received " + args.Count);
                        break;
                    }
                    return new ScriptVar(GroundLevel((int)args[0].GetFloatValue(runtime), (int)args[1].GetFloatValue(runtime)));
                }
                case "getsealevel":
                {
                    int argCount = 0;
                    if (args.Count != argCount)
                    {
                        runtime.Error("Invalid arguments", "Invoke expected " + argCount + " arguments, received " + args.Count);
                        break;
                    }
                    return new ScriptVar(200);
                }
                case "getblock":
                {
                    int argCount = 3;
                    if (args.Count != argCount)
                    {
                        runtime.Error("Invalid arguments", "Invoke expected " + argCount + " arguments, received " + args.Count);
                        break;
                    }
                    return new ScriptVar(GetBlock((int)args[0].GetFloatValue(runtime), (int)args[1].GetFloatValue(runtime), (int)args[2].GetFloatValue(runtime)));
                }
                case "setblock":
                {
                    int argCount = 4;
                    if (args.Count != argCount)
                    {
                        runtime.Error("Invalid arguments", "Invoke expected " + argCount + " arguments, received " + args.Count);
                        break;
                    }
                    SetBlock((int)args[0].GetFloatValue(runtime), (int)args[1].GetFloatValue(runtime), (int)args[2].GetFloatValue(runtime), args[3].GetStringValue(runtime));
                    break;
                }
                case "notify":
                {
                    int argCount = 1;
                    if (args.Count != argCount)
                    {
                        runtime.Error("Invalid arguments", "Invoke expected " + argCount + " arguments, received " + args.Count);
                        break;
                    }
                    _window.Log(args[0].GetStringValue(runtime));
                    break;
                }
            }

            return ScriptVar.Null;
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

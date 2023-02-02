using System;
using System.Collections.Generic;

namespace DaveTheMonitor.TMMods.CSR.CSRScript
{
    /// <summary>
    /// An <see cref="IScriptContext"/> that gives script access to a <see cref="Random"/> instance.
    /// </summary>
    public sealed class ScriptRandom : IScriptContext
    {
        private Random _random;
        private int _seed;

        /// <inheritdoc/>
        public string GetName()
        {
            return "Random";
        }

        /// <inheritdoc/>
        public ScriptVar GetProperty(IScriptRuntime runtime, string name)
        {
            switch (name)
            {
                case "seed": return new ScriptVar(_seed);
            }
            runtime.Error(ScriptError.InvalidPropertyError(GetType(), name));
            return ScriptVar.Null;
        }

        /// <inheritdoc/>
        public ScriptVar Invoke(IScriptRuntime runtime, string name, IList<ScriptVar> args)
        {
            switch (name)
            {
                case "random":
                {
                    if (args.Count == 0)
                    {
                        return new ScriptVar((float)_random.NextDouble());
                    }
                    else if (args.Count == 1)
                    {
                        return new ScriptVar((float)_random.NextDouble() * args[0].GetFloatValue(runtime));
                    }
                    else if (args.Count == 2)
                    {
                        float min = args[0].GetFloatValue(runtime);
                        float max = args[1].GetFloatValue(runtime);
                        return new ScriptVar(((float)_random.NextDouble() * (max - min)) + min);
                    }
                    else
                    {
                        InvalidArgCountError(runtime, 2, args.Count, name);
                    }
                    break;
                }
            }
            runtime.Error(ScriptError.InvalidMethodError(GetType(), name));
            return ScriptVar.Null;
        }

        private void InvalidArgCountError(IScriptRuntime runtime, int expected, int received, string method)
        {
            runtime.Error(ScriptError.InvalidArgCount(expected, received, GetType(), method));
        }

        /// <inheritdoc/>
        public void SetProperty(IScriptRuntime runtime, string name, ScriptVar value)
        {
            
        }

        /// <summary>
        /// Creates a new instance with the specified seed.
        /// </summary>
        /// <param name="seed">The seed of the random number generator.</param>
        public ScriptRandom(int seed)
        {
            _random = new Random(seed);
            _seed = seed;
        }
    }
}

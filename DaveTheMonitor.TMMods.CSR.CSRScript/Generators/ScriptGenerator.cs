using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaveTheMonitor.TMMods.CSR.CSRScript.Generators
{
    /// <summary>
    /// Creates or modifies scripts directly using <see cref="ScriptOpCode"/>s, without compiling them.
    /// </summary>
    public sealed class ScriptGenerator
    {
        /// <summary>
        /// The script this generator is modifying. Any changes made by this generator will not be saved to the script until <see cref="FinalizeScript(Dictionary{string, int})"/> is called.
        /// </summary>
        public Script Script { get; private set; }
        /// <summary>
        /// The operations performed by this script. It's better to not modify this directly.
        /// </summary>
        public List<ScriptOp> Ops { get; private set; }
        /// <summary>
        /// The current position of this generator. Operations are inserted here.
        /// </summary>
        public int Position { get; set; }
        private object[] _argsCache1;

        /// <summary>
        /// Saves any changes made to the script. Should be called when the script is complete.
        /// </summary>
        /// <param name="inVars">A dictionary containing the input variable indexes for variable names. Required for scripts to be properly initialized.</param>
        public void FinalizeScript(Dictionary<string, int> inVars)
        {
            FinalizeScript(inVars, false);
        }

        internal void FinalizeScript(Dictionary<string, int> inVars, bool compiled)
        {
            Script.SetOps(Ops.ToArray(), compiled);
            Script.InVars = inVars;
        }

        /// <summary>
        /// Inserts an operation with no args at the current position.
        /// </summary>
        /// <param name="op">The operation to insert.</param>
        /// <remarks>
        /// <para>Should be used for operations with no args, ie. <see cref="ScriptOpCode.Nop"/> or <see cref="ScriptOpCode.Null"/>. For operations with args, use <see cref="InsertOp(ScriptOpCode, object[])"/></para>
        /// </remarks>
        public void InsertOp(ScriptOpCode op)
        {
            InsertOp(op, Array.Empty<object>());
        }

        /// <summary>
        /// Inserts an operation with one arg at the current position.
        /// </summary>
        /// <param name="op">The operation to insert.</param>
        /// <param name="arg">The arg of the operation.</param>
        public void InsertOp(ScriptOpCode op, object arg)
        {
            _argsCache1[0] = arg;
            InsertOp(op, _argsCache1);
        }

        /// <summary>
        /// Inserts an operation to push a float literal on to the stack.
        /// </summary>
        /// <param name="value">The value to push.</param>
        public void InsertFloatLiteral(float value)
        {
            InsertOp(new ScriptOpFloat(ScriptOpCode.FloatLiteral, value));
        }

        /// <summary>
        /// Inserts an operation to push a string literal on to the stack.
        /// </summary>
        /// <param name="value">The value to push.</param>
        public void InsertStringLiteral(string value)
        {
            InsertOp(new ScriptOpString(ScriptOpCode.StringLiteral, value));
        }

        /// <summary>
        /// Inserts an operation to push a bool literal on to the stack.
        /// </summary>
        /// <param name="value">The value to push.</param>
        public void InsertBoolLiteral(bool value)
        {
            if (value)
            {
                InsertOp(new ScriptOp(ScriptOpCode.TrueLiteral));
            }
            else
            {
                InsertOp(new ScriptOp(ScriptOpCode.FalseLiteral));
            }
        }

        /// <summary>
        /// Inserts an operation with one float arg at the current position.
        /// </summary>
        /// <param name="op">The operation to insert.</param>
        /// <param name="value">The argument of the operation.</param>
        public void InsertFloatOp(ScriptOpCode op, float value)
        {
            InsertOp(new ScriptOpFloat(op, value));
        }

        /// <summary>
        /// Inserts an operation with one float arg at the current position.
        /// </summary>
        /// <param name="op">The operation to insert.</param>
        /// <param name="value">The argument of the operation.</param>
        public void InsertStringOp(ScriptOpCode op, string value)
        {
            InsertOp(new ScriptOpString(op, value));
        }

        /// <summary>
        /// Inserts an operation with one float arg at the current position.
        /// </summary>
        /// <param name="op">The operation to insert.</param>
        /// <param name="value">The argument of the operation.</param>
        public void InsertIntOp(ScriptOpCode op, int value)
        {
            InsertOp(new ScriptOpInt(op, value));
        }

        /// <summary>
        /// Inserts an invoke operation with the specified name and arg count.
        /// </summary>
        /// <param name="isStatic">True if this is invoking a static method.</param>
        /// <param name="ret">True if this invoke should return a value.</param>
        /// <param name="name">The name of the method to invoke.</param>
        /// <param name="args">The number of arguments to pass from the stack.</param>
        public void InsertInvoke(bool isStatic, bool ret, string name, int args)
        {
            ScriptOpCode op;
            if (isStatic)
            {
                op = ret ? ScriptOpCode.InvokeStaticRet : ScriptOpCode.InvokeStatic;
            }
            else
            {
                op = ret ? ScriptOpCode.InvokeRet : ScriptOpCode.Invoke;
            }
            InsertOp(new ScriptOpInvoke(op, name, args));
        }

        /// <summary>
        /// Inserts an operation with no args at the current position.
        /// </summary>
        /// <param name="op">The operation to insert.</param>
        /// <param name="args">The args of the operation.</param>
        public void InsertOp(ScriptOpCode op, params object[] args)
        {
            switch (op)
            {
                case ScriptOpCode.Nop:
                case ScriptOpCode.Exit:
                case ScriptOpCode.Pop:
                case ScriptOpCode.TrueLiteral:
                case ScriptOpCode.FalseLiteral:
                case ScriptOpCode.Null:
                case ScriptOpCode.Print:
                case ScriptOpCode.Add:
                case ScriptOpCode.Sub:
                case ScriptOpCode.Multi:
                case ScriptOpCode.Div:
                case ScriptOpCode.Mod:
                case ScriptOpCode.Equal:
                case ScriptOpCode.NotEqual:
                case ScriptOpCode.And:
                case ScriptOpCode.Or:
                case ScriptOpCode.Invert:
                case ScriptOpCode.Gt:
                case ScriptOpCode.Gte:
                case ScriptOpCode.Lt:
                case ScriptOpCode.Lte:
                case ScriptOpCode.Context:
                case ScriptOpCode.PushContext:
                {
                    InsertOp(new ScriptOp(op));
                    break;
                }
                case ScriptOpCode.StringLiteral:
                case ScriptOpCode.GetProperty:
                {
                    InsertOp(new ScriptOpString(op, (string)args[0]));
                    break;
                }
                case ScriptOpCode.FloatLiteral:
                {
                    InsertOp(new ScriptOpFloat(op, (float)args[0]));
                    break;
                }
                case ScriptOpCode.LoadVar:
                case ScriptOpCode.StoreVar:
                case ScriptOpCode.Delete:
                {
                    InsertOp(new ScriptOpInt(op, (int)args[0]));
                    break;
                }
                case ScriptOpCode.Jump:
                case ScriptOpCode.JT:
                case ScriptOpCode.JF:
                case ScriptOpCode.JE:
                case ScriptOpCode.JNe:
                case ScriptOpCode.JGt:
                case ScriptOpCode.JGte:
                case ScriptOpCode.JLt:
                case ScriptOpCode.JLte:
                {
                    InsertOp(new ScriptOpInt(op, (int)args[0]));
                    break;
                }
                case ScriptOpCode.Invoke:
                case ScriptOpCode.InvokeRet:
                case ScriptOpCode.InvokeStatic:
                case ScriptOpCode.InvokeStaticRet:
                {
                    InsertOp(new ScriptOpInvoke(op, (string)args[0], (int)args[1]));
                    break;
                }
                default: throw new ArgumentException("invalid op " + op);
            }
        }

        private void InsertOp(ScriptOp op)
        {
            Ops.Insert(Position, op);
            Position++;
        }

        /// <summary>
        /// Converts the operations of a script to human-readable text.
        /// </summary>
        /// <param name="script">The script to convert.</param>
        /// <param name="includeLines">If true, line numbers will be included in the output.</param>
        /// <returns>A string representing the operations of this script.</returns>
        public static string ConvertOps(Script script, bool includeLines)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < script.Ops.Length; i++)
            {
                if (includeLines)
                {
                    builder.AppendLine(i.ToString("0000") + " | " + script.Ops[i].ToString());
                }
                else
                {
                    builder.AppendLine(script.Ops[i].ToString());
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// Creates a new Script Generator with a new, empty <see cref="CSRScript.Script"/>.
        /// </summary>
        public ScriptGenerator(string scriptName)
        {
            Script = new Script(scriptName);
            Ops = new List<ScriptOp>(16);
            Position = 0;
            _argsCache1 = new object[1];
        }

        /// <summary>
        /// Creates a new Script Generator for an existing <see cref="Script"/>. Can be used to modify scripts.
        /// </summary>
        /// <param name="script">The script to modify.</param>
        /// <remarks>
        /// <para>Modified scripts are not considered compiled, even if they originally were before being modified.</para>
        /// </remarks>
        public ScriptGenerator(Script script)
        {
            Script = script;
            Ops = script.Ops.ToList();
            Position = 0;
            _argsCache1 = new object[1];
        }
    }
}

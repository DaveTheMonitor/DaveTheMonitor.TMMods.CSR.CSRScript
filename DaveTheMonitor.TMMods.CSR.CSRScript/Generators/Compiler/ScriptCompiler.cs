using System;
using System.Collections.Generic;

namespace DaveTheMonitor.TMMods.CSR.CSRScript.Generators.Compiler
{
    /// <summary>
    /// Compiles scripts from written commands.
    /// </summary>
    public sealed class ScriptCompiler
    {
        private ScriptLexer _lexer;
        private ScriptParser _parser;
        private ScriptCompilerGenerator _generator;
        /// <summary>
        /// A delegate invoked when an error occurs during compilation.
        /// </summary>
        private Action<string, string> _errorHandler;

        /// <summary>
        /// Compiles a Script from raw CSRScript source code.
        /// </summary>
        /// <param name="name">The name of the script.</param>
        /// <param name="rawScript">The raw script source code.</param>
        /// <returns>A new compiled Script.</returns>
        public Script Compile(string name, string rawScript)
        {
            List<ScriptToken> tokens = _lexer.Lex(rawScript);
            if (!_lexer._errored)
            {
                ScriptScriptTreeNode tree = _parser.Parse(rawScript, tokens);
                if (!_parser._errored)
                {
                    Script script = _generator.Generate(name, tree, rawScript);
                    if (!_generator._errored)
                    {
                        return script;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Adds a delegate to be invoked when an error occurs during compilation.
        /// </summary>
        /// <param name="handler">The handler to add.</param>
        public void AddErrorHandler(Action<string, string> handler)
        {
            _errorHandler += handler;
            _lexer.ErrorHandler += handler;
            _parser.ErrorHandler += handler;
            _generator.ErrorHandler += handler;
        }

        /// <summary>
        /// Removes a delegate that is invoked when an error occurs during compilation.
        /// </summary>
        /// <param name="handler">The handler to remove.</param>
        public void RemoveErrorHandler(Action<string, string> handler)
        {
            _errorHandler -= handler;
            _lexer.ErrorHandler -= handler;
            _parser.ErrorHandler -= handler;
            _generator.ErrorHandler -= handler;
        }

        /// <summary>
        /// Creates a new script compiler that can compile a script with default settings.
        /// </summary>
        public ScriptCompiler()
        {
            _lexer = new ScriptLexer();
            _parser = new ScriptParser();
            _generator = new ScriptCompilerGenerator();
        }
    }
}

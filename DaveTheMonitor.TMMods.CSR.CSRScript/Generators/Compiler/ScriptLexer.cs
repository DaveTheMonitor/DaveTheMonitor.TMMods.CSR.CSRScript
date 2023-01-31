using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaveTheMonitor.TMMods.CSR.CSRScript.Generators.Compiler
{
    internal sealed class ScriptLexer
    {
        public Action<string, string> ErrorHandler;
        private string[] _operators;
        private string[] _assignment;
        private string[] _keywords;
        private List<ScriptToken> _tokens;
        private int _position;
        private string _raw;
        internal bool _errored;

        public List<ScriptToken> Lex(string raw)
        {
            _errored = false;
            _raw = raw;
            _tokens.Clear();
            StringBuilder builder = new StringBuilder();
            bool inString = false;
            bool escaping = false;
            bool inComment = false;
            _position = 0;
            for (_position = 0; _position < raw.Length; _position++)
            {
                char c = raw[_position];
                if (inComment)
                {
                    if (c == '\n')
                    {
                        inComment = false;
                        AddToken(ScriptTokenType.End, "", _position);
                    }
                    continue;
                }
                if (inString)
                {
                    if (c == '\n')
                    {
                        Error("Invalid literal", "New line in string literal", _position);
                        break;
                    }

                    if (escaping)
                    {
                        builder.Append(c);
                        escaping = false;
                        continue;
                    }

                    if (c == '\\')
                    {
                        escaping = true;
                        continue;
                    }
                    if (c == '"')
                    {
                        AddToken(ScriptTokenType.StringLiteral, builder.ToString(), _position - builder.Length - 1);
                        builder.Clear();
                        inString = false;
                        escaping = false;
                        continue;
                    }
                    builder.Append(c);
                    continue;
                }

                if (c == '/' && raw[_position + 1] == '/')
                {
                    inComment = true;
                    AddCurrent(builder);
                    continue;
                }
                else if (c == '"')
                {
                    inString = true;
                    continue;
                }
                else if (c == '\n')
                {
                    AddCurrent(builder);
                    AddToken(ScriptTokenType.End, "", _position);
                    continue;
                }
                else if (c == ']')
                {
                    AddCurrent(builder);
                    AddToken(ScriptTokenType.RB, "]", _position);
                    continue;
                }
                else if (c == '[')
                {
                    AddCurrent(builder);
                    AddToken(ScriptTokenType.LB, "[", _position);
                    continue;
                }
                else if (c == ' ')
                {
                    AddCurrent(builder);
                    continue;
                }
                builder.Append(c);
            }

            _tokens.Add(new ScriptToken(ScriptTokenType.End, "", raw.Length - 1));
            return _tokens;
        }

        private void AddCurrent(StringBuilder builder)
        {
            if (builder.Length > 0)
            {
                string value = builder.ToString().Trim();
                if (string.IsNullOrWhiteSpace(value))
                {
                    return;
                }
                string lower = value.ToLowerInvariant();
                AddToken(lower, _position - builder.Length);
                builder.Clear();
            }
        }

        private void AddToken(string value, long position)
        {
            if (value == "[")
            {
                AddToken(ScriptTokenType.LB, "[", position);
            }
            else if (value == "]")
            {
                AddToken(ScriptTokenType.RB, "]", position);
            }
            else if (value == "null")
            {
                AddToken(ScriptTokenType.Null, "null", position);
            }
            else if (float.TryParse(value, out float _))
            {
                AddToken(ScriptTokenType.FloatLiteral, value, position);
            }
            else if (bool.TryParse(value, out bool _))
            {
                AddToken(ScriptTokenType.BoolLiteral, value, position);
            }
            else if (_operators.Contains(value))
            {
                AddToken(ScriptTokenType.Operator, value, position);
            }
            else if (_assignment.Contains(value))
            {
                AddToken(ScriptTokenType.Assignment, value, position);
            }
            else if (_keywords.Contains(value))
            {
                AddToken(ScriptTokenType.Keyword, value, position);
            }
            else
            {
                AddToken(ScriptTokenType.ID, value, position);
            }
        }

        private void AddToken(ScriptTokenType type, string value, long position)
        {
            _tokens.Add(new ScriptToken(type, value, position));
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

        public ScriptLexer()
        {
            _operators = new string[]
            {
                "==",
                "!=",
                "+",
                "-",
                "*",
                "/",
                "%",
                "and",
                "&&",
                "or",
                "||",
                "not",
                "!",
                ">",
                ">=",
                "<",
                "<="
            };
            _assignment = new string[]
            {
                "=",
                "+=",
                "-=",
                "/=",
                "*="
            };
            _keywords = new string[]
            {
                "invoke",
                "return",
                "if",
                "else",
                "elseif",
                "then",
                "endif",
                "while",
                "do",
                "end",
                "for",
                "var",
                "context",
                "getproperty",
                "setproperty",
                "in",
                "print",
                "continue",
                "break",
                "delete"
            };
            _tokens = new List<ScriptToken>();
        }
    }
}

namespace DaveTheMonitor.TMMods.CSR.CSRScript.Generators.Compiler
{
    internal sealed class ScriptToken
    {
        public ScriptTokenType Type { get; internal set; }
        public string Value { get; internal set; }
        public long Position { get; private set; }
        public bool IsBracket => Type == ScriptTokenType.LB || Type == ScriptTokenType.RB;
        public bool IsLiteral => Type == ScriptTokenType.StringLiteral || Type == ScriptTokenType.FloatLiteral || Type == ScriptTokenType.BoolLiteral;
        public bool IsLiteralOrID => IsLiteral || Type == ScriptTokenType.ID;
        public bool IsAnd => Type == ScriptTokenType.Operator && (Value == "and" || Value == "&&");
        public bool IsOr => Type == ScriptTokenType.Operator && (Value == "or" || Value == "||");
        public bool IsNot => Type == ScriptTokenType.Operator && (Value == "not" || Value == "!");
        public bool IsLogicalOperator => IsAnd || IsOr || IsNot;
        public bool IsBinaryOperator => Type == ScriptTokenType.Operator && (Value == "+" || Value == "-" || Value == "*" || Value == "/" || Value == "%" || Value == ">" || Value == ">=" || Value == "<" || Value == "<=" || Value == "==" || Value == "!=");

        public bool Is(ScriptTokenType type, string value = null)
        {
            if (value == null)
            {
                return Type == type;
            }
            else
            {
                return Type == type && value == Value;
            }
        }

        public override string ToString()
        {
            if (Value == null)
            {
                return Type.ToString();
            }
            else
            {
                return $"{Type} : {Value}";
            }
        }

        public ScriptToken(ScriptTokenType type, string value, long position)
        {
            Type = type;
            Value = value;
            Position = position;
        }
    }
}

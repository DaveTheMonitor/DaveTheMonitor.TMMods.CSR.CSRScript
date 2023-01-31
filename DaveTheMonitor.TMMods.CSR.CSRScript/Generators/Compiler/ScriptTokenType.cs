namespace DaveTheMonitor.TMMods.CSR.CSRScript.Generators.Compiler
{
    internal enum ScriptTokenType
    {
        Keyword,
        ID,
        StringLiteral,
        FloatLiteral,
        BoolLiteral,
        Null,
        Operator,
        Assignment,
        End,
        LB,
        RB,
    }
}

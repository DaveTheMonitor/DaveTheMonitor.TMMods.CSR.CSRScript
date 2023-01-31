namespace DaveTheMonitor.TMMods.CSR.CSRScript.Generators.Compiler
{
    internal enum ScriptTreeNodeType
    {
        Script,
        FloatLiteral,
        StringLiteral,
        BoolLiteral,
        Null,
        ID,
        StaticID,
        BinaryExpression,
        Command,
        IfStatement,
        WhileStatement,
        Var,
        InVar,
        Invoke,
        GetProperty
    }
}

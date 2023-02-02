using System;
using System.Collections.Generic;

namespace DaveTheMonitor.TMMods.CSR.CSRScript.Generators.Compiler
{
    internal class ScriptParser
    {
        private enum ScopeContext : byte
        {
            Script,
            If,
            While
        }
        public Action<string, string> ErrorHandler;
        private ScriptScriptTreeNode CurrentScope => _scope.Peek();
        private int _position;
        private List<ScriptToken> _tokens;
        private ScriptScriptTreeNode _tree;
        private Stack<ScriptScriptTreeNode> _scope;
        private Stack<ScopeContext> _scopeContext;
        private Dictionary<string, int> _inVars;
        private int _varIndex;
        private string _raw;
        internal bool _errored;

        public ScriptScriptTreeNode Parse(string raw, List<ScriptToken> tokens)
        {
            _errored = false;
            _raw = raw;
            _position = 0;
            _tokens = tokens;
            _tree = new ScriptScriptTreeNode(0);
            _inVars = new Dictionary<string, int>();
            _varIndex = 0;
            _scope.Clear();
            _scope.Push(_tree);
            _scopeContext.Push(ScopeContext.Script);

            bool inBracket = false;
            bool tokenVal = false;
            long bracketPos = 0;
            foreach (ScriptToken token in _tokens)
            {
                if (_errored)
                {
                    break;
                }
                if (token.Is(ScriptTokenType.End) && inBracket)
                {
                    Error("Invalid Token", "Line end in token", bracketPos);
                }
                else if (token.Is(ScriptTokenType.LB))
                {
                    if (inBracket)
                    {
                        Error("Invalid Token", "Attempted to open a token without closing previous token", bracketPos);
                    }
                    else
                    {
                        inBracket = true;
                        bracketPos = token.Position;
                    }
                }
                else if (token.Is(ScriptTokenType.RB))
                {
                    if (!inBracket)
                    {
                        Error("Invalid Token", "Attempted to close a token without opening a token", token.Position);
                    }
                    else
                    {
                        inBracket = false;
                        bracketPos = token.Position;
                        tokenVal = false;
                    }
                }
                else if (token.IsLiteralOrID)
                {
                    if (!inBracket)
                    {
                        Error(token.Type + " outside of token", token.Type + " outside of token", token.Position);
                    }
                    else if (tokenVal)
                    {
                        Error("Token cannot contain multiple values", "Token cannot contain multiple values", bracketPos);
                    }
                    else
                    {
                        tokenVal = true;
                    }
                }
            }

            while (_position < _tokens.Count)
            {
                ScriptToken token = NextToken();
                if (token.IsBracket)
                {
                    continue;
                }
                if (token.Is(ScriptTokenType.Keyword))
                {
                    switch (token.Value)
                    {
                        case "in": AddInVars(token); break;
                        case "var": AddVar(token); break;
                        case "invoke": AddInvoke(token); break;
                        case "getproperty": AddGetProperty(token); break;
                        case "break":
                        case "continue":
                        case "exit":
                        {
                            AddCommand(token, 0);
                            break;
                        }
                        case "context":
                        case "print":
                        case "wait":
                        {
                            AddCommand(token, 1);
                            break;
                        }
                        case "if": AddIf(token); break;
                        case "else": AddElse(token); break;
                        case "elseif":
                        {
                            AddElse(token);
                            AddIf(token);
                            break;
                        }
                        case "endif": AddEndIf(token); break;
                        case "while": AddWhile(token); break;
                        case "end": AddEnd(token); break;
                        case "delete":
                        {
                            AddCommand(token, -1);
                            break;
                        }
                    }
                }
            }

            return _tree;
        }

        private void AddInVars(ScriptToken token)
        {
            InVarScriptTreeNode node = new InVarScriptTreeNode(token.Position);
            ScriptToken nextToken = NextToken();
            while (!nextToken.Is(ScriptTokenType.End))
            {
                if (nextToken.Is(ScriptTokenType.ID))
                {
                    _inVars.Add(nextToken.Value, _varIndex++);
                    node.IDs.Add(new IDScriptTreeNode(nextToken.Position, nextToken.Value, _tree));
                }
                else if (!nextToken.IsBracket)
                {
                    Error("Invalid token in InVars", "Invalid token" + nextToken.Value + " in InVars", nextToken.Position);
                }
                nextToken = NextToken();
            }
            CurrentScope.Statements.Add(node);
        }

        private void AddVar(ScriptToken token)
        {
            VarScriptTreeNode node = new VarScriptTreeNode(token.Position);
            SkipToken(true);
            ScriptToken idToken = NextToken();
            if (!idToken.Is(ScriptTokenType.ID))
            {
                ErrorInvalidTokenType(idToken);
                return;
            }
            node.ID = new IDScriptTreeNode(idToken.Position, idToken.Value, _tree);
            SkipToken(true);
            ScriptToken nextToken = NextToken();
            if (!nextToken.Is(ScriptTokenType.Assignment))
            {
                node.Value = new NullScriptTreeNode(idToken.Position);
                CurrentScope.Statements.Add(node);
                while (!nextToken.Is(ScriptTokenType.End))
                {
                    if (!nextToken.IsBracket)
                    {
                        if (nextToken.Is(ScriptTokenType.ID))
                        {
                            VarScriptTreeNode varNode = new VarScriptTreeNode(nextToken.Position);
                            varNode.ID = new IDScriptTreeNode(nextToken.Position, nextToken.Value, _tree);
                            varNode.Value = new NullScriptTreeNode(nextToken.Position);
                            CurrentScope.Statements.Add(varNode);
                        }
                    }
                    nextToken = NextToken();
                }
            }
            else
            {
                ScriptTreeNode expr = CalcExpression(EndAtEnd);
                node.Value = expr;
                CurrentScope.Statements.Add(node);
            }
        }

        private void AddInvoke(ScriptToken token)
        {
            List<ScriptTreeNode> args = new List<ScriptTreeNode>();
            ScriptToken nextToken = NextToken();
            while (!nextToken.Is(ScriptTokenType.End))
            {
                if (!nextToken.IsBracket)
                {
                    args.Add(GetLiteralOrID(nextToken));
                }
                nextToken = NextToken();
            }
            if (args.Count == 0)
            {
                Error("Invalid arguments", "Invoke requires at least one argument", token.Position);
                return;
            }
            else
            {
                bool ret = false;
                bool isStatic = false;
                int minArgs = 1;
                ScriptTreeNode nextArg = args[0];
                if (nextArg.Type == ScriptTreeNodeType.ID && ((IDScriptTreeNode)nextArg).ID == "static")
                {
                    minArgs++;
                    if (args.Count < minArgs)
                    {
                        ArgError(minArgs);
                        return;
                    }
                    ((IDScriptTreeNode)nextArg).MakeStatic();
                    nextArg = args[minArgs - 1];
                    isStatic = true;
                }
                if (nextArg.Type == ScriptTreeNodeType.ID && ((IDScriptTreeNode)nextArg).ID.StartsWith("var:"))
                {
                    minArgs++;
                    if (args.Count < minArgs)
                    {
                        ArgError(minArgs);
                        return;
                    }
                    IDScriptTreeNode idArg = (IDScriptTreeNode)nextArg;
                    idArg.ID = idArg.ID.Substring(4).TrimStart();
                    idArg.RecalcIndex(_tree);
                    ret = true;
                }
                string name;
                int nameIndex = ret ? minArgs - 1 : 0;
                ScriptTreeNode nameNode = args[nameIndex];
                if (nameNode.Type == ScriptTreeNodeType.ID || nameNode.Type == ScriptTreeNodeType.StaticID)
                {
                    name = ((IDScriptTreeNode)nameNode).ID;
                    ((IDScriptTreeNode)nameNode).MakeStatic();
                }
                else
                {
                    Error("Invalid arguments", "Invoke name must be a static identifier", nameNode.Char);
                    return;
                }
                InvokeScriptTreeNode invoke = new InvokeScriptTreeNode(token.Position, name, ret, isStatic);
                invoke.Args = args;
                CurrentScope.Statements.Add(invoke);

                void ArgError(int argCount)
                {
                    Error("Invalid arguments", $"{(isStatic ? "Static " : null)}Invoke {(ret ? "With return value " : null)}requires at least {argCount} arguments", token.Position);
                }
            }
        }

        private void AddGetProperty(ScriptToken token)
        {
            List<ScriptTreeNode> args = new List<ScriptTreeNode>();
            ScriptToken nextToken = NextToken();
            while (!nextToken.Is(ScriptTokenType.End))
            {
                if (!nextToken.IsBracket)
                {
                    args.Add(GetLiteralOrID(nextToken));
                }
                nextToken = NextToken();
            }
            if (args.Count != 2)
            {
                Error("Invalid arguments", "getproperty expected " + args + " argument(s), received " + args.Count, token.Position);
                return;
            }

            if (args[0].Type != ScriptTreeNodeType.ID)
            {
                Error("Invalid argument", "Argument 0 of getproperty is type " + args[0].Type + ", expected ID", args[0].Char);
            }
            else if (args[1].Type != ScriptTreeNodeType.ID && args[1].Type != ScriptTreeNodeType.StaticID)
            {
                Error("Invalid argument", "Argument 1 of getproperty is type " + args[1].Type + ", expected StaticID", args[1].Char);
            }

            IDScriptTreeNode arg0 = (IDScriptTreeNode)args[0];
            IDScriptTreeNode arg1 = (IDScriptTreeNode)args[1];
            if (arg0.ID.StartsWith("var:"))
            {
                arg0.ID = arg0.ID.Substring(4).TrimStart();
                arg0.RecalcIndex(_tree);
                arg0.Store = true;
            }
            else
            {
                Error("Invalid argument", "Argument 0 of getproperty is must specify a variable name.", args[0].Char);
            }

            arg1.MakeStatic();

            GetPropertyScriptTreeNode node = new GetPropertyScriptTreeNode(token.Position, arg0, arg1);
            CurrentScope.Statements.Add(node);
        }

        private void AddCommand(ScriptToken token, int args)
        {
            CommandScriptTreeNode node = new CommandScriptTreeNode(token.Position, token.Value);
            ScriptToken nextToken = NextToken();
            int arg = 0;
            while (!nextToken.Is(ScriptTokenType.End))
            {
                if (!nextToken.IsBracket)
                {
                    node.Args.Add(GetLiteralOrID(nextToken));
                    arg++;
                }
                nextToken = NextToken();
            }
            if (args != -1 && arg != args)
            {
                Error("Invalid arguments", node.Value + " expected " + args + " argument(s), received " + arg, token.Position);
            }
            CurrentScope.Statements.Add(node);
        }

        private void AddIf(ScriptToken token)
        {
            ScriptTreeNode condition = CalcExpression(EndAtThen);
            IfScriptTreeNode node = new IfScriptTreeNode(token.Position);
            node.Condition = condition;
            node.Statement = new ScriptScriptTreeNode(NextToken().Position);
            CurrentScope.Statements.Add(node);
            _scope.Push(node.Statement);
            _scopeContext.Push(ScopeContext.If);
        }

        private void AddEndIf(ScriptToken token)
        {
            ScopeContext context = _scopeContext.Pop();
            if (context != ScopeContext.If)
            {
                Error("Unexpected keyword", token.Value + " encountered outside of if statement", token.Position);
            }
            _scope.Pop();
        }

        private void AddElse(ScriptToken token)
        {
            ScopeContext context = _scopeContext.Pop();
            if (context != ScopeContext.If)
            {
                Error("Unexpected keyword", token.Value + " encountered outside of if statement", token.Position);
            }
            _scope.Pop();
            IfScriptTreeNode node = (IfScriptTreeNode)CurrentScope.Statements[CurrentScope.Statements.Count - 1];
            node.ElseStatement = new ScriptScriptTreeNode(token.Position);
            _scope.Push(node.ElseStatement);
        }

        private void AddWhile(ScriptToken token)
        {
            ScriptTreeNode condition = CalcExpression(EndAtDo);
            WhileScriptTreeNode node = new WhileScriptTreeNode(token.Position);
            node.Condition = condition;
            node.Statement = new ScriptScriptTreeNode(NextToken().Position);
            CurrentScope.Statements.Add(node);
            _scope.Push(node.Statement);
            _scopeContext.Push(ScopeContext.While);
        }

        private void AddEnd(ScriptToken token)
        {
            ScopeContext context = _scopeContext.Pop();
            if (context != ScopeContext.While)
            {
                Error("Unexpected keyword", token.Value + " encountered outside of while statement", token.Position);
            }
            _scope.Pop();
        }

        private bool EndAtEnd()
        {
            return PeekToken().Is(ScriptTokenType.End);
        }

        private bool EndAtThen()
        {
            return PeekToken().Is(ScriptTokenType.Keyword, "then");
        }

        private bool EndAtDo()
        {
            return PeekToken().Is(ScriptTokenType.Keyword, "do");
        }

        private ScriptTreeNode CalcExpression(Func<bool> endCondition)
        {
            List<ScriptToken> currentGroup = new List<ScriptToken>();
            List<List<ScriptToken>> groups = new List<List<ScriptToken>>();
            List<ScriptToken> separators = new List<ScriptToken>();
            while (!endCondition())
            {
                ScriptToken token = NextToken();
                if (token.IsBracket || token.Is(ScriptTokenType.End))
                {
                    continue;
                }
                if (token.IsLogicalOperator)
                {
                    groups.Add(currentGroup);
                    separators.Add(token);
                    currentGroup = new List<ScriptToken>();
                }
                else
                {
                    currentGroup.Add(token);
                }
            }
            if (currentGroup.Count > 0)
            {
                groups.Add(currentGroup);
            }

            if (groups.Count == 1 && groups[0].Count == 1)
            {
                return GetLiteralOrID(groups[0][0]);
            }
            else
            {
                if (separators.Count == 0)
                {
                    return CalcBinaryExpression(groups[0]);
                }
                else
                {
                    BinExprScriptTreeNode top = null;
                    BinExprScriptTreeNode current = null;
                    for (int i = 0; i < separators.Count; i++)
                    {
                        ScriptToken op = separators[i];
                        if (current == null)
                        {
                            current = new BinExprScriptTreeNode(op.Position, op.Value);
                            current.Left = CalcBinaryExpression(groups[i]);
                            top = current;
                        }
                        else
                        {
                            BinExprScriptTreeNode expr = new BinExprScriptTreeNode(op.Position, op.Value);
                            expr.Left = CalcBinaryExpression(groups[i]);
                            current.Right = expr;
                            current = expr;
                        }
                    }
                    current.Right = CalcBinaryExpression(groups[separators.Count]);
                    return top;
                }
            }
        }

        private ScriptTreeNode CalcBinaryExpression(List<ScriptToken> group)
        {
            ScriptTreeNode top = null;
            ScriptTreeNode current = null;
            for (int i = 0; i < group.Count; i++)
            {
                ScriptTreeNode operand = GetLiteralOrID(group[i]);
                ScriptToken op = i < group.Count - 1 ? group[++i] : null;
                if (op == null)
                {
                    if (current == null)
                    {
                        current = operand;
                        top = current;
                    }
                    else
                    {
                        ((BinExprScriptTreeNode)current).Right = operand;
                    }
                }
                else if (op.IsBinaryOperator)
                {
                    BinExprScriptTreeNode expr = new BinExprScriptTreeNode(op.Position, op.Value);
                    expr.Left = operand;
                    if (current == null)
                    {
                        top = expr;
                    }
                    else
                    {
                        ((BinExprScriptTreeNode)current).Right = expr;
                    }
                    current = expr;
                }
                else
                {
                    ErrorInvalidTokenType(op);
                }
            }
            return top;
        }

        private ScriptTreeNode GetLiteralOrID(ScriptToken token)
        {
            if (token.Is(ScriptTokenType.ID))
            {
                IDScriptTreeNode node = new IDScriptTreeNode(token.Position, token.Value, _tree);
                if (token.Value.Contains(":") && !token.Value.StartsWith("var:"))
                {
                    node.MakeStatic();
                }
                return node;
            }
            else if (token.Is(ScriptTokenType.StringLiteral))
            {
                return new StringLiteralScriptTreeNode(token.Position, token.Value);
            }
            else if (token.Is(ScriptTokenType.FloatLiteral))
            {
                return new FloatLiteralScriptTreeNode(token.Position, float.Parse(token.Value));
            }
            else if (token.Is(ScriptTokenType.BoolLiteral))
            {
                return new BoolLiteralScriptTreeNode(token.Position, bool.Parse(token.Value));
            }
            else if (token.Is(ScriptTokenType.Null))
            {
                return new NullScriptTreeNode(token.Position);
            }
            else
            {
                ErrorInvalidTokenType(token);
                return null;
            }
        }

        private void ErrorInvalidTokenType(ScriptToken token)
        {
            Error("Invalid token type", "Invalid token type " + token.Type + " for expression", token.Position);
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

        private ScriptToken NextToken()
        {
            ScriptToken token = _tokens[_position];
            _position++;
            return token;
        }

        private ScriptToken PeekToken(int offset = 1)
        {
            ScriptToken token = _tokens[_position + offset];
            return token;
        }

        private void SkipToken(bool onlySkipIfBracket)
        {
            if (onlySkipIfBracket)
            {
                if (PeekToken(0).IsBracket)
                {
                    _position++;
                }
            }
            else
            {
                _position++;
            }
        }

        public ScriptParser()
        {
            _position = 0;
            _scope = new Stack<ScriptScriptTreeNode>();
            _scopeContext = new Stack<ScopeContext>();
        }
    }
}

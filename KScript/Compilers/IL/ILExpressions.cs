using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KScript.Compilers.IL
{
    public struct ILOperator
    {
        public string Symbol { get; private set; }
        public bool LeftAssociative { get; private set; }
        public int Precedence { get; private set; }

        public ILOperator(string symbol, bool leftAssociative, int precedence)
            : this()
        {
            Symbol = symbol;
            LeftAssociative = leftAssociative;
            Precedence = precedence;
        }
    }

    public partial class ILCompiler : Compiler
    {
        List<ILOperator> operatorTable;

        void CreateOperatorTable()
        {
            operatorTable = new List<ILOperator>();

            operatorTable.Add(new ILOperator("=", false, 1));
            operatorTable.Add(new ILOperator("|", true, 2));
            operatorTable.Add(new ILOperator("^", true, 3));
            operatorTable.Add(new ILOperator("&", true, 4));
            operatorTable.Add(new ILOperator("==", true, 5));
            operatorTable.Add(new ILOperator("!=", true, 5));
            operatorTable.Add(new ILOperator("<", true, 6));
            operatorTable.Add(new ILOperator(">", true, 6));
            operatorTable.Add(new ILOperator("<=", true, 6));
            operatorTable.Add(new ILOperator(">=", true, 6));
            operatorTable.Add(new ILOperator(">>", true, 7));
            operatorTable.Add(new ILOperator("<<", true, 7));
            operatorTable.Add(new ILOperator("+", true, 8));
            operatorTable.Add(new ILOperator("-", true, 8));
            operatorTable.Add(new ILOperator("*", true, 9));
            operatorTable.Add(new ILOperator("/", true, 9));
            operatorTable.Add(new ILOperator("%", true, 9));
        }

        string[] operators = new string[]
        {
            "+", "++", "-", "--", "*", "/", "%", "&", "|", "^", "!", "==", "!=", "<", "<=", ">", ">=", "=", "<<", ">>"
        };

        /// <summary>
        /// Parses an expression.
        /// </summary>
        /// <param name="logic">Determines if the expression should be wrapped in parentheses for IF statements and loops.</param>
        /// <returns>Returns true if the expression is successfully parsed. Otherwise, returns false.</returns>
        bool ParseExpression(bool logic)
        {
            Stack<string> stack = new Stack<string>();
            double temp;

            if (!GetNextToken())
            {
                Error("Expected expression.");
                return false;
            }

            do
            {
                if (logic & CurrentToken.Value != "(")
                {
                    Error("Logic expressions must be wrapped in parentheses.");
                    return false;
                }

                if (double.TryParse(CurrentToken.Value, out temp))
                {
                    AddAsm("pushi " + CurrentToken.Value);
                }
                else if (HasVariable(CurrentToken.Value))
                {
                    AddAsm("rmem a " + GetUniqueVariable(CurrentToken.Value));
                }
                else if (operators.Contains(CurrentToken.Value))
                {
                    // TODO: Do operator logic
                }
                else if (CurrentToken.Value == "(")
                {
                    stack.Push(CurrentToken.Value);
                }
                else if (CurrentToken.Value == ")")
                {
                    while ((stack.Count > 0) && (stack.Peek() != "("))
                    {
                        string op = stack.Pop();
                        switch (op)
                        {
                            case "+":
                                AddAsm("add");
                                break;

                            case "-":
                                AddAsm("sub");
                                break;

                            case "++":
                                AddAsm("incr");
                                break;

                            case "--":
                                AddAsm("decr");
                                break;

                            case "*":
                                AddAsm("mult");
                                break;

                            case "/":
                                AddAsm("div");
                                break;

                            case "%":
                                AddAsm("mod");
                                break;

                            case "&":
                                AddAsm("and");
                                break;

                            case "|":
                                AddAsm("or");
                                break;

                            case "!":
                                AddAsm("not");
                                break;
                                
                            case "^":
                                AddAsm("xor");
                                break;

                            case "<<":
                                AddAsm("lsh");
                                break;

                            case ">>":
                                AddAsm("rsh");
                                break;

                            case "==":
                                AddAsm("equal");
                                break;

                            case "!=":
                                AddAsm("nequal");
                                break;

                            case "<":
                                AddAsm("less");
                                break;

                            case ">":
                                AddAsm("great");
                                break;

                            case "<=":
                                AddAsm("lequal");
                                break;

                            case ">=":
                                AddAsm("gequal");
                                break;

                            default:
                                AddAsm("jmp " + op);
                                break;
                        }

                        if (stack.Count == 0)
                        {
                            Error("Parentheses mismatch.");
                            return false;
                        }
                    }
                    stack.Pop(); // pop the parethesis
                    
                }
                
            } while ((!logic & CurrentToken.Value == ";") | (logic & CurrentToken.Value == ")"));

            return true;
        }
    }
}

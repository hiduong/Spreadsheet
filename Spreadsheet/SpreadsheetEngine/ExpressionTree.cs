// NAME: Hien Duong
// WSUID: 11587750

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CptS321
{
    /// <summary>
    /// Expression tree class that parses and builds the tree for the expression.
    /// </summary>
    public class ExpressionTree
    {
        /// <summary>
        /// member variables.
        /// </summary>
        public HashSet<string> VariableNames = new HashSet<string>();
        private string expression;
        private string postfixExpression;
        private string[] parsedPostfixExpression;
        private Dictionary<string, double> variables = new Dictionary<string, double>();
        private ExpressionTreeNode root;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionTree"/> class.
        /// </summary>
        /// <param name="expression">expression to be parsed and built.</param>
        public ExpressionTree(string expression)
        {
            this.expression = expression;

            // convert the infix expression to a postfix expression
            this.postfixExpression = this.ConvertToPostfix();

            // parse the postifx expression by the white space
            this.parsedPostfixExpression = this.postfixExpression.Split(' ');
            this.root = this.BuildTree();
        }

        /// <summary>
        /// Sets the specified variable within the ExpressionTree variables dictionary.
        /// </summary>
        /// <param name="variableName">the name of the variable.</param>
        /// <param name="variableValue">the value of the variable.</param>
        public void SetVariable(string variableName, double variableValue)
        {
            this.variables[variableName] = variableValue;
        }

        /// <summary>
        /// Evaluates the expression.
        /// </summary>
        /// <returns>the value of the expression.</returns>
        public double Evaluate()
        {
            return this.root.Evaluate();
        }

        /// <summary>
        /// method to convert infix epression to post fix expression.
        /// </summary>
        /// <returns>the post fix string.</returns>
        private string ConvertToPostfix()
        {
            StringBuilder postfixString = new StringBuilder();
            Stack<char> postfixStack = new Stack<char>();

            /* Shunting Yard Algorithm:
               1.If the incoming symbols is an operand, print it..
               2.If the incoming symbol is a left parenthesis, push it on the stack.
               3.If the incoming symbol is a right parenthesis: discard the right parenthesis, pop and print the stack symbols until you see a left parenthesis. Pop the left parenthesis and discard it.
               4.If the incoming symbol is an operator and the stack is empty or contains a left parenthesis on top, push the incoming operator onto the stack.
               5.If the incoming symbol is an operator and has either higher precedence than the operator on the top of the stack, or has the same precedence as the operator on the top of the stack and is right associative -- push it on the stack.
               6.If the incoming symbol is an operator and has either lower precedence than the operator on the top of the stack, or has the same precedence as the operator on the top of the stack and is left associative -- continue to pop the stack until this is not true. Then, push the incoming operator.
               7.At the end of the expression, pop and print all operators on the stack. (No parentheses should remain.) */

            foreach (char character in this.expression)
            {
                // if operator
                if (character == '+' || character == '-' || character == '/' || character == '*' || character == '^')
                {
                    postfixString.Append(' ');
                    if (postfixStack.Count == 0 || postfixStack.Peek() == '(')
                    {
                        postfixStack.Push(character);
                    }
                    else if (this.GetPrecedence(character) >= this.GetPrecedence(postfixStack.Peek()))
                    {
                        while (postfixStack.Count != 0)
                        {
                            if (postfixStack.Peek() == '(')
                            {
                                break;
                            }
                            else if (this.GetPrecedence(character) < this.GetPrecedence(postfixStack.Peek()))
                            {
                                break;
                            }
                            else
                            {
                                postfixString.Append(postfixStack.Pop());
                                postfixString.Append(' ');
                            }
                        }

                        postfixStack.Push(character);
                    }
                    else
                    {
                        // if higher or same precedence push it onto the stack
                        if (this.GetPrecedence(character) == this.GetPrecedence(postfixStack.Peek()))
                        {
                            postfixString.Append(postfixStack.Pop());
                            postfixString.Append(' ');
                        }

                        postfixStack.Push(character);
                    }
                }
                else if (character == '(')
                {
                    postfixStack.Push(character);
                }
                else if (character == ')')
                {
                    while (postfixStack.Peek() != '(')
                    {
                        postfixString.Append(' ');
                        postfixString.Append(postfixStack.Pop());
                    }

                    postfixStack.Pop();
                }
                else if (character == ' ')
                {
                }
                else
                {
                    postfixString.Append(character);
                }
            }

            while (postfixStack.Count != 0)
            {
                postfixString.Append(' ');
                postfixString.Append(postfixStack.Pop());
            }

            return postfixString.ToString();
        }

        /// <summary>
        /// Method to return the precedence of a operator.
        /// </summary>
        /// <param name="character">the operator.</param>
        /// <returns>the precedence.</returns>
        private int GetPrecedence(char character)
        {
            if (character == '+')
            {
                return 7;
            }
            else if (character == '-')
            {
                return 7;
            }
            else if (character == '*')
            {
                return 6;
            }
            else if (character == '/')
            {
                return 6;
            }

            return -1;
        }

        /// <summary>
        /// Constructs the expression tree based on the post fix expression.
        /// </summary>
        private ExpressionTreeNode BuildTree()
        {
            Stack<ExpressionTreeNode> treeStack = new Stack<ExpressionTreeNode>();
            OperatorNodeFactory factory = new OperatorNodeFactory();
            foreach (string postFixString in this.parsedPostfixExpression)
            {
                /*If the symbol is an operator then pop the last two trees from the
                 stack and create a new tree with the operator as the root, last
                 element of the stack as left subtree, and the one before last element
                 of the stack as right subtree.Push this newly created tree to the stack. */
                if (this.CheckIfOperator(postFixString) == true)
                {
                    OperatorNode operatorNode = factory.CreateOperatorNode(postFixString);

                    // last element before last will be right.
                    if (treeStack.Count != 0)
                    {
                        operatorNode.Right = treeStack.Pop();
                    }

                    // last element will be left.
                    if (treeStack.Count != 0)
                    {
                        operatorNode.Left = treeStack.Pop();
                    }

                    // push it to the tree.
                    treeStack.Push(operatorNode);
                }
                else
                {
                    // If the symbol is an operand then create a tree with it and push it to the stack.
                    if (this.CheckIfConstant(postFixString) == true)
                    {
                        treeStack.Push(new ConstantNode(Convert.ToDouble(postFixString)));
                    }
                    else
                    {
                        if (!this.variables.ContainsKey(postFixString))
                        {
                            this.variables.Add(postFixString, 0.0);
                        }

                        this.VariableNames.Add(postFixString);
                        treeStack.Push(new VariableNode(postFixString, ref this.variables));
                    }
                }
            }

            return treeStack.Pop();
        }

        /// <summary>
        /// checks to see if the current string in question is an operator.
        /// </summary>
        /// <param name="currentString">the current string when building the tree.</param>
        /// <returns>true or false.</returns>
        private bool CheckIfOperator(string currentString)
        {
            if (currentString == "+" || currentString == "-" || currentString == "/" || currentString == "*")
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Will read through the whole string if it reaches the end and it doesn't contain [a-z][A-Z] its a constant.
        /// </summary>
        /// <param name="currentString">the current string when building the tree.</param>
        /// <returns>true or false.</returns>
        private bool CheckIfConstant(string currentString)
        {
            foreach (char character in currentString)
            {
                if (char.IsLetter(character) == true)
                {
                    return false;
                }
            }

            return true;
        }
    }
}

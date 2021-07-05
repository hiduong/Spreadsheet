// NAME: Hien Duong
// WSUID: 11587550
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CptS321;

namespace ExpressionTreeConsole
{
    /// <summary>
    /// Program where the expression tree console demo will run.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// main program where the expression tree demo runs.
        /// </summary>
        /// <param name="args">args.</param>
        public static void Main(string[] args)
        {
            int menuOption = 0;
            string stringMenuOption;
            string currentExpression = "A1+B2+12";
            string variableName;
            double variableValue = 0.0;

            ExpressionTree consoleDemo = new ExpressionTree(currentExpression);

            while (menuOption != 4)
            {
                Console.WriteLine("Menu (Current expression = \"" + currentExpression + "\")");
                Console.WriteLine("  1 = Enter a new expression");
                Console.WriteLine("  2 = set a variable value");
                Console.WriteLine("  3 = Evaluate tree");
                Console.WriteLine("  4 = Quit");

                stringMenuOption = Console.ReadLine();
                if (string.IsNullOrEmpty(stringMenuOption) == false)
                {
                    menuOption = Convert.ToInt32(stringMenuOption);
                }
                else
                {
                    menuOption = 0;
                }

                switch (menuOption)
                {
                    case 1:
                        Console.Write("Enter new expression : ");
                        currentExpression = Console.ReadLine();
                        consoleDemo = new ExpressionTree(currentExpression);
                        break;
                    case 2:
                        Console.Write("Enter variable name : ");
                        variableName = Console.ReadLine();
                        Console.Write("Enter variable value : ");
                        variableValue = Convert.ToDouble(Console.ReadLine());
                        consoleDemo.SetVariable(variableName, variableValue);
                        break;
                    case 3:
                        Console.WriteLine(consoleDemo.Evaluate().ToString());
                        break;
                    default:
                        break;
                }
            }
        }
    }
}

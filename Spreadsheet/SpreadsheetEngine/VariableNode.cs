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
    /// variable node of the tree.
    /// </summary>
    public class VariableNode : ExpressionTreeNode
    {
        /// <summary>
        /// member variables.
        /// </summary>
        private readonly string name;
        private Dictionary<string, double> variables;

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableNode"/> class.
        /// </summary>
        /// <param name="name">name of variable.</param>
        /// <param name="variables">a reference to the dictionary in ExpressionTree class.</param>
        public VariableNode(string name, ref Dictionary<string, double> variables)
        {
            this.name = name;
            this.variables = variables;
        }

        /// <summary>
        /// Variables Nodes evaluate method.
        /// </summary>
        /// <returns>return the value of the variable.</returns>
        public override double Evaluate()
        {
            double value = 0.0;
            if (this.variables.ContainsKey(this.name))
            {
                value = this.variables[this.name];
            }

            return value;
        }
    }
}

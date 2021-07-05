using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CptS321
{
    /// <summary>
    /// the constant node of the tree.
    /// </summary>
    public class ConstantNode : ExpressionTreeNode
    {
        /// <summary>
        /// member variables.
        /// </summary>
        private readonly double value;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantNode"/> class.
        /// </summary>
        /// <param name="value">value of the constantnode.</param>
        public ConstantNode(double value)
        {
            this.value = value;
        }

        /// <summary>
        /// evaluate method of constantnode.
        /// </summary>
        /// <returns>the value of the constant node.</returns>
        public override double Evaluate()
        {
            return this.value;
        }
    }
}

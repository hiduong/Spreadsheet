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
    /// a node in the expression tree.
    /// </summary>
    public abstract class ExpressionTreeNode
    {
        /// <summary>
        /// abstract evaluate method for nodes in the expression tree.
        /// </summary>
        /// <returns>a double.</returns>
        public abstract double Evaluate();
    }
}

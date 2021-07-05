// NAME: Hien Duong
// WSUID: 11587550

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CptS321
{
    /// <summary>
    /// Operator node of the tree.
    /// </summary>
    public abstract class OperatorNode : ExpressionTreeNode
    {
        /// <summary>
        /// Defining Associativity with enum for Right and Left.
        /// </summary>
        public enum Associative
        {
            /// <summary>
            /// Right side.
            /// </summary>
            Right,

            /// <summary>
            /// left side.
            /// </summary>
            Left
        }

        /// <summary>
        /// Gets or sets for the left node.
        /// </summary>
        public ExpressionTreeNode Left { get; set; }

        /// <summary>
        /// Gets or sets for the right node.
        /// </summary>
        public ExpressionTreeNode Right { get; set; }
    }
}

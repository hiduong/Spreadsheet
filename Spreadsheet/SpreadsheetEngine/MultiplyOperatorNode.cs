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
    /// multiplay operator node class for operator *.
    /// </summary>
    public class MultiplyOperatorNode : OperatorNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultiplyOperatorNode"/> class.
        /// </summary>
        public MultiplyOperatorNode()
        {
        }

        /// <summary>
        /// Gets the operator as *.
        /// </summary>
        public static string Operator => "*";

        /// <summary>
        /// Gets the precedence of the operator.
        /// </summary>
        public static ushort Precendence => 6;

        /// <summary>
        /// Gets the associativity as left.
        /// </summary>
        public static Associative Associativity => Associative.Left;

        /// <summary>
        /// Evaluation of the operator *.
        /// </summary>
        /// <returns>the added value of the left and right node.</returns>
        public override double Evaluate()
        {
            return this.Left.Evaluate() * this.Right.Evaluate();
        }
    }
}

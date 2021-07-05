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
    /// Minus operator class for the operator -.
    /// </summary>
    public class MinusOperatorNode : OperatorNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MinusOperatorNode"/> class.
        /// </summary>
        public MinusOperatorNode()
        {
        }

        /// <summary>
        /// Gets the operator as -.
        /// </summary>
        public static string Operator => "-";

        /// <summary>
        /// Gets the precedence of the operator.
        /// </summary>
        public static ushort Precendence => 7;

        /// <summary>
        /// Gets the associativity as left.
        /// </summary>
        public static Associative Associativity => Associative.Left;

        /// <summary>
        /// Evaluation of the operator -.
        /// </summary>
        /// <returns>the added value of the left and right node.</returns>
        public override double Evaluate()
        {
            return this.Left.Evaluate() - this.Right.Evaluate();
        }
    }
}

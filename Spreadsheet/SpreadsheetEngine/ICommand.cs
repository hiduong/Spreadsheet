using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CptS321
{
    /// <summary>
    /// Interface for the undo and redo commands.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// method to execute the command.
        /// </summary>
        void Execute();

        /// <summary>
        /// method to unexecute the command.
        /// </summary>
        void UnExecute();
    }
}

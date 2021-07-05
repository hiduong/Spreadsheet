using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CptS321
{
    /// <summary>
    /// Textcommand class for when we undo and redo the text of cell.
    /// </summary>
    public class TextCommand : ICommand
    {
        /// <summary>
        /// Data members.
        /// </summary>
        private string newText;
        private string oldText;
        private Cell changedCell;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextCommand"/> class.
        /// </summary>
        /// <param name="changedCell">the cell being changed.</param>
        /// <param name="newText">the new text we want to execute.</param>
        /// <param name="oldText">the old text we want to unexecute.</param>
        public TextCommand(Cell changedCell, string newText, string oldText)
        {
            this.changedCell = changedCell;
            this.newText = newText;
            this.oldText = oldText;
        }

        /// <summary>
        /// execute method that will change the text of the cell undo back to old text.
        /// </summary>
        public void Execute()
        {
            this.changedCell.Text = this.oldText;
        }

        /// <summary>
        /// unexecute method that will change the text of the cell back redo to new text.
        /// </summary>
        public void UnExecute()
        {
            this.changedCell.Text = this.newText;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CptS321
{
    /// <summary>
    /// ColorCommand class for when undo and redo of background colors.
    /// </summary>
    public class ColorCommand : ICommand
    {
        /// <summary>
        /// Data members.
        /// </summary>
        private uint newCellColor;
        private List<Cell> selectedCells;
        private List<uint> oldCellColors;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorCommand"/> class.
        /// </summary>
        /// <param name="newColor">the color of the cell we want to change to.</param>
        /// <param name="selectedCells">the selected cells we want to change the color of.</param>
        /// <param name="oldColors">the old colors of the selected cells.</param>
        public ColorCommand(uint newColor, List<Cell> selectedCells, List<uint> oldColors)
        {
            this.newCellColor = newColor;
            this.selectedCells = selectedCells;
            this.oldCellColors = oldColors;
        }

        /// <summary>
        /// execute method that will change the color of the cell undo changing back to old color.
        /// </summary>
        public void Execute()
        {
            for (int iterator = 0; iterator < this.selectedCells.Count; iterator++)
            {
                this.selectedCells[iterator].BGColor = this.oldCellColors[iterator];
            }
        }

        /// <summary>
        /// unexecute method that will change the color of the cell back redo changing back to new color.
        /// </summary>
        public void UnExecute()
        {
            foreach (Cell cell in this.selectedCells)
            {
                cell.BGColor = this.newCellColor;
            }
        }
    }
}

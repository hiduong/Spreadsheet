// NAME: Hien Duong
// WSU ID: 11587750

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Spreadsheet_Hien_Duong
{
    /// <summary>
    /// form class that initializes the rows and columns of the datagrid.
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>
        /// member variable spreadsheet object.
        /// </summary>
        private CptS321.Spreadsheet spreadSheet;

        /// <summary>
        /// Initializes a new instance of the <see cref="Form1"/> class.
        /// </summary>
        public Form1()
        {
            List<string> columnHeader = new List<string> { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            this.InitializeComponent();

            // initializing object;
            this.spreadSheet = new CptS321.Spreadsheet(50, 26);
            this.spreadSheet.PropertyChanged += this.CellPropertyChanged;
            this.spreadSheet.Invoker.PropertyChanged += this.CellPropertyChanged;
            this.undoToolStripMenuItem.Enabled = false;
            this.redoToolStripMenuItem.Enabled = false;

            // Programatically Initializing the Columns in the data grid.
            foreach (string name in columnHeader)
            {
                this.dataGridView1.Columns.Add(name, name);
            }

            // Programatically Initializing the Rows in the data grid.
            for (int iterator = 0; iterator < 50; iterator++)
            {
                int increment = iterator + 1;
                this.dataGridView1.Rows.Add();
                this.dataGridView1.Rows[iterator].HeaderCell.Value = increment.ToString();
            }

            this.dataGridView1.CellBeginEdit += this.DataGridView1_CellBeginEdit;
            this.dataGridView1.CellEndEdit += this.DataGridView1_CellEndEdit;
        }

        /// <summary>
        /// Tells subscribers what to do when the cell property has been changed.
        /// </summary>
        /// <param name="sender">sender object.</param>
        /// <param name="e">Property changed event args.</param>
        private void CellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CptS321.Cell changedCell = sender as CptS321.Cell;

            switch (e.PropertyName)
            {
                case "Value":
                    this.dataGridView1.Rows[changedCell.RowIndex].Cells[changedCell.ColumnIndex].Value = changedCell.Value;
                    break;
                case "Empty Redo Stack":
                    this.redoToolStripMenuItem.Enabled = false;
                    this.redoToolStripMenuItem.Text = "Redo";
                    break;
                case "Empty Undo Stack":
                    this.undoToolStripMenuItem.Enabled = false;
                    this.undoToolStripMenuItem.Text = "Undo";
                    break;
                case "Undo Stack Not Empty":
                    this.undoToolStripMenuItem.Enabled = true;
                    this.undoToolStripMenuItem.Text = "Undo " + this.spreadSheet.Invoker.GetUndoMessage();
                    if (this.spreadSheet.Invoker.GetRedoMessage() != null)
                    {
                        this.redoToolStripMenuItem.Text = "Redo " + this.spreadSheet.Invoker.GetRedoMessage();
                    }

                    break;
                case "Redo Stack Not Empty":
                    this.redoToolStripMenuItem.Enabled = true;
                    this.redoToolStripMenuItem.Text = "Redo " + this.spreadSheet.Invoker.GetRedoMessage();
                    if (this.spreadSheet.Invoker.GetUndoMessage() != null)
                    {
                        this.undoToolStripMenuItem.Text = "Undo " + this.spreadSheet.Invoker.GetUndoMessage();
                    }

                    break;
                case "Color":
                    this.dataGridView1.Rows[changedCell.RowIndex].Cells[changedCell.ColumnIndex].Style.BackColor = Color.FromArgb((int)changedCell.BGColor);
                    break;
            }
        }

        /// <summary>
        /// Method to perfrom the demo.
        /// </summary>
        /// <param name="sender">sender object.</param>
        /// <param name="e">arguments for the event.</param>
        private void Button1_Click(object sender, EventArgs e)
        {
           string oldValue;
           Random rnd = new Random();
           int rndRow = 0;
           int rndCol = 0;
           for (int iteratorDemo1 = 0; iteratorDemo1 < 50; iteratorDemo1++)
           {
               // setting column B values to be "This is cell B#" where # is the row.
               oldValue = this.spreadSheet.GetCell(iteratorDemo1, 0).Text;
               this.spreadSheet.GetCell(iteratorDemo1, 1).Text = "This is cell B" + (iteratorDemo1 + 1).ToString();
               this.spreadSheet.Invoker.AddUndo(new CptS321.TextCommand(this.spreadSheet.GetCell(iteratorDemo1, 0), "=B" + (iteratorDemo1 + 1).ToString(), oldValue), "cell text change");
            }

           for (int iteratorDemo1 = 0; iteratorDemo1 < 50; iteratorDemo1++)
           {
               // setting column A values to be "=B#" where # is the row.
               oldValue = this.spreadSheet.GetCell(iteratorDemo1, 0).Text;
               this.spreadSheet.GetCell(iteratorDemo1, 0).Text = "=B" + (iteratorDemo1 + 1).ToString();
               this.spreadSheet.Invoker.AddUndo(new CptS321.TextCommand(this.spreadSheet.GetCell(iteratorDemo1, 0), "=B" + (iteratorDemo1 + 1).ToString(), oldValue), "cell text change");
            }

           for (int iteratorDemo1 = 0; iteratorDemo1 < 50; iteratorDemo1++)
           {
                // setting the text "Hello World!" in random cells from column C-Z and rows 1-50.
               rndRow = rnd.Next(0, 50);
               rndCol = rnd.Next(2, 26);
               oldValue = this.spreadSheet.GetCell(rndRow, rndCol).Text;
               this.spreadSheet.GetCell(rndRow, rndCol).Text = "Hello World!";
               this.spreadSheet.Invoker.AddUndo(new CptS321.TextCommand(this.spreadSheet.GetCell(iteratorDemo1, 0), "Hello World!", oldValue), "cell text change");
            }

           this.undoToolStripMenuItem.Enabled = true;
           this.undoToolStripMenuItem.Text = "Undo cell text change";
        }

        /// <summary>
        /// Event handler for when user begins to edit the cell.
        /// </summary>
        /// <param name="sender">sender object.</param>
        /// <param name="e">the cell.</param>
        private void DataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            // when the user begins to edit the cell we will set the value of the cell to be the text of the cell.
            this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = this.spreadSheet.GetCell(e.RowIndex, e.ColumnIndex).Text;
        }

        /// <summary>
        /// event handler for when the user is done editing the cell.
        /// </summary>
        /// <param name="sender">the sender object.</param>
        /// <param name="e">the cell.</param>
        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            string cellValue;
            string oldValue;

            // try catch to catch any exceptions, if there are any, just set the celll value to empty;
            try
            {
                cellValue = this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            }
            catch
            {
                cellValue = string.Empty;
            }

            if (cellValue == this.spreadSheet.GetCell(e.RowIndex, e.ColumnIndex).Text)
            {
                this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = this.spreadSheet.GetCell(e.RowIndex, e.ColumnIndex).Value;
            }
            else
            {
                this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = this.spreadSheet.GetCell(e.RowIndex, e.ColumnIndex).Value;
                oldValue = this.spreadSheet.GetCell(e.RowIndex, e.ColumnIndex).Text;
                this.spreadSheet.GetCell(e.RowIndex, e.ColumnIndex).Text = cellValue;
                this.spreadSheet.Invoker.AddUndo(new CptS321.TextCommand(this.spreadSheet.GetCell(e.RowIndex, e.ColumnIndex), cellValue, oldValue), "cell text change");
                this.undoToolStripMenuItem.Enabled = true;
                this.undoToolStripMenuItem.Text = "Undo cell text change";
            }
        }

        private void UndoToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            this.spreadSheet.Invoker.Undo();
        }

        private void RedoToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            this.spreadSheet.Invoker.Redo();
        }

        private void ChangeBackgroundColorToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            ColorDialog myDialog = new ColorDialog();
            List<uint> oldColors = new List<uint>();
            List<CptS321.Cell> selectedCells = new List<CptS321.Cell>();

            // Keeps the user from selecting a custom color.
            myDialog.AllowFullOpen = true;

            // Update the text box color if the user clicks OK
            if (myDialog.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < this.dataGridView1.SelectedCells.Count; i++)
                {
                    // get the cell's that are changing and change the BGColor property which will fire property changed and will update all the cells.
                    oldColors.Add(this.spreadSheet.GetCell(this.dataGridView1.SelectedCells[i].RowIndex, this.dataGridView1.SelectedCells[i].ColumnIndex).BGColor);

                    selectedCells.Add(this.spreadSheet.GetCell(this.dataGridView1.SelectedCells[i].RowIndex, this.dataGridView1.SelectedCells[i].ColumnIndex));

                    this.spreadSheet.GetCell(this.dataGridView1.SelectedCells[i].RowIndex, this.dataGridView1.SelectedCells[i].ColumnIndex).BGColor = (uint)myDialog.Color.ToArgb();
                }

                this.spreadSheet.Invoker.AddUndo(new CptS321.ColorCommand((uint)myDialog.Color.ToArgb(), selectedCells, oldColors), "changing cell background color");
                this.undoToolStripMenuItem.Enabled = true;
                this.undoToolStripMenuItem.Text = "Undo changing cell background color";
            }
        }

        /// <summary>
        /// Method to save the cell information to the files.
        /// </summary>
        /// <param name="sender">the sender.</param>
        /// <param name="e">event args e.</param>
        private void SaveToFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stream myStream;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            // Set filter options and filter index.
            saveFileDialog1.Filter = "XML Files (*.xml)|*.xml";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;

            // Process input if the user clicked OK.
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = saveFileDialog1.OpenFile()) != null)
                {
                    // save the cells to xml file
                    this.spreadSheet.Save(myStream);

                    // close the stream
                    myStream.Dispose();
                    myStream.Close();
                }
            }
        }

        /// <summary>
        /// Method to load the cells from the xml file.
        /// </summary>
        /// <param name="sender">the sender.</param>
        /// <param name="e">even args e.</param>
        private void LoadFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            // Set filter options and filter index.
            openFileDialog1.Filter = "XML Files (*.xml)|*.xml";
            openFileDialog1.FilterIndex = 1;

            // Process input if the user clicked OK.
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // Open the selected file to read.
                Stream fileStream = openFileDialog1.OpenFile();

                // load the cells from the xml file.
                this.spreadSheet.Load(fileStream);

                // close the stream
                fileStream.Dispose();
                fileStream.Close();
            }
        }
    }
}

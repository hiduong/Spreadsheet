// NAME: Hien Duong
// WSU ID: 11587750

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace CptS321
{
    /// <summary>
    /// Spreadsheet class with methods to create the spreadsheet and obtain cells.
    /// </summary>
    public class Spreadsheet
    {
        /// <summary>
        /// Instance of undo redo.
        /// </summary>
        public UndoRedo Invoker = new UndoRedo();

        /// <summary>
        /// private member varibles.
        /// </summary>
        private int rowCount;
        private int columnCount;
        private SpreadsheetCell[,] cellArray; // 2d array of cells.
        private Dictionary<Cell, HashSet<string>> referenceDictionary = new Dictionary<Cell, HashSet<string>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Spreadsheet"/> class.
        /// </summary>
        /// <param name="numberRows">number of rows the spreadsheet will have.</param>
        /// <param name="numberColumns">number of columns the spreadsheet will have.</param>
        public Spreadsheet(int numberRows, int numberColumns)
        {
            this.rowCount = numberRows;
            this.columnCount = numberColumns;

            this.cellArray = new SpreadsheetCell[numberRows, numberColumns];

            for (int iteratorRows = 0; iteratorRows < numberRows; iteratorRows++)
            {
                for (int iteratorColumns = 0; iteratorColumns < numberColumns; iteratorColumns++)
                {
                    this.cellArray[iteratorRows, iteratorColumns] = new SpreadsheetCell(iteratorRows, iteratorColumns);
                    this.cellArray[iteratorRows, iteratorColumns].PropertyChanged += this.CellPropertyChanged;
                }
            }
        }

        /// <summary>
        /// Declaration of the event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets Property to return the number of rows in the spreadsheet.
        /// </summary>
        public int RowCount
        {
            get { return this.rowCount; }
        }

        /// <summary>
        /// Gets Property to return the number off columns in the spreadsheet.
        /// </summary>
        public int ColumnCount
        {
            get { return this.columnCount; }
        }

        /// <summary>
        /// Obtains the cell at given row index and column index, if it doesn't exists return null.
        /// </summary>
        /// <param name="rowIndex">the index where cell lies in the rows.</param>
        /// <param name="columnIndex">the index where cell lies in the columns.</param>
        /// <returns>null or cell.</returns>
        public Cell GetCell(int rowIndex, int columnIndex)
        {
            if (rowIndex > (this.rowCount - 1) || rowIndex < 0 || columnIndex > (this.columnCount - 1) || columnIndex < 0)
            {
                return null;
            }

            return this.cellArray[rowIndex, columnIndex];
        }

        /// <summary>
        /// Method to save the cells to an xml file.
        /// </summary>
        /// <param name="xmlFile">the xml file.</param>
        public void Save(Stream xmlFile)
        {
            // create a xml writer with the stream
            XmlWriter writer = XmlWriter.Create(xmlFile);

            // write the first element, spreadsheet
            writer.WriteStartElement("spreadsheet");

            // go through all the cells
            for (int iteratorRows = 0; iteratorRows < this.rowCount; iteratorRows++)
            {
                for (int iteratorColumns = 0; iteratorColumns < this.columnCount; iteratorColumns++)
                {
                    // if a cell property is not default, then we will write to the xml file
                    if ((this.GetCell(iteratorRows, iteratorColumns).Text != string.Empty) || (this.GetCell(iteratorRows, iteratorColumns).BGColor != 0xFFFFFFFF))
                    {
                        // write the cell tag.
                        writer.WriteStartElement("cell");

                        // write the attribute name for the cell tag where name is the name of the cell
                        writer.WriteAttributeString("name", this.GetNameofCell(this.GetCell(iteratorRows, iteratorColumns)));

                        // write the text tag, the contents of the text tag is the text of the cell
                        writer.WriteStartElement("text");
                        writer.WriteString(this.GetCell(iteratorRows, iteratorColumns).Text);
                        writer.WriteEndElement();

                        // write the bgcolor tag, the contents of the tag is the uint value of the cell converted into a string
                        writer.WriteStartElement("bgcolor");
                        writer.WriteString(this.GetCell(iteratorRows, iteratorColumns).BGColor.ToString());
                        writer.WriteEndElement();

                        writer.WriteEndElement();
                    }
                }
            }

            writer.WriteEndElement();

            // dispose and close the writer
            writer.Dispose();
            writer.Close();
        }

        /// <summary>
        /// Method to load the cells from the xml file.
        /// </summary>
        /// <param name="xmlFile">the xml file.</param>
        public void Load(Stream xmlFile)
        {
            // clear the undo and redo stacks
            this.Invoker.ClearRedo();
            this.Invoker.ClearUndo();

            // clear all the cells and set them back to original value
            this.Clear();

            // create the xdocument with the stream to read the xml file
            XDocument reader = XDocument.Load(xmlFile);

            // go through all the elements in each cell element
            foreach (XElement elements in reader.Root.Elements("cell"))
            {
                // get the cell from the name
                Cell currentCell = this.GetCellFromName(elements.Attribute("name").Value);

                // if the cell is not null
                if (currentCell != null)
                {
                    // set the text and the bgcolor
                    currentCell.Text = elements.Element("text").Value;
                    currentCell.BGColor = uint.Parse(elements.Element("bgcolor").Value);
                }
            }
        }

        /// <summary>
        /// Tells subscribers what to do when the cell property has been changed.
        /// </summary>
        /// <param name="sender">sender object.</param>
        /// <param name="e">Property changed event args.</param>
        private void CellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Cell changedCell = sender as Cell;
            try
            {
                if (e.PropertyName == "Text")
                {
                    // if the changed cell is in the reference dictionary delete it.
                    this.referenceDictionary.Remove(changedCell);

                    // case for if the user want's to delete the value in the cell.
                    if (string.IsNullOrEmpty(changedCell.Text))
                    {
                        changedCell.Value = null;
                        this.PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs("Value"));
                        this.UpdateReference(this.GetNameofCell(changedCell));
                        return;
                    }

                    // if Text starts with = we will pull the text value from another cell.
                    // else we will just set the text of the cell.
                    if (changedCell.Text[0] == '=')
                    {
                        // calls helper function to handle formulas
                        this.HandleCellFormula(changedCell, 0);
                    }
                    else
                    {
                        // if not a formula just set the value to be the text
                        changedCell.Value = changedCell.Text;
                    }

                    // set the value and update all the references
                    this.PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs("Value"));
                    this.UpdateReference(this.GetNameofCell(changedCell));
                }
                else if (e.PropertyName == "Color")
                {
                    // do nothing send it off to the ui
                    this.PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs("Color"));
                }
            }
            catch
            {
                // catch any error and set the value to be #ERROR! just like the microsoft spreadsheet
                changedCell.Value = "#ERROR!";
                this.PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs("Value"));
                this.UpdateReference(this.GetNameofCell(changedCell));
            }
        }

        /// <summary>
        /// handles the setting and update of the formula for the cell.
        /// </summary>
        /// <param name="changedCell">the cell being changed.</param>
        /// <param name="updateFlag">the flag that deteremins if we are updating the cell or adding next values to it, 0 == add.</param>
        private void HandleCellFormula(Cell changedCell, int updateFlag)
        {
            // if it's just '=' then the value is '='
            if (changedCell.Text.Length == 1)
            {
                changedCell.Value = changedCell.Text;
            }
            else
            {
                // get the expression so remove the equal in the beginning
                string expression = changedCell.Text.Substring(1);

                // checks to see if the expression contains operators
                if (this.ContainsOperators(expression) == true && changedCell.Text.Length > 2)
                {
                    ExpressionTree tree = new ExpressionTree(expression);

                    // Search through the variable map
                    foreach (string variable in tree.VariableNames)
                    {
                        // if the changed cell is referencing itself raise error message
                        if (changedCell == this.GetCellFromName(variable))
                        {
                            changedCell.Value = "#SELFREF!";
                            return;
                        }

                        // if there is a circular reference raise error message
                        if (this.CircularReferenceExists(this.GetNameofCell(changedCell), variable) == true)
                        {
                            changedCell.Value = "#CIRCULARREF!";
                            return;
                        }

                        // try to pull the variable values
                        try
                        {                 
                            // if we try to pull any of the error messages then the changedcell will also contain that error message
                            string pulledValue = this.PullValueFromCell(variable);
                            if (pulledValue == "#DIV/0!" || pulledValue == "#NAME?" || pulledValue == "#VALUE!" || pulledValue == "#ERROR!" || pulledValue == "#SELFREF!" || pulledValue == "#CIRCULARREF!")
                            {
                                changedCell.Value = pulledValue;
                                return;
                            }
                            else if (pulledValue == null)
                            {
                                // // if pulled value is not set
                                // the microsoft spreadsheet has this value set to 0 if it's not set
                                tree.SetVariable(variable, double.Parse("0"));
                            }
                            else if (string.IsNullOrEmpty(pulledValue))
                            {
                                // if we get an empty return then variable is not a cell
                                changedCell.Value = "#NAME?";
                                return;
                            }
                            else
                            {
                                // else we will set it
                                tree.SetVariable(variable, double.Parse(pulledValue));
                            }
                        }
                        catch (FormatException)
                        {
                            // catching any format exceptions and setting the cell to value
                            this.referenceDictionary[changedCell] = tree.VariableNames;
                            changedCell.Value = "#VALUE!";
                            return;
                        }
                    }

                    // Evaluate the exppression
                    double evaluatedValue = tree.Evaluate();

                    // checks to see if we divided by 0
                    if (evaluatedValue == double.PositiveInfinity)
                    {
                        // prints the correct error message
                        changedCell.Value = "#DIV/0!";
                    }
                    else
                    {
                        // else convert it to string and set the value
                        changedCell.Value = evaluatedValue.ToString();
                    }

                    // add's the reference to the dictionary.
                    if (updateFlag == 0)
                    {
                        this.referenceDictionary[changedCell] = tree.VariableNames;
                    }
                }
                else
                {
                    // if expression does not contain any operators
                    // pull the value

                    // if the changed cell is referencing itself raise error message
                    if (changedCell == this.GetCellFromName(expression))
                    {
                        changedCell.Value = "#SELFREF!";
                        return;
                    }

                    // if there is a circular reference raise error message
                    if (this.CircularReferenceExists(this.GetNameofCell(changedCell), expression) == true)
                    {
                        changedCell.Value = "#CIRCULARREF!";
                        return;
                    }

                    string pulledValue = this.PullValueFromCell(expression);

                    if (string.IsNullOrEmpty(pulledValue))
                    {
                        // check if the expression is a constant or variable
                        if (this.CheckIfConstant(expression) == true)
                        {
                            changedCell.Value = expression;
                        }
                        else if (pulledValue == null)
                        {
                            changedCell.Value = "0";
                            if (updateFlag == 0)
                            {
                                this.referenceDictionary[changedCell] = new HashSet<string>() { expression };
                            }
                        }
                        else if (string.IsNullOrEmpty(pulledValue))
                        {
                            // if we get an empty return then variable is not a cell
                            changedCell.Value = "#NAME?";
                        }
                    }
                    else
                    {
                        changedCell.Value = pulledValue;

                        // add reference to dictionary
                        if (updateFlag == 0)
                        {
                            this.referenceDictionary[changedCell] = new HashSet<string>() { expression };
                        }
                    }
                }
            }
        }

        /// <summary>
        /// updates all the cells that reference the changedCell.
        /// Algorithm is to search through a dictionary of all the references made.
        /// the key being the cell that references and the the value being the referenced cells.
        /// if we find the cell that is changing in the referenced cells we will updated the cell that references it.
        /// then it will recusively call the functoin to do the same for itself.
        /// </summary>
        /// <param name="changedCell">The changed cell.</param>
        private void UpdateReference(string changedCell)
        {
            // Say we have A1=A2+A3 and the cell being changed is A2
            // the dictionary should have key: A1 value: [A2,A3]
            // we will go through the keys, if we find A2
            // we have to update the value of the key, A1
            foreach (KeyValuePair<Cell, HashSet<string>> currentPair in this.referenceDictionary)
            {
                if (currentPair.Value.Contains(changedCell))
                {
                    this.HandleCellFormula(currentPair.Key, 2);
                    this.PropertyChanged?.Invoke(currentPair.Key, new PropertyChangedEventArgs("Value"));
                    this.UpdateReference(this.GetNameofCell(currentPair.Key));
                }
            }
        }

        /// <summary>
        /// Method to determine if there is a circular reference.
        /// </summary>
        /// <returns>returns true if there is and false if there isn't.</returns>
        private bool CircularReferenceExists(string changedCell, string referencedCell)
        {
            Cell referenced = this.GetCellFromName(referencedCell);

            //A4 = A1
            //A1 = A2
            //A2 = A3+A4

            // Algorithm:
            // A3 = A1, A3 is referencing A1
            // A1 = A2, A1:[A2], See if A1 is a key, if it is, recursively call the function again with A3, A2
            // A2 = A3, A2:[A3], See if A2 is a key, if it is, we will return true because the value A3 == the changed cell, therefore there is a circular reference

            // if the referenced Cell is in the reference dictionary as a key
            if (referenced != null)
            {
                if (this.referenceDictionary.ContainsKey(referenced))
                {
                    // go through all the references
                    foreach (string name in this.referenceDictionary[referenced])
                    {
                        // if the reference is the same as the changed cell there is a circular reference.
                        if (name == changedCell)
                        {
                            // return true
                            return true;
                        }
                        else
                        {
                            // recursively call the function with the name.
                            if (this.CircularReferenceExists(changedCell, name) == true)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Obtains the pulled cell's text.
        /// </summary>
        /// <param name="cell">the cell to be pulled.</param>
        /// <returns>the text of the pulled cell.</returns>
        private string PullValueFromCell(string cell)
        {
            int iteratorColumn = 0;
            List<char> columnHeader = new List<char> { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            foreach (char columnIndex in columnHeader)
            {
                if (cell[0] == columnIndex)
                {
                    // catching any exceptions and just returning empty.
                    try
                    {
                        // if B2 else B29
                        if (cell.Length == 2)
                        {
                            return this.GetCell(int.Parse(cell[1].ToString()) - 1, iteratorColumn).Value;
                        }
                        else if (cell.Length == 3)
                        {
                            return this.GetCell(int.Parse(cell.Substring(1)) - 1, iteratorColumn).Value;
                        }
                    }
                    catch
                    {
                        return string.Empty;
                    }
                }

                iteratorColumn++;
            }

            return string.Empty;
        }

        /// <summary>
        /// function to see if an expression contains operators.
        /// </summary>
        /// <param name="expression">the cell.</param>
        /// <returns>true or false.</returns>
        private bool ContainsOperators(string expression)
        {
            if (expression.Contains("+") || expression.Contains("-") || expression.Contains("/") || expression.Contains("*"))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Will read through the whole string if it reaches the end and it doesn't contain [a-z][A-Z] its a constant.
        /// </summary>
        /// <param name="currentString">the current string when building the tree.</param>
        /// <returns>true or false.</returns>
        private bool CheckIfConstant(string currentString)
        {
            foreach (char character in currentString)
            {
                if (char.IsLetter(character) == true || char.IsSymbol(character) == true || char.IsPunctuation(character) == true)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets the name of a cell.
        /// </summary>
        /// <param name="cell">the cell we want to name of.</param>
        /// <returns>the name of the cell.</returns>
        private string GetNameofCell(Cell cell)
        {
            StringBuilder cellName = new StringBuilder();
            List<char> columnHeader = new List<char> { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            cellName.Append(columnHeader[cell.ColumnIndex]);
            cellName.Append(cell.RowIndex + 1);
            return cellName.ToString(); 
        }

        /// <summary>
        /// method returns the cell given the name of the cell.
        /// </summary>
        /// <param name="name">the name of the cell.</param>
        /// <returns>the cell corresponding to that name.</returns>
        private Cell GetCellFromName(string name)
        {
            // go through all the cells
            List<string> columnHeader = new List<string> { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            for (int iteratorRows = 0; iteratorRows < this.rowCount; iteratorRows++)
            {
                for (int iteratorColumns = 0; iteratorColumns < this.columnCount; iteratorColumns++)
                {
                    // if the current cell matches the cell we want
                    if ((columnHeader[iteratorColumns] + (iteratorRows + 1).ToString()) == name)
                    {
                        // return that cell
                        return this.GetCell(iteratorRows, iteratorColumns);
                    }
                }
            }

            // else return null
            return null;
        }

        /// <summary>
        /// method to clear all the text and bg color of the cells and set them to default.
        /// </summary>
        private void Clear()
        {
            // go through all the cells
            for (int iteratorRows = 0; iteratorRows < this.rowCount; iteratorRows++)
            {
                for (int iteratorColumns = 0; iteratorColumns < this.columnCount; iteratorColumns++)
                {
                    // set the text and color back to their default values
                    this.GetCell(iteratorRows, iteratorColumns).Text = string.Empty;
                    this.GetCell(iteratorRows, iteratorColumns).BGColor = 0xFFFFFFFF;
                }
            }
        }
    }
}

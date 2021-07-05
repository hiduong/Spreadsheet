// NAME: Hien Duong
// WSU ID: 11587750

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CptS321
{
    /// <summary>
    /// abstract class representing a cell.
    /// </summary>
    public abstract class Cell : INotifyPropertyChanged
    {
        /// <summary>
        /// protected member variable text for the text in the cell.
        /// </summary>
        protected string text = string.Empty;

        /// <summary>
        /// protected member variable value for evaluating the value in the cell.
        /// </summary>
        protected string value;

        /// <summary>
        /// private member variables for location of the cell inn the 2d Array.
        /// </summary>
        private readonly int rowIndex;
        private readonly int columnIndex;

        /// <summary>
        /// private member variable value to change the background color of the cell.
        /// </summary>
        private uint bgColor = 0xFFFFFFFF;

        /// <summary>
        /// Initializes a new instance of the <see cref="Cell"/> class.
        /// </summary>
        /// <param name="rowIndex">The row index of the cell.</param>
        /// <param name="columnIndex">The column index of the cell.</param>
        public Cell(int rowIndex, int columnIndex)
        {
            this.rowIndex = rowIndex;
            this.columnIndex = columnIndex;
        }

        /// <summary>
        /// Declaration of the event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets read only property that returns the cell's row index.
        /// </summary>
        public int RowIndex
        {
            get { return this.rowIndex; }
        }

        /// <summary>
        /// Gets read only property that returns the cell's column index.
        /// </summary>
        public int ColumnIndex
        {
            get { return this.columnIndex; }
        }

        /// <summary>
        /// Gets or sets the text of the cell.
        /// </summary>
        public string Text
        {
            get
            {
                return this.text;
            }

            set
            {
                if (value != this.text)
                {
                    this.text = value;
                    this.OnPropertyChanged("Text");
                }
            }
        }

        /// <summary>
        /// Gets the evaluated value of the cell.
        /// </summary>
        public string Value
        {
            get
            {
                return this.value;
            }

            internal set
            {
                this.value = value;
                this.OnPropertyChanged("Value");
            }
        }

        /// <summary>
        /// Gets or sets the background color of the cell.
        /// </summary>
        public uint BGColor
        {
            get
            {
                return this.bgColor;
            }

            set
            {
                this.bgColor = value;
                this.OnPropertyChanged("Color");
            }
        }

        /// <summary>
        /// Method to notify that the property has changed.
        /// https://docs.microsoft.com/en-us/dotnet/framework/wpf/data/how-to-implement-property-change-notification?redirectedfrom=MSDN.
        /// </summary>
        /// <param name="text">Name of the member that was changed.</param>
        protected void OnPropertyChanged(string text)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(text));
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CptS321
{
    /// <summary>
    /// class implements the undo redo.
    /// </summary>
    public class UndoRedo : INotifyPropertyChanged
    {
        /// <summary>
        /// the redo and undo stack.
        /// </summary>
        private Stack<KeyValuePair<string, ICommand>> redoStack = new Stack<KeyValuePair<string, ICommand>>();
        private Stack<KeyValuePair<string, ICommand>> undoStack = new Stack<KeyValuePair<string, ICommand>>();

        /// <summary>
        /// Declaration of the event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Method to clear the undo stack.
        /// </summary>
        public void ClearRedo()
        {
            this.redoStack.Clear();
            this.OnPropertyChanged("Empty Redo Stack");
        }

        /// <summary>
        /// Method to clear the redo stack.
        /// </summary>
        public void ClearUndo()
        {
            this.undoStack.Clear();
            this.OnPropertyChanged("Empty Undo Stack");
        }

        /// <summary>
        /// method to push the command onto the stack.
        /// </summary>
        /// <param name="command">the command.</param>
        /// <param name="commandType">the command message.</param>
        public void AddRedo(ICommand command, string commandType)
        {
            this.redoStack.Push(new KeyValuePair<string, ICommand>(commandType, command));
        }

        /// <summary>
        /// method to push the command onto the stack.
        /// </summary>
        /// <param name="command">the command.</param>
        /// <param name="commandType">the command message.</param>
        public void AddUndo(ICommand command, string commandType)
        {
            this.undoStack.Push(new KeyValuePair<string, ICommand>(commandType, command));
        }

        /// <summary>
        /// executes the top element in the undo stack.
        /// </summary>
        public void Undo()
        {
            if (this.undoStack.Count != 0)
            {
                this.undoStack.Peek().Value.Execute();
                this.redoStack.Push(this.undoStack.Pop());

                if (this.undoStack.Count == 0)
                {
                    this.OnPropertyChanged("Empty Undo Stack");
                }

                this.OnPropertyChanged("Redo Stack Not Empty");
            }
        }

        /// <summary>
        /// executes the top element in the redo stack.
        /// </summary>
        public void Redo()
        {
            if (this.redoStack.Count != 0)
            {
                this.redoStack.Peek().Value.UnExecute();
                this.undoStack.Push(this.redoStack.Pop());

                if (this.redoStack.Count == 0)
                {
                    this.OnPropertyChanged("Empty Redo Stack");
                }

                this.OnPropertyChanged("Undo Stack Not Empty");
            }
        }

        /// <summary>
        /// returns the the message indicating what is being changed.
        /// </summary>
        /// <returns>the message from the top of redo stack.</returns>
        public string GetRedoMessage()
        {
            if (this.redoStack.Count != 0)
            {
                return this.redoStack.Peek().Key;
            }

            return null;
        }

        /// <summary>
        /// returns the the message indicating what is being changed.
        /// </summary>
        /// <returns>the message from the top of redo stack.</returns>
        public string GetUndoMessage()
        {
            if (this.undoStack.Count != 0)
            {
                return this.undoStack.Peek().Key;
            }

            return null;
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

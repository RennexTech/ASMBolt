using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMBolt.Editor
{
    /// <summary>
    /// Provides data for the <see cref="CodeEditor.TextChanged"/> event.
    /// </summary>
    internal class TextChangedEventArgs : EventArgs
    {
        #region Fields
        private string text;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the text of the editor after the change.
        /// </summary>
        public string Text
        {
            get { return text; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TextChangedEventArgs"/> class.
        /// </summary>
        /// <param name="text">The text of the editor after the change.</param>
        public TextChangedEventArgs(string text)
        {
            this.text = text;
        }
        #endregion
    }
}
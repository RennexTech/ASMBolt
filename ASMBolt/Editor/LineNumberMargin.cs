using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ASMBolt.Editor
{
    /// <summary>
    /// Represents the line number margin for the CodeEditor control.
    /// </summary>
    internal class LineNumberMargin
    {
        #region Fields

        private CodeEditor parentEditor;
        private int width;
        private Color backgroundColor;
        private Color foregroundColor;
        private Font font;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the parent CodeEditor control.
        /// </summary>
        public CodeEditor ParentEditor
        {
            get { return parentEditor; }
            set { parentEditor = value; }
        }

        /// <summary>
        /// Gets the width of the line number margin.
        /// </summary>
        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        /// <summary>
        /// Gets or sets the background color of the line number margin.
        /// </summary>
        public Color BackgroundColor
        {
            get { return backgroundColor; }
            set { backgroundColor = value; }
        }

        /// <summary>
        /// Gets or sets the foreground color of the line number margin.
        /// </summary>
        public Color ForegroundColor
        {
            get { return foregroundColor; }
            set { foregroundColor = value; }
        }

        /// <summary>
        /// Gets or sets the font used to draw the line numbers.
        /// </summary>
        public Font Font
        {
            get { return font; }
            set { font = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LineNumberMargin"/> class.
        /// </summary>
        public LineNumberMargin()
        {
            width = 25; // Default width.
            backgroundColor = Color.LightGray; // Default background color.
            foregroundColor = Color.Black; // Default text color.
            font = new Font("Consolas", 10); // Default font.  Should match CodeEditor's font.
        }

        #endregion

        #region Methods

        /// <summary>
        /// Draws the line number margin.
        /// </summary>
        /// <param name="g">The graphics object to draw on.</param>
        /// <param name="firstVisibleLine">The index of the first visible line.</param>
        /// <param name="visibleLineCount">The number of visible lines.</param>
        /// <param name="lineHeight">The height of each line.</param>
        public void Draw(Graphics g, int firstVisibleLine, int visibleLineCount, int lineHeight)
        {
            if (parentEditor == null) return;

            RectangleF marginRect = new RectangleF(0, 0, width, parentEditor.ClientSize.Height);
            g.FillRectangle(new SolidBrush(backgroundColor), marginRect);

            int startLine = firstVisibleLine;
            int endLine = Math.Min(startLine + visibleLineCount, parentEditor.Lines.Count);

            for (int i = startLine; i < endLine; i++)
            {
                int y = (i - firstVisibleLine) * lineHeight;
                string lineNumber = (i + 1).ToString(); // Display 1-based line numbers.
                SizeF textSize = g.MeasureString(lineNumber, font);
                // Right-align the line number within the margin
                float x = width - textSize.Width - 2; // 2 for padding
                g.DrawString(lineNumber, font, new SolidBrush(foregroundColor), x, y);
            }
        }

        #endregion
    }
}

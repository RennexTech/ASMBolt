using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace ASMBolt.Editor
{
    /// <summary>
    /// A custom text editor control with syntax highlighting and line numbers.
    /// </summary>
    internal class CodeEditor : Control
    {
        #region Fields
        private List<string> lines;
        private int firstVisibleLine;
        private int visibleLineCount;
        private int charWidth;
        private int lineHeight;
        private int xOffset; // Offset for horizontal scrolling
        private Point selectionStart;
        private Point selectionEnd;
        private bool isDragging;
        private SyntaxHighlighter syntaxHighlighter;
        private LineNumberMargin lineNumberMargin;

        private VScrollBar vScrollBar;
        private HScrollBar hScrollBar;

        private const int DEFAULT_CHAR_WIDTH = 7;  // Approximate, can be adjusted.
        private const int DEFAULT_LINE_HEIGHT = 15; // Approximate, can be adjusted.

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the text in the editor.
        /// </summary>
        [Browsable(false)] // Hide from properties window
        public override string Text
        {
            get
            {
                return string.Join(Environment.NewLine, lines);
            }
            set
            {
                lines = value.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList();
                firstVisibleLine = 0;
                RecalculateVisibleLines();
                OnTextChanged(EventArgs.Empty); // Notify about text change
                Invalidate(); // Redraw the control
            }
        }

        /// <summary>
        /// Gets or sets the syntax highlighter.
        /// </summary>
        public SyntaxHighlighter SyntaxHighlighter
        {
            get { return syntaxHighlighter; }
            set
            {
                syntaxHighlighter = value;
                if (syntaxHighlighter != null)
                {
                    syntaxHighlighter.ParentEditor = this; //set the parent
                }
                Invalidate(); // Redraw to apply new highlighting
            }
        }

        /// <summary>
        /// Gets the collection of lines in the editor.  Useful for direct manipulation.
        /// </summary>
        [Browsable(false)]
        public List<string> Lines
        {
            get { return lines; }
        }

        /// <summary>
        /// Gets or sets the starting position of the selection.
        /// </summary>
        [Browsable(false)]
        public Point SelectionStart
        {
            get { return selectionStart; }
            set
            {
                selectionStart = value;
                EnsureVisible(value.Y); // Make sure the line is visible
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the ending position of the selection.
        /// </summary>
        [Browsable(false)]
        public Point SelectionEnd
        {
            get { return selectionEnd; }
            set
            {
                selectionEnd = value;
                EnsureVisible(value.Y);
                Invalidate();
            }
        }

        /// <summary>
        /// Gets the selected text.
        /// </summary>
        [Browsable(false)]
        public string SelectedText
        {
            get
            {
                if (selectionStart == selectionEnd)
                {
                    return string.Empty;
                }

                // Ensure start is before end.
                Point start = selectionStart;
                Point end = selectionEnd;
                if (start.Y > end.Y || (start.Y == end.Y && start.X > end.X))
                {
                    Point temp = start;
                    start = end;
                    end = temp;
                }

                StringBuilder sb = new StringBuilder();
                for (int i = start.Y; i <= end.Y; i++)
                {
                    string lineText = lines[i];
                    if (i == start.Y)
                    {
                        if (i == end.Y)
                        {
                            sb.Append(lineText.Substring(start.X, end.X - start.X));
                        }
                        else
                        {
                            sb.AppendLine(lineText.Substring(start.X));
                        }
                    }
                    else if (i == end.Y)
                    {
                        sb.Append(lineText.Substring(0, end.X));
                    }
                    else
                    {
                        sb.AppendLine(lineText);
                    }
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the line number margin.
        /// </summary>
        public LineNumberMargin LineNumberMargin
        {
            get { return lineNumberMargin; }
            set
            {
                lineNumberMargin = value;
                if (lineNumberMargin != null)
                {
                    lineNumberMargin.ParentEditor = this; //set the parent
                }
                // Adjust padding to accommodate the line number margin.
                if (lineNumberMargin != null)
                {
                    Padding = new Padding(lineNumberMargin.Width + 5, Padding.Top, Padding.Right, Padding.Bottom); // Add a small gap
                }
                else
                {
                    Padding = Padding.Empty;
                }
                Invalidate();
            }
        }

        /// <summary>
        /// Gets the X offset (horizontal scroll position).
        /// </summary>
        [Browsable(false)]
        public int XOffset
        {
            get { return xOffset; }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the text in the editor changes.
        /// </summary>
        public event EventHandler<TextChangedEventArgs> TextChanged;

        /// <summary>
        /// Raises the TextChanged event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected virtual void OnTextChanged(EventArgs e)
        {
            TextChanged?.Invoke(this, new TextChangedEventArgs(this.Text));
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeEditor"/> class.
        /// </summary>
        public CodeEditor()
        {
            Initialize();
        }

        private void Initialize()
        {
            lines = new List<string> { "" }; // Start with one empty line.
            firstVisibleLine = 0;
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            BackColor = Color.White; // Default background color.
            ForeColor = Color.Black; // Default text color.
            Font = new Font("Consolas", 10); // Default font.
            charWidth = DEFAULT_CHAR_WIDTH;
            lineHeight = DEFAULT_LINE_HEIGHT;
            selectionStart = Point.Empty;
            selectionEnd = Point.Empty;
            isDragging = false;

            // Initialize scrollbars
            vScrollBar = new VScrollBar();
            vScrollBar.Dock = DockStyle.Right;
            vScrollBar.Scroll += VScrollBar_Scroll;
            Controls.Add(vScrollBar);

            hScrollBar = new HScrollBar();
            hScrollBar.Dock = DockStyle.Bottom;
            hScrollBar.Scroll += HScrollBar_Scroll;
            Controls.Add(hScrollBar);

            RecalculateVisibleLines();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Calculates the number of visible lines in the editor.
        /// </summary>
        private void RecalculateVisibleLines()
        {
            if (lineHeight <= 0) return; // Avoid division by zero.
            visibleLineCount = ClientSize.Height / lineHeight;
            // Adjust visibleLineCount if the vertical scrollbar is visible.
            if (vScrollBar.Visible)
            {
                visibleLineCount = (ClientSize.Height - vScrollBar.Height) / lineHeight;
            }
            vScrollBar.Maximum = Math.Max(0, lines.Count - 1); // Ensure maximum is not negative
            vScrollBar.Visible = lines.Count > visibleLineCount;
            vScrollBar.LargeChange = visibleLineCount;
            vScrollBar.SmallChange = 1;

            // Horizontal Scrollbar
            int longestLineLength = 0;
            foreach (string line in lines)
            {
                longestLineLength = Math.Max(longestLineLength, line.Length);
            }
            hScrollBar.Maximum = longestLineLength * charWidth;
            hScrollBar.Visible = hScrollBar.Maximum > ClientSize.Width;
            hScrollBar.LargeChange = ClientSize.Width;
            hScrollBar.SmallChange = charWidth;
        }

        /// <summary>
        /// Ensures that the specified line is visible in the editor.
        /// </summary>
        /// <param name="lineIndex">The index of the line to make visible.</param>
        private void EnsureVisible(int lineIndex)
        {
            if (lineIndex < firstVisibleLine)
            {
                firstVisibleLine = lineIndex;
            }
            else if (lineIndex >= firstVisibleLine + visibleLineCount)
            {
                firstVisibleLine = lineIndex - visibleLineCount + 1;
            }
            //keep the scrollbar updated
            vScrollBar.Value = firstVisibleLine;
            Invalidate(); // Redraw to show the updated view.
        }

        /// <summary>
        /// Handles the KeyDown event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            bool control = e.Control;
            bool shift = e.Shift;

            switch (e.KeyCode)
            {
                case Keys.Left:
                    MoveCursor(-1, 0, shift);
                    break;
                case Keys.Right:
                    MoveCursor(1, 0, shift);
                    break;
                case Keys.Up:
                    MoveCursor(0, -1, shift);
                    break;
                case Keys.Down:
                    MoveCursor(0, 1, shift);
                    break;
                case Keys.PageUp:
                    MoveCursor(0, -visibleLineCount, shift);
                    break;
                case Keys.PageDown:
                    MoveCursor(0, visibleLineCount, shift);
                    break;
                case Keys.Home:
                    if (control)
                    {
                        SelectionStart = new Point(0, 0);
                        SelectionEnd = shift ? SelectionEnd : SelectionStart;
                    }
                    else
                    {
                        SelectionStart = new Point(0, SelectionStart.Y);
                        SelectionEnd = shift ? SelectionEnd : SelectionStart;
                    }
                    break;
                case Keys.End:
                    if (control)
                    {
                        SelectionStart = new Point(lines.Count > 0 ? lines[lines.Count - 1].Length : 0, lines.Count - 1);
                        SelectionEnd = shift ? SelectionEnd : SelectionStart;
                    }
                    else
                    {
                        SelectionStart = new Point(lines[SelectionStart.Y].Length, SelectionStart.Y);
                        SelectionEnd = shift ? SelectionEnd : SelectionStart;
                    }
                    break;
                case Keys.Enter:
                    InsertNewLine();
                    break;
                case Keys.Back:
                    DeleteCharacter(false);
                    break;
                case Keys.Delete:
                    DeleteCharacter(true);
                    break;
                case Keys.Tab:
                    InsertTab();
                    break;
                case Keys.Control | Keys.V: // Handle Ctrl+V for paste
                case Keys.V:
                    if (control)
                    {
                        PasteText();
                    }
                    break;
                case Keys.Control | Keys.X: //handle Ctrl + X for cut
                case Keys.X:
                    if (control)
                    {
                        CutText();
                    }
                    break;
                case Keys.Control | Keys.C: // Handle Ctrl+C for copy
                case Keys.C:
                    if (control)
                    {
                        CopyText();
                    }
                    break;
                case Keys.Control | Keys.A: // Handle Ctrl+A for select all
                case Keys.A:
                    if (control)
                    {
                        SelectAll();
                    }
                    break;
            }
        }

        private void SelectAll()
        {
            if (lines.Count > 0)
            {
                SelectionStart = new Point(0, 0);
                SelectionEnd = new Point(lines[lines.Count - 1].Length, lines.Count - 1);
            }
            Invalidate();
        }

        private void CopyText()
        {
            string selectedText = SelectedText; //get selected text
            if (!string.IsNullOrEmpty(selectedText))
            {
                Clipboard.SetText(selectedText); // Copy
            }
        }

        private void CutText()
        {
            string selectedText = SelectedText;
            if (!string.IsNullOrEmpty(selectedText))
            {
                Clipboard.SetText(selectedText); //copy
                DeleteSelectedText(); //delete
            }
        }

        private void PasteText()
        {
            string clipboardText = Clipboard.GetText();
            if (!string.IsNullOrEmpty(clipboardText))
            {
                // Insert the text at the current cursor position
                InsertText(clipboardText);
            }
        }

        private void InsertText(string text)
        {
            // Handle newlines in the pasted text
            string[] pasteLines = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            if (pasteLines.Length > 1)
            {
                // Insert multiple lines
                for (int i = 0; i < pasteLines.Length; i++)
                {
                    if (i == 0)
                    {
                        // Insert the first part of the first line
                        lines[SelectionStart.Y] = lines[SelectionStart.Y].Insert(SelectionStart.X, pasteLines[i]);
                    }
                    else if (i == pasteLines.Length - 1)
                    {
                        // Insert the last part of the last line
                        lines.Insert(SelectionStart.Y + i, pasteLines[i]);
                    }
                    else
                    {
                        //insert the lines in between
                        lines.Insert(SelectionStart.Y + i, pasteLines[i]);
                    }
                }
                //move cursor
                SelectionStart = new Point(pasteLines[pasteLines.Length - 1].Length, SelectionStart.Y + pasteLines.Length - 1);
                SelectionEnd = SelectionStart;
            }
            else
            {
                // Insert a single line
                lines[SelectionStart.Y] = lines[SelectionStart.Y].Insert(SelectionStart.X, text);
                SelectionStart = new Point(SelectionStart.X + text.Length, SelectionStart.Y);
                SelectionEnd = SelectionStart;
            }
            OnTextChanged(EventArgs.Empty);
            Invalidate();
        }

        /// <summary>
        /// Deletes the selected text.
        /// </summary>
        private void DeleteSelectedText()
        {
            if (selectionStart == selectionEnd)
                return;

            Point start = selectionStart;
            Point end = selectionEnd;
            if (start.Y > end.Y || (start.Y == end.Y && start.X > end.X))
            {
                Point temp = start;
                start = end;
                end = temp;
            }

            if (start.Y == end.Y)
            {
                // Delete within the same line
                lines[start.Y] = lines[start.Y].Remove(start.X, end.X - start.X);
            }
            else
            {
                // Delete across multiple lines
                string remainingText = lines[end.Y].Substring(end.X); //keep end
                lines[start.Y] = lines[start.Y].Substring(0, start.X) + remainingText; //connect start
                lines.RemoveRange(start.Y + 1, end.Y - start.Y); //remove middle
            }

            SelectionStart = start;
            SelectionEnd = start;
            OnTextChanged(EventArgs.Empty);
            Invalidate();
        }

        /// <summary>
        /// Deletes a character at the current cursor position.
        /// </summary>
        /// <param name="forward">True to delete the character after the cursor, false to delete before.</param>
        private void DeleteCharacter(bool forward)
        {
            if (selectionStart != selectionEnd)
            {
                DeleteSelectedText();
                return;
            }

            if (lines.Count == 0) return; // Nothing to delete

            int lineIndex = SelectionStart.Y;
            int charIndex = SelectionStart.X;

            if (forward)
            {
                if (lineIndex < lines.Count - 1 || charIndex < lines[lineIndex].Length) //check within bounds
                {
                    if (charIndex < lines[lineIndex].Length)
                    {
                        // Delete character in the current line
                        lines[lineIndex] = lines[lineIndex].Remove(charIndex, 1);
                    }
                    else
                    {
                        // Move content from the next line to the current line
                        if (lineIndex + 1 < lines.Count)
                        {
                            lines[lineIndex] += lines[lineIndex + 1];
                            lines.RemoveAt(lineIndex + 1);
                        }
                    }
                    OnTextChanged(EventArgs.Empty);
                    Invalidate();
                }
            }
            else //backwards
            {
                if (lineIndex > 0 || charIndex > 0)
                {
                    if (charIndex > 0)
                    {
                        // Delete character in the current line
                        lines[lineIndex] = lines[lineIndex].Remove(charIndex - 1, 1);
                        SelectionStart = new Point(charIndex - 1, lineIndex);
                        SelectionEnd = SelectionStart;
                    }
                    else
                    {
                        // Move content from the current line to the previous line
                        if (lineIndex > 0)
                        {
                            int prevLineLength = lines[lineIndex - 1].Length;
                            lines[lineIndex - 1] += lines[lineIndex];
                            lines.RemoveAt(lineIndex);
                            SelectionStart = new Point(prevLineLength, lineIndex - 1);
                            SelectionEnd = SelectionStart;
                        }
                    }
                    OnTextChanged(EventArgs.Empty);
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Inserts a new line at the current cursor position.
        /// </summary>
        private void InsertNewLine()
        {
            int lineIndex = SelectionStart.Y;
            int charIndex = SelectionStart.X;
            string currentLine = lines[lineIndex];
            string newLine = currentLine.Substring(charIndex);
            lines[lineIndex] = currentLine.Substring(0, charIndex);
            lines.Insert(lineIndex + 1, newLine);
            SelectionStart = new Point(0, lineIndex + 1);
            SelectionEnd = SelectionStart;
            OnTextChanged(EventArgs.Empty);
            Invalidate();
            EnsureVisible(lineIndex + 1);
        }

        /// <summary>
        /// Inserts a tab character at the current cursor position.
        /// </summary>
        private void InsertTab()
        {
            int lineIndex = SelectionStart.Y;
            int charIndex = SelectionStart.X;
            lines[lineIndex] = lines[lineIndex].Insert(charIndex, "    "); // Use 4 spaces for a tab
            SelectionStart = new Point(charIndex + 4, lineIndex);
            SelectionEnd = SelectionStart;
            OnTextChanged(EventArgs.Empty);
            Invalidate();
        }

        /// <summary>
        /// Moves the cursor by the specified amount.
        /// </summary>
        /// <param name="dx">The horizontal movement.</param>
        /// <param name="dy">The vertical movement.</param>
        /// <param name="shift">Indicates whether the shift key is pressed (for selection).</param>
        private void MoveCursor(int dx, int dy, bool shift)
        {
            Point originalSelection = shift ? SelectionStart : SelectionEnd; // Store for shift behavior
            int line = SelectionEnd.Y;
            int character = SelectionEnd.X;

            line += dy;
            character += dx;

            // Clamp the line number.
            if (line < 0)
            {
                line = 0;
                character = 0; //move to beginning
            }
            else if (line >= lines.Count)
            {
                line = lines.Count - 1;
                character = lines[line].Length; //move to end
            }

            // Clamp the character position.
            if (line >= 0 && line < lines.Count) //check within bounds
            {
                if (character < 0)
                {
                    if (line > 0)
                    {
                        line--;
                        character = lines[line].Length;
                    }
                    else
                    {
                        character = 0;
                    }
                }
                else if (character > lines[line].Length)
                {
                    if (line < lines.Count - 1)
                    {
                        line++;
                        character = 0;
                    }
                    else
                    {
                        character = lines[line].Length;
                    }
                }
            }

            if (shift)
            {
                SelectionStart = originalSelection; //start stays the same
                SelectionEnd = new Point(character, line);
            }
            else
            {
                SelectionStart = new Point(character, line);
                SelectionEnd = SelectionStart;
            }
            EnsureVisible(line);
            Invalidate();
        }

        /// <summary>
        /// Handles the MouseDown event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                int line = firstVisibleLine + e.Y / lineHeight;
                int character = (e.X - Padding.Left + xOffset) / charWidth; //account for padding
                if (line >= lines.Count)
                {
                    line = lines.Count - 1;
                }
                if (line < 0)
                {
                    line = 0;
                }
                if (character < 0)
                {
                    character = 0;
                }
                else if (line < lines.Count && character > lines[line].Length)
                {
                    character = lines[line].Length;
                }

                selectionStart = new Point(character, line);
                selectionEnd = selectionStart;
                isDragging = true;
                Capture = true; // Capture mouse input
                Invalidate();
            }
        }

        /// <summary>
        /// Handles the MouseMove event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (isDragging)
            {
                int line = firstVisibleLine + e.Y / lineHeight;
                int character = (e.X - Padding.Left + xOffset) / charWidth; //account for padding
                if (line >= lines.Count)
                {
                    line = lines.Count - 1;
                }
                if (line < 0)
                {
                    line = 0;
                }
                if (character < 0)
                {
                    character = 0;
                }
                else if (line < lines.Count && character > lines[line].Length)
                {
                    character = lines[line].Length;
                }
                SelectionEnd = new Point(character, line);
                Invalidate();
            }
        }

        /// <summary>
        /// Handles the MouseUp event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
                Capture = false; // Release mouse input
                Invalidate();
            }
        }

        /// <summary>
        /// Handles the Paint event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.Clear(BackColor); // Clear background

            if (lines.Count == 0 || lineHeight <= 0) return; // Nothing to draw

            int startLine = firstVisibleLine;
            int endLine = Math.Min(startLine + visibleLineCount, lines.Count);

            // Draw visible lines
            for (int i = startLine; i < endLine; i++)
            {
                int y = (i - firstVisibleLine) * lineHeight;
                string lineText = lines[i];
                int lineLength = lineText.Length;

                // Syntax highlighting
                Dictionary<int, Color> colors = null;
                if (syntaxHighlighter != null)
                {
                    colors = syntaxHighlighter.GetHighlighting(i);
                }

                // Draw each character with appropriate color and selection highlighting
                int x = Padding.Left - xOffset; // Apply horizontal scrolling
                for (int j = 0; j < lineLength; j++)
                {
                    Color charColor = ForeColor; // Default color
                    if (colors != null && colors.ContainsKey(j))
                    {
                        charColor = colors[j]; //use the color from the highlighter
                    }

                    // Selection highlighting
                    bool isSelected = IsCharSelected(i, j);
                    if (isSelected)
                    {
                        using (SolidBrush selectionBrush = new SolidBrush(Color.FromArgb(50, 0, 0, 255))) // Semi-transparent blue
                        {
                            g.FillRectangle(selectionBrush, x, y, charWidth, lineHeight);
                        }
                        charColor = Color.White; // Selected text color
                    }

                    using (SolidBrush textBrush = new SolidBrush(charColor))
                    {
                        g.DrawString(lineText[j].ToString(), Font, textBrush, x, y);
                    }
                    x += charWidth;
                }
                // Draw selection at the end of the line
                if (IsCharSelected(i, lineLength))
                {
                    using (SolidBrush selectionBrush = new SolidBrush(Color.FromArgb(50, 0, 0, 255))) // Semi-transparent blue
                    {
                        g.FillRectangle(selectionBrush, x, y, charWidth, lineHeight);
                    }
                    using (SolidBrush textBrush = new SolidBrush(Color.White))
                    {
                        g.DrawString(" ", Font, textBrush, x, y); //draw a space
                    }
                }
            }
            //draw line number margin
            if (lineNumberMargin != null)
            {
                lineNumberMargin.Draw(g, firstVisibleLine, visibleLineCount, lineHeight);
            }
        }

        /// <summary>
        /// Determines if the character at the specified position is selected.
        /// </summary>
        /// <param name="line">The line index.</param>
        /// <param name="character">The character index.</param>
        /// <returns>True if the character is selected, false otherwise.</returns>
        private bool IsCharSelected(int line, int character)
        {
            Point start = selectionStart;
            Point end = selectionEnd;
            if (start.Y > end.Y || (start.Y == end.Y && start.X > end.X))
            {
                Point temp = start;
                start = end;
                end = temp;
            }
            return (line >= start.Y && line <= end.Y &&
                    ((line == start.Y && character >= start.X) ||
                     (line == end.Y && character < end.X) ||
                     (line > start.Y && line < end.Y)));
        }

        /// <summary>
        /// Handles the Resize event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RecalculateVisibleLines();
            Invalidate(); // Redraw on resize
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            //measure the character width and height.
            using (Graphics g = CreateGraphics())
            {
                SizeF size = g.MeasureString("A", Font); // Use "A" as a typical character.
                charWidth = (int)Math.Ceiling(size.Width);
                lineHeight = (int)Math.Ceiling(size.Height);
            }
            RecalculateVisibleLines();
            Invalidate();
        }

        private void VScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            firstVisibleLine = e.NewValue;
            Invalidate();
        }

        private void HScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            xOffset = e.NewValue;
            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //dispose managed resources
                if (vScrollBar != null)
                {
                    vScrollBar.Dispose();
                }
                if (hScrollBar != null)
                {
                    hScrollBar.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Text.RegularExpressions;

namespace ASMBolt.Editor
{
    /// <summary>
    /// Provides syntax highlighting for the CodeEditor control.
    /// </summary>
    internal abstract class SyntaxHighlighter
    {
        #region Fields
        private CodeEditor parentEditor;
        private Dictionary<string, Color> keywords;
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
        /// Gets the dictionary of keywords and their colors.
        /// </summary>
        protected Dictionary<string, Color> Keywords
        {
            get { return keywords; }
            set { keywords = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SyntaxHighlighter"/> class.
        /// </summary>
        public SyntaxHighlighter()
        {
            keywords = new Dictionary<string, Color>();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the highlighting colors for the specified line of text.
        /// </summary>
        /// <param name="lineNumber">The line number.</param>
        /// <returns>A dictionary of character positions and their corresponding colors.</returns>
        public abstract Dictionary<int, Color> GetHighlighting(int lineNumber);

        /// <summary>
        /// Applies basic keyword highlighting to a line.  This can be used by derived classes.
        /// </summary>
        /// <param name="lineText">The text of the line.</param>
        /// <returns>A dictionary of character positions and their colors.</returns>
        protected Dictionary<int, Color> HighlightKeywords(string lineText)
        {
            Dictionary<int, Color> colors = new Dictionary<int, Color>();
            if (keywords == null || keywords.Count == 0) return colors;

            // Order keywords by length to match longer keywords first (e.g., "DWORD" before "DW").
            var orderedKeywords = keywords.OrderByDescending(k => k.Key.Length);
            foreach (var keyword in orderedKeywords)
            {
                string pattern = @"\b" + Regex.Escape(keyword.Key) + @"\b"; //match whole words only
                MatchCollection matches = Regex.Matches(lineText, pattern, RegexOptions.IgnoreCase); //case-insensitive
                foreach (Match match in matches)
                {
                    for (int i = match.Index; i < match.Index + match.Length; i++)
                    {
                        colors[i] = keyword.Value;
                    }
                }
            }
            return colors;
        }
        #endregion
    }

    /// <summary>
    /// Provides syntax highlighting for MASM assembly language.
    /// </summary>
    internal class MasmSyntaxHighlighter : SyntaxHighlighter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MasmSyntaxHighlighter"/> class.
        /// </summary>
        public MasmSyntaxHighlighter()
        {
            Keywords.Add("mov", Color.Blue);
            Keywords.Add("add", Color.Blue);
            Keywords.Add("sub", Color.Blue);
            Keywords.Add("jmp", Color.Blue);
            Keywords.Add("jz", Color.Blue);
            Keywords.Add("jnz", Color.Blue);
            Keywords.Add("cmp", Color.Blue);
            Keywords.Add("push", Color.Blue);
            Keywords.Add("pop", Color.Blue);
            Keywords.Add("call", Color.Blue);
            Keywords.Add("ret", Color.Blue);
            Keywords.Add("section", Color.DarkGreen);
            Keywords.Add(".data", Color.DarkGreen);
            Keywords.Add(".text", Color.DarkGreen);
            Keywords.Add("db", Color.DarkRed);
            Keywords.Add("dw", Color.DarkRed);
            Keywords.Add("dd", Color.DarkRed);
            Keywords.Add("equ", Color.Purple);
            Keywords.Add("include", Color.Purple);
            Keywords.Add("org", Color.Purple);
        }
        /// <summary>
        /// Gets the highlighting colors for the specified line of text.
        /// </summary>
        /// <param name="lineNumber">The line number.</param>
        /// <returns>A dictionary of character positions and their corresponding colors.</returns>
        public override Dictionary<int, Color> GetHighlighting(int lineNumber)
        {
            if (ParentEditor == null || lineNumber < 0 || lineNumber >= ParentEditor.Lines.Count)
            {
                return new Dictionary<int, Color>();
            }
            string lineText = ParentEditor.Lines[lineNumber];
            Dictionary<int, Color> colors = HighlightKeywords(lineText);

            //comments
            int commentIndex = lineText.IndexOf(';');
            if (commentIndex >= 0)
            {
                for (int i = commentIndex; i < lineText.Length; i++)
                {
                    colors[i] = Color.Green;
                }
            }
            return colors;
        }
    }

    /// <summary>
    /// Provides syntax highlighting for NASM assembly language.
    /// </summary>
    internal class NasmSyntaxHighlighter : SyntaxHighlighter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NasmSyntaxHighlighter"/> class.
        /// </summary>
        public NasmSyntaxHighlighter()
        {
            Keywords.Add("mov", Color.Blue);
            Keywords.Add("add", Color.Blue);
            Keywords.Add("sub", Color.Blue);
            Keywords.Add("jmp", Color.Blue);
            Keywords.Add("je", Color.Blue);
            Keywords.Add("jne", Color.Blue);
            Keywords.Add("cmp", Color.Blue);
            Keywords.Add("push", Color.Blue);
            Keywords.Add("pop", Color.Blue);
            Keywords.Add("call", Color.Blue);
            Keywords.Add("ret", Color.Blue);
            Keywords.Add("section", Color.DarkGreen);
            Keywords.Add("data", Color.DarkGreen);
            Keywords.Add("text", Color.DarkGreen);
            Keywords.Add("db", Color.DarkRed);
            Keywords.Add("dw", Color.DarkRed);
            Keywords.Add("dd", Color.DarkRed);
            Keywords.Add("equ", Color.Purple);
            Keywords.Add("incbin", Color.Purple);
            Keywords.Add("bits", Color.Purple);
        }

        /// <summary>
        /// Gets the highlighting colors for the specified line of text.
        /// </summary>
        /// <param name="lineNumber">The line number.</param>
        /// <returns>A dictionary of character positions and their corresponding colors.</returns>
        public override Dictionary<int, Color> GetHighlighting(int lineNumber)
        {
            if (ParentEditor == null || lineNumber < 0 || lineNumber >= ParentEditor.Lines.Count)
            {
                return new Dictionary<int, Color>();
            }
            string lineText = ParentEditor.Lines[lineNumber];
            Dictionary<int, Color> colors = HighlightKeywords(lineText);

            // Comments
            int commentIndex = lineText.IndexOf(';');
            if (commentIndex >= 0)
            {
                for (int i = commentIndex; i < lineText.Length; i++)
                {
                    colors[i] = Color.Green;
                }
            }
            return colors;
        }
    }
}




//VERSION 2 OF THE SYNTAX HIGHLIGHTER

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Text.RegularExpressions;

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
                int lineLength = lineText.Length;A

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



                        //version 3 of hte syntax highlighter

                        using System;
                        using System.Diagnostics;
                        using System.IO;
                        using System.Linq;
                        using System.Text;
                        using System.Text.RegularExpressions;
                        using System.Windows.Forms;
                        using Microsoft.Win32; // For registry access
                        using System.Collections.Generic;
                        using System.Drawing;

namespace ASM_Bolt
    {
        public partial class MainForm : Form
        {
            private const string AppTitle = "ASM⚡Bolt";
            private const string DefaultOutputFileName = "output.exe";
            private string _currentFilePath;
            private bool _isDarkMode = true; // Default to dark mode
            private string _currentAssembler = ""; // Store detected assembler

            // Define syntax highlighting rules with improved regex for MASM and NASM
            private Dictionary<string, Color> _syntaxHighlightingRules = new Dictionary<string, Color>
        {
            // MASM Keywords and Directives (more comprehensive)
            { @"\b(PROC|ENDP|INCLUDE|INCLUDELIB|EXTERN|PUBLIC|GLOBAL|SEGMENT|ENDS|ASSUME|DB|DW|DD|DQ|DT|DF|ORG|EQU|LABEL|BYTE|WORD|DWORD|QWORD|TBYTE|FWORD)\b", Color.SteelBlue }, // Directives
            { @"\b(mov|add|sub|mul|div|imul|idiv|and|or|xor|not|shl|shr|sal|sar|push|pop|lea|cmp|test|jmp|je|jne|jz|jnz|jg|jge|jl|jle|ja|jae|jb|jbe|call|ret|int|iret|loop|loope|loopne|loopz|loopnz)\b", Color.DodgerBlue }, // Instructions
            { @"\b(eax|ebx|ecx|edx|esi|edi|esp|ebp|ax|bx|cx|dx|si|di|sp|bp|al|ah|bl|bh|cl|ch|dl|dh|ip|eip|cs|ds|es|fs|gs|ss)\b", Color.MediumPurple }, // Registers
            { @"\b(ptr|offset|seg|type|length|size|this)\b", Color.DarkCyan}, //MASM Specific
            { @"\b[0-9]+[0-9a-fA-F]*[hH]?\b", Color.OrangeRed }, // Numbers (decimal and hex)
            { @"'[^']*?'", Color.DarkGreen }, // Strings
            { @";[^\n]*", Color.Green },    // Comments
            { @"\b[a-zA-Z_][a-zA-Z0-9_]*:?\b", Color.Yellow }, // Labels (Improved to handle optional colon)

            //NASM Keywords and Directives
            { @"\b(section|global|extern|bits|resb|resw|resd|resq|rest|db|dw|dd|dq|dt|do|dy|times)\b", Color.SteelBlue }, // Directives
            { @"\b(mov|add|sub|mul|div|imul|idiv|and|or|xor|not|shl|shr|sal|sar|push|pop|lea|cmp|test|jmp|je|jne|jz|jnz|jg|jge|jl|jle|ja|jae|jb|jbe|call|ret|int|iret|loop|loope|loopne|loopz|loopnz)\b", Color.DodgerBlue }, // Instructions
            { @"\b(rax|rbx|rcx|rdx|rsi|rdi|rsp|rbp|eax|ebx|ecx|edx|esi|edi|esp|ebp|ax|bx|cx|dx|si|di|sp|bp|al|ah|bl|bh|cl|ch|dl|dh|ip|eip|rip|cs|ds|es|fs|gs|ss)\b", Color.MediumPurple }, // Registers
            { @"\b(byte|word|dword|qword|tword|oword|yword|zword)\b", Color.DarkCyan}, //NASM Specific
            { @"\b[0-9]+[0-9a-fA-F]*[hH]?\b", Color.OrangeRed }, // Numbers (decimal and hex)
            { @"'[^']*?'", Color.DarkGreen }, // Strings
            { @";[^\n]*", Color.Green },    // Comments
            { @"\b[a-zA-Z_][a-zA-Z0-9_]*:?\b", Color.Yellow }, // Labels
        };

            public MainForm()
            {
                InitializeComponent();
                InitializeUI();
            }

            private void InitializeUI()
            {
                // Set initial UI state
                SetDarkMode(_isDarkMode);
                outputTextBox.Text = "Ready.";
                fileStatusLabel.Text = "No file loaded";
                // Set default values for other UI elements
                outputFileNameTextBox.Text = DefaultOutputFileName;
                linkCheckBox.Checked = true;
                runCheckBox.Checked = false;
                // Initialize RichTextBox for syntax highlighting
                sourceCodeRichTextBox.BackColor = _isDarkMode ? Color.FromArgb(40, 40, 40) : SystemColors.Window;
                sourceCodeRichTextBox.ForeColor = _isDarkMode ? Color.White : SystemColors.WindowText;
                sourceCodeRichTextBox.Font = new Font("Consolas", 10); // Use a monospaced font
                sourceCodeRichTextBox.TextChanged += SourceCodeRichTextBox_TextChanged; // Attach event
                sourceCodeRichTextBox.SelectionChanged += SourceCodeRichTextBox_SelectionChanged;
            }

            private void SetDarkMode(bool isDarkMode)
            {
                _isDarkMode = isDarkMode;
                if (isDarkMode)
                {
                    // Dark mode colors
                    BackColor = Color.FromArgb(30, 30, 30);
                    ForeColor = Color.White;
                    outputTextBox.BackColor = Color.FromArgb(40, 40, 40);
                    outputTextBox.ForeColor = Color.White;
                    fileStatusLabel.ForeColor = Color.White;
                    menuStrip1.BackColor = Color.FromArgb(40, 40, 40);
                    menuStrip1.ForeColor = Color.White;
                    toolsToolStripMenuItem.ForeColor = Color.White;
                    optionsToolStripMenuItem.ForeColor = Color.White;
                    darkModeToolStripMenuItem.Checked = true;
                    outputFileNameTextBox.BackColor = Color.FromArgb(40, 40, 40);
                    outputFileNameTextBox.ForeColor = Color.White;
                    linkCheckBox.ForeColor = Color.White;
                    runCheckBox.ForeColor = Color.White;
                    sourceCodeRichTextBox.BackColor = Color.FromArgb(40, 40, 40);
                    sourceCodeRichTextBox.ForeColor = Color.White;

                }
                else
                {
                    // Light mode colors
                    BackColor = SystemColors.Control;
                    ForeColor = SystemColors.ControlText;
                    outputTextBox.BackColor = SystemColors.Window;
                    outputTextBox.ForeColor = SystemColors.WindowText;
                    fileStatusLabel.ForeColor = SystemColors.ControlText;
                    menuStrip1.BackColor = SystemColors.Control;
                    menuStrip1.ForeColor = SystemColors.ControlText;
                    toolsToolStripMenuItem.ForeColor = SystemColors.ControlText;
                    optionsToolStripMenuItem.ForeColor = Color.ControlText;
                    darkModeToolStripMenuItem.Checked = false;
                    outputFileNameTextBox.BackColor = SystemColors.Window;
                    outputFileNameTextBox.ForeColor = SystemColors.WindowText;
                    linkCheckBox.ForeColor = SystemColors.ControlText;
                    runCheckBox.ForeColor = Color.ControlText;
                    sourceCodeRichTextBox.BackColor = SystemColors.Window;
                    sourceCodeRichTextBox.ForeColor = SystemColors.WindowText;
                }
            }

            private void OpenFile(string filePath)
            {
                try
                {
                    _currentFilePath = filePath;
                    fileNameLabel.Text = Path.GetFileName(_currentFilePath);
                    fileStatusLabel.Text = "File loaded.";
                    outputTextBox.Text = $"File loaded: {filePath}";
                    EnableCompileButton(); // Enable compile button when a file is loaded.

                    // Load the file content into the RichTextBox
                    sourceCodeRichTextBox.Text = File.ReadAllText(_currentFilePath);
                    // Apply syntax highlighting
                    HighlightSyntax();

                }
                catch (Exception ex)
                {
                    outputTextBox.Text = $"Error opening file: {ex.Message}";
                    fileStatusLabel.Text = "Error loading file.";
                    DisableCompileButton(); // Disable if loading fails
                }
            }

            private void EnableCompileButton()
            {
                compileButton.Enabled = true;
            }

            private void DisableCompileButton()
            {
                compileButton.Enabled = false;
            }

            private void fileToolStripMenuItem_Click(object sender, EventArgs e)
            {
                using (var openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Assembly Files (*.asm;*.s;*.S)|*.asm;*.s;*.S|All files (*.*)|*.*";
                    openFileDialog.Title = "Open Assembly File";
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        OpenFile(openFileDialog.FileName);
                    }
                }
            }

            private void exitToolStripMenuItem_Click(object sender, EventArgs e)
            {
                Application.Exit();
            }

            private void Compile()
            {
                if (string.IsNullOrEmpty(_currentFilePath))
                {
                    outputTextBox.Text = "No file to compile. Open a file first.";
                    return;
                }

                string assembler = DetectAssembler();
                if (string.IsNullOrEmpty(assembler))
                {
                    outputTextBox.Text = "No supported assembler found. Please install MASM, NASM, FASM, or GAS, and ensure it's in your PATH.";
                    return;
                }
                _currentAssembler = assembler; // Store the detected assembler
                CompileAssembly(assembler);
            }

            private string DetectAssembler()
            {
                // Check for MASM (ml.exe or ml64.exe) - Order matters for preference.
                if (CheckForExecutable("ml64.exe"))
                {
                    return "MASM64";
                }
                if (CheckForExecutable("ml.exe"))
                {
                    return "MASM";
                }
                // Check for NASM (nasm.exe)
                if (CheckForExecutable("nasm.exe"))
                {
                    return "NASM";
                }
                // Check for FASM (fasm.exe)
                if (CheckForExecutable("fasm.exe"))
                {
                    return "FASM";
                }
                // Check for GAS (as.exe, often with GCC) -  More complex, so check last.
                if (CheckForExecutable("as.exe"))
                {
                    return "GAS";
                }
                return null;
            }

            private bool CheckForExecutable(string exeName)
            {
                // Check if the executable exists in the system's PATH.
                string path = Environment.GetEnvironmentVariable("PATH");
                if (string.IsNullOrEmpty(path)) return false;

                string[] paths = path.Split(';');
                return paths.Any(p => File.Exists(Path.Combine(p, exeName)));
            }

            private void CompileAssembly(string assembler)
            {
                outputTextBox.Text = $"Using {assembler} to compile...";
                string outputFilePath = Path.Combine(Path.GetDirectoryName(_currentFilePath), outputFileNameTextBox.Text);
                string objFilePath = Path.ChangeExtension(_currentFilePath, ".obj"); // Use .obj for object file

                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.UseShellExecute = false;
                    psi.RedirectStandardOutput = true;
                    psi.RedirectStandardError = true;
                    psi.CreateNoWindow = true; // Prevents a command window from flashing.

                    // Assembler-specific command line arguments.
                    switch (assembler)
                    {
                        case "MASM":
                        case "MASM64":
                            string masmPath = GetMasmPath(assembler); // Get the path to ml.exe or ml64.exe
                            if (string.IsNullOrEmpty(masmPath))
                            {
                                outputTextBox.Text = $"{assembler} path not found in registry.";
                                return;
                            }
                            psi.FileName = masmPath;
                            psi.Arguments = assembler == "MASM"
                                ? $"/c /coff /Fo\"{objFilePath}\" \"{_currentFilePath}\"" // 32-bit MASM
                                : $"/c /coff /Fo\"{objFilePath}\" \"{_currentFilePath}\""; // 64-bit MASM
                            break;
                        case "NASM":
                            psi.FileName = "nasm";
                            psi.Arguments = $"-f win{(IntPtr.Size == 8 ? "64" : "32")} \"{_currentFilePath}\" -o \"{objFilePath}\"";
                            break;
                        case "FASM":
                            psi.FileName = "fasm";
                            psi.Arguments = $"\"{_currentFilePath}\" \"{objFilePath}\""; // FASM creates .obj
                            break;
                        case "GAS": // GAS is more complex, often used with GCC.
                            psi.FileName = "as"; // Or "gcc" if you want linking done by GCC
                            psi.Arguments = $"-o \"{objFilePath}\" \"{_currentFilePath}\""; // GAS creates .o on Windows, but we'll rename it
                            break;
                        default:
                            outputTextBox.Text = $"Assembler '{assembler}' not supported.";
                            return;
                    }

                    Process process = Process.Start(psi);
                    if (process == null)
                    {
                        outputTextBox.Text = "Failed to start the compilation process.";
                        return;
                    }
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    // Rename .o to .obj for GAS (if used) for consistency in linking.
                    if (assembler == "GAS" && File.Exists(objFilePath))
                    {
                        File.Move(objFilePath, Path.ChangeExtension(_currentFilePath, ".obj"), true); //overwrite if exists
                    }

                    if (process.ExitCode != 0)
                    {
                        outputTextBox.Text = $"Compilation failed.\nOutput: {output}\nError: {error}";
                        return;
                    }

                    outputTextBox.Text = $"Compilation successful.\nOutput: {output}";

                    if (linkCheckBox.Checked)
                    {
                        LinkAndRun(assembler, objFilePath, outputFilePath);
                    }
                    else
                    {
                        File.Delete(objFilePath); // Clean up .obj if not linking.
                    }
                }
                catch (Exception ex)
                {
                    outputTextBox.Text = $"Error during compilation: {ex.Message}";
                }
            }

            private void LinkAndRun(string assembler, string objFilePath, string outputFilePath)
            {
                outputTextBox.Text += "\nLinking...";
                try
                {
                    ProcessStartInfo linkPsi = new ProcessStartInfo();
                    linkPsi.UseShellExecute = false;
                    linkPsi.RedirectStandardOutput = true;
                    linkPsi.RedirectStandardError = true;
                    linkPsi.CreateNoWindow = true;

                    // Linker-specific commands.  Use LINK.EXE for MASM, GCC for others (more portable).
                    if (assembler == "MASM" || assembler == "MASM64")
                    {
                        string linkPath = GetLinkerPath(assembler);
                        if (string.IsNullOrEmpty(linkPath))
                        {
                            outputTextBox.Text = $"{assembler} linker (link.exe) not found in registry.";
                            return;
                        }
                        linkPsi.FileName = linkPath;
                        linkPsi.Arguments = assembler == "MASM"
                            ? $"/NOLOGO /SUBSYSTEM:CONSOLE \"{objFilePath}\" /OUT:\"{outputFilePath}\"" // 32-bit
                            : $"/NOLOGO /SUBSYSTEM:CONSOLE \"{objFilePath}\" /OUT:\"{outputFilePath}\""; // 64-bit
                    }
                    else
                    {
                        linkPsi.FileName = "gcc"; // Use GCC for linking (cross-assembler compatibility)
                        linkPsi.Arguments = $"\"{objFilePath}\" -o \"{outputFilePath}\"";
                    }
                    Process linkProcess = Process.Start(linkPsi);
                    if (linkProcess == null)
                    {
                        outputTextBox.Text = "Failed to start the linking process.";
                        return;
                    }

                    string linkOutput = linkProcess.StandardOutput.ReadToEnd();
                    string linkError = linkProcess.StandardError.ReadToEnd();
                    linkProcess.WaitForExit();

                    if (linkProcess.ExitCode != 0)
                    {
                        outputTextBox.Text = $"Linking failed.\nOutput: {linkOutput}\nError: {linkError}";
                        File.Delete(objFilePath); // Clean up .obj on linking failure
                        return;
                    }
                    outputTextBox.Text += $"\nLinking successful.\nOutput: {linkOutput}";
                    File.Delete(objFilePath); // Clean up .obj after successful linking

                    if (runCheckBox.Checked)
                    {
                        RunProgram(outputFilePath);
                    }
                }
                catch (Exception ex)
                {
                    outputTextBox.Text = $"Error during linking: {ex.Message}";
                }
            }

            private void RunProgram(string filePath)
            {
                outputTextBox.Text += "\nRunning program...";
                try
                {
                    ProcessStartInfo runPsi = new ProcessStartInfo();
                    runPsi.FileName = filePath;
                    runPsi.UseShellExecute = false; // Don't use shell for redirection.
                    runPsi.RedirectStandardOutput = true;
                    runPsi.RedirectStandardError = true;
                    runPsi.CreateNoWindow = true;
                    Process runProcess = Process.Start(runPsi);

                    if (runProcess == null)
                    {
                        outputTextBox.Text = "Failed to start the program.";
                        return;
                    }
                    string runOutput = runProcess.StandardOutput.ReadToEnd();
                    string runError = runProcess.StandardError.ReadToEnd();
                    runProcess.WaitForExit();
                    outputTextBox.Text += $"\nProgram output:\n{runOutput}\nError:{runError}";
                }
                catch (Exception ex)
                {
                    outputTextBox.Text = $"Error running program: {ex.Message}";
                }
            }

            // Helper function to get MASM path from registry
            private string GetMasmPath(string assemblerType)
            {
                string keyName = assemblerType == "MASM"
                    ? @"SOFTWARE\Microsoft\VisualStudio\SxS\VC7" // For 32-bit MASM
                    : @"SOFTWARE\Wow6432Node\Microsoft\VisualStudio\SxS\VC7"; // For 64-bit MASM on 64-bit Windows

                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(keyName))
                {
                    if (key != null)
                    {
                        string version = key.GetValue("14.0") as string; // Example: "14.0" for VS 2015, may need adjustment
                        if (!string.IsNullOrEmpty(version))
                        {
                            string masmPath = Path.Combine(version, assemblerType == "MASM" ? "bin" : "bin\\amd64", assemblerType == "MASM" ? "ml.exe" : "ml64.exe");
                            if (File.Exists(masmPath))
                            {
                                return masmPath;
                            }
                        }
                    }
                }
                return string.Empty;
            }

            private string GetLinkerPath(string assemblerType)
            {
                string keyName = assemblerType == "MASM"
                   ? @"SOFTWARE\Microsoft\VisualStudio\SxS\VC7" // For 32-bit MASM
                   : @"SOFTWARE\Wow6432Node\Microsoft\VisualStudio\SxS\VC7";  // For 64-bit MASM on 64-bit Windows
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(keyName))
                {
                    if (key != null)
                    {
                        string version = key.GetValue("14.0") as string;  // May need to adjust
                        if (!string.IsNullOrEmpty(version))
                        {
                            string linkPath = Path.Combine(version, assemblerType == "MASM" ? "bin" : "bin\\amd64", "link.exe");
                            if (File.Exists(linkPath))
                            {
                                return linkPath;
                            }
                        }
                    }
                }
                return string.Empty;
            }
            private void compileButton_Click(object sender, EventArgs e)
            {
                Compile();
            }

            private void darkModeToolStripMenuItem_Click(object sender, EventArgs e)
            {
                _isDarkMode = !_isDarkMode;
                SetDarkMode(_isDarkMode);
            }

            private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
            {
                MessageBox.Show("ASM⚡Bolt v1.0\n\nDeveloped by [Your Name]\n\nA lightweight assembly compilation tool.", "About ASM⚡Bolt", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            private void SourceCodeRichTextBox_TextChanged(object sender, EventArgs e)
            {
                HighlightSyntax();
            }

            private void HighlightSyntax()
            {
                if (string.IsNullOrEmpty(sourceCodeRichTextBox.Text)) return;

                // Store cursor position and original text color to avoid losing these.
                int selectionStart = sourceCodeRichTextBox.SelectionStart;
                Color originalColor = sourceCodeRichTextBox.ForeColor;

                // Set the entire text to the default color.
                sourceCodeRichTextBox.SelectAll();
                sourceCodeRichTextBox.SelectionColor = _isDarkMode ? Color.White : SystemColors.WindowText;
                sourceCodeRichTextBox.SelectionStart = 0; // Reset selection to the start


                foreach (var rule in _syntaxHighlightingRules)
                {
                    Regex regex = new Regex(rule.Key, RegexOptions.IgnoreCase);
                    MatchCollection matches = regex.Matches(sourceCodeRichTextBox.Text);

                    foreach (Match match in matches)
                    {
                        // Apply the highlighting color to the matched text.
                        sourceCodeRichTextBox.Select(match.Index, match.Length);
                        sourceCodeRichTextBox.SelectionColor = rule.Value;
                    }
                }
                // Restore the original cursor position and text color.
                sourceCodeRichTextBox.SelectionStart = selectionStart;
                sourceCodeRichTextBox.SelectionColor = originalColor;
            }
            private void SourceCodeRichTextBox_SelectionChanged(object sender, EventArgs e)
            {
                //This method is needed to prevent the RichTextBox from losing focus.
                sourceCodeRichTextBox.Focus();
            }
        }
    }


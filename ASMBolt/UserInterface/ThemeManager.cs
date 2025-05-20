using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace ASMBolt.UserInterface
{
    /// <summary>
    /// Manages the application's visual theme, applying colors and styles to UI elements.
    /// </summary>
    internal class ThemeManager
    {
        private static Color _backgroundColor;
        private static Color _textColor;
        private static Color _controlBackColor;
        private static Color _controlForeColor;

        /// <summary>
        /// Gets the background color for the current theme.
        /// </summary>
        public static Color BackgroundColor { get { return _backgroundColor; } }

        /// <summary>
        /// Gets the text color for the current theme.
        /// </summary>
        public static Color TextColor { get { return _textColor; } }

        /// <summary>
        /// Gets the background color for controls in the current theme.
        /// </summary>
        public static Color ControlBackColor { get { return _controlBackColor; } }

        /// <summary>
        /// Gets the foreground color for controls in the current theme.
        /// </summary>
        public static Color ControlForeColor { get { return _controlForeColor; } }

        /// <summary>
        /// Applies the current theme to the specified form and its controls.
        /// </summary>
        /// <param name="form">The form to apply the theme to.</param>
        public static void ApplyTheme(Form form)
        {
            // Determine theme based on the setting.
            string themeName = SettingsManager.Theme;
            switch (themeName.ToLower())
            {
                case "dark":
                    _backgroundColor = Color.FromArgb(30, 30, 30);  // Dark background.
                    _textColor = Color.White;
                    _controlBackColor = Color.FromArgb(45, 45, 45);
                    _controlForeColor = Color.White;
                    break;
                case "light":
                    _backgroundColor = Color.White;        // Light background.
                    _textColor = Color.Black;
                    _controlBackColor = Color.FromArgb(240, 240, 240);
                    _controlForeColor = Color.Black;
                    break;
                default:
                    _backgroundColor = Color.FromArgb(30, 30, 30);  // Default to dark.
                    _textColor = Color.White;
                    _controlBackColor = Color.FromArgb(45, 45, 45);
                    _controlForeColor = Color.White;
                    break;
            }

            // Apply colors to the form.
            form.BackColor = _backgroundColor;
            form.ForeColor = _textColor;
            ApplyThemeToControls(form.Controls); //recursive
        }

        /// <summary>
        /// Applies the current theme to the specified control and its children.
        /// </summary>
        /// <param name="controls">The collection of controls to apply the theme to.</param>
        private static void ApplyThemeToControls(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                control.BackColor = _controlBackColor;
                control.ForeColor = _controlForeColor;
                if (control is TextBox)
                {
                    control.BackColor = _backgroundColor;
                    control.ForeColor = _textColor;
                }
                else if (control is RichTextBox)
                {
                    control.BackColor = _backgroundColor;
                    control.ForeColor = _textColor;
                }
                else if (control is Button)
                {
                    control.BackColor = _controlBackColor;
                    control.ForeColor = _controlForeColor;
                }
                // Recursively apply the theme to child controls.
                if (control.HasChildren)
                {
                    ApplyThemeToControls(control.Controls);
                }
            }
        }
    }
}
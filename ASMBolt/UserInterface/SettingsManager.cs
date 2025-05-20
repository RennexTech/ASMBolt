using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Drawing;

namespace ASMBolt.UserInterface
{
    /// <summary>
    /// Manages application settings, providing strongly-typed access and persistence.
    /// </summary>
    internal class SettingsManager
    {
        // Use constants for setting keys to avoid typos and improve maintainability.
        private const string Key_Theme = "Theme";
        private const string Key_FontSize = "FontSize";
        private const string Key_FontName = "FontName";
        private const string Key_EditorBackgroundColor = "EditorBackgroundColor";
        private const string Key_EditorTextColor = "EditorTextColor";

        /// <summary>
        /// Gets or sets the current theme of the application.
        /// </summary>
        public static string Theme
        {
            get
            {
                // Use ConfigurationManager for both reading and writing settings.
                return ConfigurationManager.AppSettings[Key_Theme] ?? "Dark"; // Default value.
            }
            set
            {
                ConfigurationManager.AppSettings[Key_Theme] = value;
            }
        }

        /// <summary>
        /// Gets or sets the font size for the editor.
        /// </summary>
        public static int FontSize
        {
            get
            {
                string fontSizeValue = ConfigurationManager.AppSettings[Key_FontSize];
                if (int.TryParse(fontSizeValue, out int fontSize))
                {
                    return fontSize;
                }
                return 10; // Default value.
            }
            set
            {
                ConfigurationManager.AppSettings[Key_FontSize] = value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the font name for the editor.
        /// </summary>
        public static string FontName
        {
            get
            {
                return ConfigurationManager.AppSettings[Key_FontName] ?? "Consolas"; // Default
            }
            set
            {
                ConfigurationManager.AppSettings[Key_FontName] = value;
            }
        }

        /// <summary>
        /// Gets or sets the background color of the editor.
        /// </summary>
        public static Color EditorBackgroundColor
        {
            get
            {
                string colorValue = ConfigurationManager.AppSettings[Key_EditorBackgroundColor];
                if (colorValue != null)
                {
                    try
                    {
                        return Color.FromName(colorValue);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error converting EditorBackgroundColor: {ex.Message}");
                    }
                }
                return Color.White; // Default
            }
            set
            {
                ConfigurationManager.AppSettings[Key_EditorBackgroundColor] = value.Name;
            }
        }

        /// <summary>
        /// Gets or sets the text color of the editor.
        /// </summary>
        public static Color EditorTextColor
        {
            get
            {
                string colorValue = ConfigurationManager.AppSettings[Key_EditorTextColor];
                if (colorValue != null)
                {
                    try
                    {
                        return Color.FromName(colorValue);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error converting EditorTextColor: {ex.Message}");
                    }
                }
                return Color.Black; // Default
            }
            set
            {
                ConfigurationManager.AppSettings[Key_EditorTextColor] = value.Name;
            }
        }

        /// <summary>
        /// Saves all modified settings to the configuration file.
        /// </summary>
        public static void SaveSettings()
        {
            // ConfigurationManager.AppSettings is an in-memory collection.  To persist
            // changes, you need to save them back to the configuration file.
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            foreach (string key in ConfigurationManager.AppSettings.AllKeys)
            {
                config.AppSettings.Settings[key].Value = ConfigurationManager.AppSettings[key];
            }
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings"); //reload
        }
    }
}

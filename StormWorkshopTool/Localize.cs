using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace StormWorkshopTool
{
    /// <summary>
    /// Localization utils for the tool
    /// </summary>
    public static class Localize
    {
        private static readonly Dictionary<string, string> Replacements = new Dictionary<string, string>();

        /// <summary>
        /// Culture name expected by the tool, name your INI file {CultureName}-loc.ini
        /// </summary>
        public static string CultureName { get; private set; }

        /// <summary>
        /// Call this once to load a localization file
        /// </summary>
        public static void Load()
        {
            var myCulture = CultureInfo.CurrentUICulture.ThreeLetterISOLanguageName.ToLowerInvariant();
            CultureName = myCulture;
            var exeDir = AppContext.BaseDirectory;

            var textEncoding = new UTF8Encoding(false, true);
            var iniPath = Path.Combine(exeDir, myCulture + "-loc.ini");
            try
            {
                var lines = File.ReadAllLines(iniPath, textEncoding);
                Replacements.Clear();
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    // # a comment
                    // // a comment
                    // ; a comment
                    if (line[0] == '#' || line[0] == '/' || line[0] == ';' || line[0] == '[')
                        continue;
                    var eqIdx = line.IndexOf('=');
                    // a valid line cannot begin with an equal sign...
                    if (eqIdx <= 0)
                        continue;
                    var key = line.Substring(0, eqIdx).Trim();
                    var value = line.Substring(eqIdx + 1).Trim();
                    // do not accept empty or whitespace keys...
                    if (string.IsNullOrWhiteSpace(key))
                        continue;
                    // not .Add so the last key's val overwrites previous if it exists...
                    Replacements[key] = value;
                }
            }
            catch
            {
                /* uh no! revert to the fallback... :( */
            }
        }

        /// <summary>
        /// Takes a C# format string and localizes it...
        /// </summary>
        /// <param name="fallback">Default english text</param>
        /// <param name="id">Localization ID</param>
        /// <param name="args">Format values</param>
        /// <returns>Localized formatted string</returns>
        public static string Tr(string fallback, string id, params object[] args)
        {
            if (Replacements.TryGetValue(id, out var trd))
            {
                return string.Format(trd, args);
            }
            return string.Format(fallback, args);
        }
    }
}

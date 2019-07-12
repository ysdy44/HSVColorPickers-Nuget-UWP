using Windows.UI;

namespace HSVColorPickers
{
    /// <summary> 
    /// Hex to color converter. 
    /// </summary>
    public sealed class Hex
    {
        /// <summary> Hex number To Color </summary>
        public static Color IntToColor(int hexNumber) => Color.FromArgb(255, (byte)((hexNumber >> 16) & 0xff), (byte)((hexNumber >> 8) & 0xff), (byte)((hexNumber >> 0) & 0xff));

        /// <summary> String To Hex number </summary>
        public static int StringToInt(string hex) => int.Parse(hex, System.Globalization.NumberStyles.HexNumber);

        /// <summary> String To Color </summary>
        public static string ColorToString(Color color) => color.R.ToString("x2") + color.G.ToString("x2") + color.B.ToString("x2").ToString();

        /// <summary> Subste </summary>
        public static string TextSubstring(string text)
        {
            if (text == null) return null;

            if (text.Length < 6) return null;

            if (text.Length == 6) return text;

            return text.Substring(text.Length - 6, 6);
        }
    }
}
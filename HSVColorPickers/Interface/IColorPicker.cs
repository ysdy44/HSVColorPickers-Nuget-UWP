using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace HSVColorPickers
{
    /// <summary>
    /// Represents a basic Interface of color picker.
    /// </summary>
    public interface IColorPicker
    {
        /// <summary> Gets picker's type name. </summary>
        string Type { get; }

        /// <summary> Gets picker self. </summary>
        UserControl Self { get; }

        /// <summary> Gets or Sets picker's color. </summary>
        Color Color { get; set; }

        /// <summary> Occurs when the color value changes. </summary>
        event ColorChangeHandler ColorChange;
    }
}
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
        Control Self { get; }

        /// <summary> Gets or sets picker's color. </summary>
        Color Color { get; set; }

        /// <summary> Occurs when the color value changed. </summary>
        event ColorChangeHandler ColorChanged;
        /// <summary> Occurs when the color changed starts. </summary>
        event ColorChangeHandler ColorChangedStarted;
        /// <summary> Occurs when color changed. </summary>
        event ColorChangeHandler ColorChangedDelta;
        /// <summary> Occurs when the color changed is complete. </summary>
        event ColorChangeHandler ColorChangedCompleted;
    }
}
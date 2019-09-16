using Windows.UI.Xaml.Controls;

namespace HSVColorPickers
{
    /// <summary>
    /// Represents a basic Interface of HSV picker.
    /// </summary>
    public interface IHSVPicker
    {
        /// <summary> Gets picker's type name. </summary>
        string Type { get; }

        /// <summary> Gets picker self. </summary>
        UserControl Self { get; }

        /// <summary> Gets or sets picker's hsv. </summary>
        HSV HSV { get; set; }

        /// <summary> Occurs when the hsv value changes. </summary>
        event HSVChangeHandler HSVChange;
    }
}
using Windows.UI.Xaml.Controls;

namespace HSVColorPickers
{
    /// <summary>
    /// Represents a basic Interface of alpha picker.
    /// </summary>
    public interface IAlphaPicker
    {
        /// <summary> Gets picker's type name. </summary>
        string Type { get; }

        /// <summary> Gets picker self. </summary>
        UserControl Self { get; }

        /// <summary> Gets or Sets picker's alpha. </summary>
        byte Alpha { get; set; }

        /// <summary> Occurs when the alpha value changes. </summary>
        event AlphaChangeHandler AlphaChange;
    }
}
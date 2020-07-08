using Windows.UI.Xaml;
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
        Control Self { get; }

        /// <summary> Gets or sets picker's alpha. </summary>
        byte Alpha { get; set; }

        /// <summary> Occurs when the alpha value changed. </summary>
        event AlphaChangeHandler AlphaChanged;
        /// <summary> Occurs when the alpha change starts. </summary>
        event AlphaChangeHandler AlphaChangeStarted;
        /// <summary> Occurs when alpha change. </summary>
        event AlphaChangeHandler AlphaChangeDelta;
        /// <summary> Occurs when the alpha change is complete. </summary>
        event AlphaChangeHandler AlphaChangeCompleted;
    }
}
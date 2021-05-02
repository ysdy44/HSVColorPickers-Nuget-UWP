using Windows.UI.Xaml;
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
        Control Self { get; }

        /// <summary> Gets or sets picker's hsv. </summary>
        HSV HSV { get; set; }

        /// <summary> Occurs when the hsv value changed. </summary>
        event HSVChangeHandler HSVChanged;
        /// <summary> Occurs when the hsv changed starts. </summary>
        event HSVChangeHandler HSVChangeStarted;
        /// <summary> Occurs when hsv changed. </summary>
        event HSVChangeHandler HSVChangeDelta;
        /// <summary> Occurs when the hsv changed is complete. </summary>
        event HSVChangeHandler HSVChangeCompleted;
    }
}
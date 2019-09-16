namespace HSVColorPickers
{
    /// <summary>
    /// Represents PalettePicker from PaletteSaturation.
    /// </summary>
    public class PaletteSaturationPicker : PalettePicker
    {
        /// <summary>
        /// Construct an PaletteSaturationPicker.
        /// </summary>
        public PaletteSaturationPicker() : base(new PaletteSaturation())
        {
        }
    }
}
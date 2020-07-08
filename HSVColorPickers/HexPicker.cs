using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HSVColorPickers
{
    /// <summary>
    /// Hex code picker.
    /// </summary>
    public sealed partial class HexPicker : TextBox
    {
        //@Delegate
        /// <summary> Occurs when the color value changed. </summary>
        public event ColorChangeHandler ColorChanged = null;


        /// <summary> Gets picker's type name. </summary>
        public string Type => "Hex";
        /// <summary> Gets picker self. </summary>
        public Control Self => this;


        /// <summary> Gets or sets picker's color. </summary>
        public Color Color
        {
            get => this.color;
            set
            {
                this.Text = Hex.ColorToString(value).ToUpper();
                this.color = value;
            }
        }
        private Color color = Color.FromArgb(255, 255, 255, 255);


        #region Color


        private Color _Color
        {
            get => this.color;
            set
            {
                this.ColorChanged?.Invoke(this, value);//Delegate

                this.color = value;
            }
        }


        #endregion

        string _text;
        //@Construct
        /// <summary>
        /// Construct a HexPicker.
        /// </summary>
        public HexPicker()
        {
            this.GotFocus += (s, e) => this._text = this.Text.ToUpper();
            this.LostFocus += (s, e) =>
            {
                string text = this.Text.ToUpper();
                if (this._text == text) return;

                this.Color = this._Color = this.TextHex(text);
            };
        }

        private Color TextHex(string text)
        {
            string hex = Hex.TextSubstring(text);

            if (hex == null) return this.color;

            try
            {
                return Hex.IntToColor(Hex.StringToInt(hex));
            }
            catch (Exception)
            {
                return this.color;
            }
        }
    }
}
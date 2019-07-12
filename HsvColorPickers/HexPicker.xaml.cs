using System;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace HSVColorPickers
{    
    /// <summary>
    /// Hex code picker.
    /// </summary>
    public sealed partial class HexPicker : UserControl,IColorPicker
    {
        //@Delegate
        /// <summary> Occurs when the color value changes. </summary>
        public event ColorChangeHandler ColorChange = null;


        /// <summary> Gets picker's type name. </summary>
        public string Type => "Hex";
        /// <summary> Gets picker self. </summary>
        public UserControl Self => this;

        /// <summary> Gets or Sets picker's color. </summary>
        public Color Color
        {
            get => this.color;
            set
            {
                this.TextBox.Text = Hex.ColorToString(value).ToUpper();
                this.color = value;
            }
        }

        private Color color = Color.FromArgb(255, 255, 255, 255);
        private Color _Color
        {
            get => this.color;
            set
            {
                this.ColorChange?.Invoke(this, value);//Delegate

                this.color = value;
            }
        }


        //@Construct
        /// <summary>
        /// Construct a HexPicker.
        /// </summary>
        public HexPicker()
        {
            this.InitializeComponent();

            this.TextBox.GotFocus += (s, e) => { };
            this.TextBox.LostFocus += (s, e) => this.Color = this._Color = this.TextHex(this.TextBox.Text);
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
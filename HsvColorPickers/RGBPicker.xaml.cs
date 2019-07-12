using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace HSVColorPickers
{
    /// <summary>
    /// RGBPicker:
    ///    RGB of color picker.
    /// </summary>
    public sealed partial class RGBPicker : UserControl, IColorPicker
    {
        //@Delegate
        /// <summary> Occurs when the color value changes. </summary>
        public event ColorChangeHandler ColorChange = null;


        /// <summary> Gets picker's type name. </summary>
        public string Type => "RGB";
        /// <summary> Gets picker self. </summary>
        public UserControl Self => this;

        /// <summary> Get or set the current hsv for a rgb picker. </summary>
        public Color Color
        {
            get => this.color;
            set
            {
                //R
                this.RSlider.Value = value.R;
                this.RPicker.Value = value.R;
                this.RLeft.Color = Color.FromArgb(255, 0, value.G, value.B);
                this.RRight.Color = Color.FromArgb(255, 255, value.G, value.B);

                //G
                this.GSlider.Value = value.G;
                this.GPicker.Value = value.G;
                this.GLeft.Color = Color.FromArgb(255, value.R, 0, value.B);
                this.GRight.Color = Color.FromArgb(255, value.R, 255, value.B);

                //B
                this.BSlider.Value = value.B;
                this.BPicker.Value = value.B;
                this.BLeft.Color = Color.FromArgb(255, value.R, value.G, 0);
                this.BRight.Color = Color.FromArgb(255, value.R, value.G, 255);

                this.color = value;
            }
        }


        #region DependencyProperty


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


        #endregion


        //@Construct
        /// <summary>
        /// Construct a RGBPicker.
        /// </summary>
        public RGBPicker()
        {
            this.InitializeComponent();

            //Slider
            this.RSlider.ValueChangeDelta += (sender, value) => this.Color = this._Color = Color.FromArgb(this.color.A, (byte)value, this.color.G, this.color.B);
            this.GSlider.ValueChangeDelta += (sender, value) => this.Color = this._Color = Color.FromArgb(this.color.A, this.color.R, (byte)value, this.color.B);
            this.BSlider.ValueChangeDelta += (sender, value) => this.Color = this._Color = Color.FromArgb(this.color.A, this.color.R, this.color.G, (byte)value);

            //Picker
            this.RPicker.ValueChange += (sender, Value) => this.Color = this._Color = Color.FromArgb(this.color.A, (byte)Value, this.color.G, this.color.B);
            this.GPicker.ValueChange += (sender, Value) => this.Color = this._Color = Color.FromArgb(this.color.A, this.color.R, (byte)Value, this.color.B);
            this.BPicker.ValueChange += (sender, Value) => this.Color = this._Color = Color.FromArgb(this.color.A, this.color.R, this.color.G, (byte)Value);
        }
    }
}
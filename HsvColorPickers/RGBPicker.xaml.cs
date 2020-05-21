using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace HSVColorPickers
{
    /// <summary>
    /// RGBPicker:
    ///    RGB of color picker.
    /// </summary>
    public sealed partial class RGBPicker : UserControl, IColorPicker
    {
        //@Delegate
        /// <summary> Occurs when the color value changed. </summary>
        public event ColorChangeHandler ColorChanged;
        /// <summary> Occurs when the color change starts. </summary>
        public event ColorChangeHandler ColorChangeStarted;
        /// <summary> Occurs when color change. </summary>
        public event ColorChangeHandler ColorChangeDelta;
        /// <summary> Occurs when the color change is complete. </summary>
        public event ColorChangeHandler ColorChangeCompleted;


        /// <summary> Gets picker's type name. </summary>
        public string Type => "RGB";
        /// <summary> Gets picker self. </summary>
        public UserControl Self => this;


        #region Color


        /// <summary> Get or set the current hsv for a rgb picker. </summary>
        public Color Color
        {
            get => this.color;
            set
            {
                this.Change(value);
                this.color = value;
            }
        }
        private Color color = Color.FromArgb(255, 255, 255, 255);


        private Color _Color
        {
            get => this.color;
            set
            {
                this.ColorChanged?.Invoke(this, value);//Delegate

                this.color = value;
            }
        }
        private Color _ColorStarted
        {
            get => this.color;
            set
            {
                this.ColorChangeStarted?.Invoke(this, value);//Delegate

                this.color = value;
            }
        }
        private Color _ColorDelta
        {
            get => this.color;
            set
            {
                this.ColorChangeDelta?.Invoke(this, value);//Delegate

                this.color = value;
            }
        }
        private Color _ColorCompleted
        {
            get => this.color;
            set
            {
                this.ColorChangeCompleted?.Invoke(this, value);//Delegate

                this.color = value;
            }
        }


        #endregion

        #region DependencyProperty


        /// <summary> Get or set the button style. </summary>
        public Style ButtonStyle
        {
            get { return (Style)GetValue(ButtonStyleProperty); }
            set { SetValue(ButtonStyleProperty, value); }
        }
        /// <summary> Identifies the <see cref = "WheelPicker.ButtonStyle" /> dependency property. </summary>
        public static readonly DependencyProperty ButtonStyleProperty = DependencyProperty.Register(nameof(ButtonStyle), typeof(Style), typeof(WheelPicker), new PropertyMetadata(null));


        /// <summary> Get or set the flyout style. </summary>
        public Style FlyoutPresenterStyle
        {
            get { return (Style)GetValue(FlyoutPresenterStyleProperty); }
            set { SetValue(FlyoutPresenterStyleProperty, value); }
        }
        /// <summary> Identifies the <see cref = "WheelPicker.FlyoutPresenterStyle" /> dependency property. </summary>
        public static readonly DependencyProperty FlyoutPresenterStyleProperty = DependencyProperty.Register(nameof(FlyoutPresenterStyle), typeof(Style), typeof(WheelPicker), new PropertyMetadata(null));


        /// <summary> Get or set the flyout placement. </summary>
        public FlyoutPlacementMode Placement
        {
            get { return (FlyoutPlacementMode)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }
        /// <summary> Identifies the <see cref = "WheelPicker.Placement" /> dependency property. </summary>
        public static readonly DependencyProperty PlacementProperty = DependencyProperty.Register(nameof(Placement), typeof(FlyoutPlacementMode), typeof(RGBPicker), new PropertyMetadata(FlyoutPlacementMode.Bottom));


        /// <summary>  Gets or sets a brush that describes the border fill of the control. </summary>
        public SolidColorBrush Stroke
        {
            get { return (SolidColorBrush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        /// <summary> Identifies the <see cref = "WheelPicker.Stroke" /> dependency property. </summary>
        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(nameof(Stroke), typeof(SolidColorBrush), typeof(WheelPicker), new PropertyMetadata(new SolidColorBrush(Windows.UI.Colors.Gray)));


        #endregion


        //@Construct
        /// <summary>
        /// Construct a RGBPicker.
        /// </summary>
        public RGBPicker()
        {
            this.InitializeComponent();

            //Slider
            this.RSlider.ValueChangeStarted += (sender, value) => this._ColorStarted = this.Change((byte)value, RGBMode.NotR, false);
            this.RSlider.ValueChangeDelta += (sender, value) => this._ColorDelta = this.Change((byte)value, RGBMode.NotR, false);
            this.RSlider.ValueChangeCompleted += (sender, value) => this._ColorCompleted = this.Change((byte)value, RGBMode.NotR, false);

            this.GSlider.ValueChangeStarted += (sender, value) => this._ColorStarted = this.Change((byte)value, RGBMode.NotG, false);
            this.GSlider.ValueChangeDelta += (sender, value) => this._ColorDelta = this.Change((byte)value, RGBMode.NotG, false);
            this.GSlider.ValueChangeCompleted += (sender, value) => this._ColorCompleted = this.Change((byte)value, RGBMode.NotG, false);

            this.BSlider.ValueChangeStarted += (sender, value) => this._ColorStarted = this.Change((byte)value, RGBMode.NotB, false);
            this.BSlider.ValueChangeDelta += (sender, value) => this._ColorDelta = this.Change((byte)value, RGBMode.NotB, false);
            this.BSlider.ValueChangeCompleted += (sender, value) => this._ColorCompleted = this.Change((byte)value, RGBMode.NotB, false);

            //Picker
            this.RPicker.ValueChanged += (sender, value) => this._Color = this.Change((byte)value, RGBMode.NotR, true);
            this.GPicker.ValueChanged += (sender, value) => this._Color = this.Change((byte)value, RGBMode.NotG, true);
            this.BPicker.ValueChanged += (sender, value) => this._Color = this.Change((byte)value, RGBMode.NotB, true);
        }

        #region Change


        private Color Change(byte value, RGBMode RGBMode = RGBMode.All, bool? sliderOrPicker = null)
        {
            Color color = this.color;

            switch (RGBMode)
            {
                case RGBMode.NotR: color.R = value; break;
                case RGBMode.NotG: color.G = value; break;
                case RGBMode.NotB: color.B = value; break;
            }

            this.Change(color, RGBMode, sliderOrPicker);
            return color;
        }

        private void Change(Color color, RGBMode RGBMode = RGBMode.All, bool? sliderOrPicker = null)
        {
            byte R = color.R;
            byte G = color.G;
            byte B = color.B;

            //Brush
            {
                if (RGBMode != RGBMode.NotR)
                {
                    this.RLeft.Color = Color.FromArgb(255, 0, G, B);
                    this.RRight.Color = Color.FromArgb(255, 255, G, B);
                }
                if (RGBMode != RGBMode.NotG)
                {
                    this.GLeft.Color = Color.FromArgb(255, R, 0, B);
                    this.GRight.Color = Color.FromArgb(255, R, 255, B);
                }
                if (RGBMode != RGBMode.NotB)
                {
                    this.BLeft.Color = Color.FromArgb(255, R, G, 0);
                    this.BRight.Color = Color.FromArgb(255, R, G, 255);
                }
            }


            //Value
            {
                if (RGBMode == RGBMode.All || RGBMode == RGBMode.NotR)
                {
                    if (sliderOrPicker != false) this.RSlider.Value = R;
                    if (sliderOrPicker != true) this.RPicker.Value = (int)R;
                }
                if (RGBMode == RGBMode.All || RGBMode == RGBMode.NotG)
                {
                    if (sliderOrPicker != false) this.GSlider.Value = G;
                    if (sliderOrPicker != true) this.GPicker.Value = (int)G;
                }
                if (RGBMode == RGBMode.All || RGBMode == RGBMode.NotB)
                {
                    if (sliderOrPicker != false) this.BSlider.Value = B;
                    if (sliderOrPicker != true) this.BPicker.Value = (int)B;
                }
            }
        }


        #endregion

    }
}
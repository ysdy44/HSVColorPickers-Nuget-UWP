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
        public event ColorChangedHandler ColorChanged;
        /// <summary> Occurs when the color changed starts. </summary>
        public event ColorChangedHandler ColorChangedStarted;
        /// <summary> Occurs when color changed. </summary>
        public event ColorChangedHandler ColorChangedDelta;
        /// <summary> Occurs when the color changed is complete. </summary>
        public event ColorChangedHandler ColorChangedCompleted;


        /// <summary> Gets picker's type name. </summary>
        public string Type => "RGB";
        /// <summary> Gets picker self. </summary>
        public Control Self => this;


        #region Color


        /// <summary> Get or set the current hsv for a rgb picker. </summary>
        public Color Color
        {
            get => this.color;
            set
            {
                this.Changed(value);
                this.color = value;
            }
        }
        private Color color = Color.FromArgb(255, 255, 255, 255);


        private void OnColorChanged(Color value)
        {
            this.ColorChanged?.Invoke(this, value);//Delegate

            this.color = value;
        }
        private void OnColorChangedStarted(Color value)
        {
            this.ColorChangedStarted?.Invoke(this, value);//Delegate

            this.color = value;
        }
        private void OnColorChangedDelta(Color value)
        {
            this.ColorChangedDelta?.Invoke(this, value);//Delegate

            this.color = value;
        }
        private void OnColorChangedCompleted(Color value)
        {
            this.ColorChangedCompleted?.Invoke(this, value);//Delegate

            this.color = value;
        }


        #endregion

        #region DependencyProperty


        /// <summary> Get or set the text style. </summary>
        public Style TextStyle
        {
            get => (Style)base.GetValue(TextStyleProperty);
            set => base.SetValue(TextStyleProperty, value);
        }
        /// <summary> Identifies the <see cref = "RGBPicker.ButtonStyle" /> dependency property. </summary>
        public static readonly DependencyProperty TextStyleProperty = DependencyProperty.Register(nameof(TextStyle), typeof(Style), typeof(RGBPicker), new PropertyMetadata(null));


        /// <summary> Get or set the button style. </summary>
        public Style ButtonStyle
        {
            get => (Style)base.GetValue(ButtonStyleProperty);
            set => base.SetValue(TextStyleProperty, value);
        }
        /// <summary> Identifies the <see cref = "RGBPicker.ButtonStyle" /> dependency property. </summary>
        public static readonly DependencyProperty ButtonStyleProperty = DependencyProperty.Register(nameof(ButtonStyle), typeof(Style), typeof(RGBPicker), new PropertyMetadata(null));


        /// <summary> Get or set the flyout style. </summary>
        public Style FlyoutPresenterStyle
        {
            get => (Style)base.GetValue(FlyoutPresenterStyleProperty);
            set => base.SetValue(FlyoutPresenterStyleProperty, value);
        }
        /// <summary> Identifies the <see cref = "RGBPicker.FlyoutPresenterStyle" /> dependency property. </summary>
        public static readonly DependencyProperty FlyoutPresenterStyleProperty = DependencyProperty.Register(nameof(FlyoutPresenterStyle), typeof(Style), typeof(RGBPicker), new PropertyMetadata(null));


        /// <summary> Get or set the flyout placement. </summary>
        public FlyoutPlacementMode Placement
        {
            get => (FlyoutPlacementMode)base.GetValue(PlacementProperty);
            set => base.SetValue(FlyoutPresenterStyleProperty, value);
        }
        /// <summary> Identifies the <see cref = "RGBPicker.Placement" /> dependency property. </summary>
        public static readonly DependencyProperty PlacementProperty = DependencyProperty.Register(nameof(Placement), typeof(FlyoutPlacementMode), typeof(RGBPicker), new PropertyMetadata(FlyoutPlacementMode.Bottom));


        /// <summary>  Gets or sets a brush that describes the border fill of the control. </summary>
        public SolidColorBrush Stroke
        {
            get => (SolidColorBrush)base.GetValue(StrokeProperty);
            set => base.SetValue(StrokeProperty, value);
        }
        /// <summary> Identifies the <see cref = "RGBPicker.Stroke" /> dependency property. </summary>
        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(nameof(Stroke), typeof(SolidColorBrush), typeof(RGBPicker), new PropertyMetadata(new SolidColorBrush(Windows.UI.Colors.Gray)));


        #endregion


        //@Construct
        /// <summary>
        /// Construct a RGBPicker.
        /// </summary>
        public RGBPicker()
        {
            this.InitializeComponent();


            //R
            this.RPicker.Unit = null;
            this.RPicker.Minimum = 0;
            this.RPicker.Maximum = 255;
            this.RPicker.ValueChanged += (sender, value) => this.OnColorChanged(this.Changed((byte)value, RGBMode.NotR, true));

            this.RSlider.Minimum = 0.0d;
            this.RSlider.Maximum = 255.0d;
            this.RSlider.ValueChangeStarted += (sender, value) => this.OnColorChangedStarted(this.Changed((byte)value, RGBMode.NotR, false));
            this.RSlider.ValueChangeDelta += (sender, value) => this.OnColorChangedDelta(this.Changed((byte)value, RGBMode.NotR, false));
            this.RSlider.ValueChangeCompleted += (sender, value) => this.OnColorChangedCompleted(this.Changed((byte)value, RGBMode.NotR, false));


            //G
            this.GPicker.Unit = null;
            this.GPicker.Minimum = 0;
            this.GPicker.Maximum = 255;
            this.GPicker.ValueChanged += (sender, value) => this.OnColorChanged(this.Changed((byte)value, RGBMode.NotG, true));

            this.GSlider.Minimum = 0.0d;
            this.GSlider.Maximum = 255.0d;
            this.GSlider.ValueChangeStarted += (sender, value) => this.OnColorChangedStarted(this.Changed((byte)value, RGBMode.NotG, false));
            this.GSlider.ValueChangeDelta += (sender, value) => this.OnColorChangedDelta(this.Changed((byte)value, RGBMode.NotG, false));
            this.GSlider.ValueChangeCompleted += (sender, value) => this.OnColorChangedCompleted(this.Changed((byte)value, RGBMode.NotG, false));


            //B
            this.BPicker.Unit = null;
            this.BPicker.Minimum = 0;
            this.BPicker.Maximum = 255;
            this.BPicker.ValueChanged += (sender, value) => this.OnColorChanged(this.Changed((byte)value, RGBMode.NotB, true));

            this.BSlider.Minimum = 0.0d;
            this.BSlider.Maximum = 255.0d;
            this.BSlider.ValueChangeStarted += (sender, value) => this.OnColorChangedStarted(this.Changed((byte)value, RGBMode.NotB, false));
            this.BSlider.ValueChangeDelta += (sender, value) => this.OnColorChangedDelta(this.Changed((byte)value, RGBMode.NotB, false));
            this.BSlider.ValueChangeCompleted += (sender, value) => this.OnColorChangedCompleted(this.Changed((byte)value, RGBMode.NotB, false));
        }

        #region Changed


        private Color Changed(byte value, RGBMode RGBMode = RGBMode.All, bool? sliderOrPicker = null)
        {
            Color color = this.color;

            switch (RGBMode)
            {
                case RGBMode.NotR: color.R = value; break;
                case RGBMode.NotG: color.G = value; break;
                case RGBMode.NotB: color.B = value; break;
            }

            this.Changed(color, RGBMode, sliderOrPicker);
            return color;
        }

        private void Changed(Color color, RGBMode RGBMode = RGBMode.All, bool? sliderOrPicker = null)
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
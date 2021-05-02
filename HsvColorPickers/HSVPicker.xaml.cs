using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace HSVColorPickers
{
    /// <summary>
    /// HSV picker.
    /// </summary>
    public sealed partial class HSVPicker : UserControl, IColorPicker, IHSVPicker
    {
        //@Delegate
        /// <summary> Occurs when the color value changed. </summary>
        public event ColorChangeHandler ColorChanged;
        /// <summary> Occurs when the color changed starts. </summary>
        public event ColorChangeHandler ColorChangedStarted;
        /// <summary> Occurs when color changed. </summary>
        public event ColorChangeHandler ColorChangedDelta;
        /// <summary> Occurs when the color changed is complete. </summary>
        public event ColorChangeHandler ColorChangedCompleted;
        /// <summary> Occurs when the hsv value changed. </summary>
        public event HSVChangeHandler HSVChanged;
        /// <summary> Occurs when the hsv changed starts. </summary>
        public event HSVChangeHandler HSVChangeStarted;
        /// <summary> Occurs when hsv changed. </summary>
        public event HSVChangeHandler HSVChangeDelta;
        /// <summary> Occurs when the hsv changed is complete. </summary>
        public event HSVChangeHandler HSVChangeCompleted;


        /// <summary> Gets picker's type name. </summary>
        public string Type => "HSV";
        /// <summary> Gets picker self. </summary>
        public Control Self => this;


        #region Color


        /// <summary> Gets or sets picker's color. </summary>
        public Color Color
        {
            get => HSV.HSVtoRGB(this.HSV);
            set => this.HSV = HSV.RGBtoHSV(value);
        }
        /// <summary> Gets or sets picker's hsv. </summary>
        public HSV HSV
        {
            get => this.hsv;
            set
            {
                this.Changed(value);
                this.hsv = value;
            }
        }
        private HSV hsv = new HSV(255, 0, 100, 100);


        private void OnHSVChanged(HSV value)
        {
            this.ColorChanged?.Invoke(this, HSV.HSVtoRGB(value.A, value.H, value.S, value.V));//Delegate
            this.HSVChanged?.Invoke(this, value);//Delegate

            this.hsv = value;
        }
        private void OnHSVChangeStarted(HSV value)
        {
            this.ColorChangedStarted?.Invoke(this, HSV.HSVtoRGB(value.A, value.H, value.S, value.V));//Delegate
            this.HSVChangeStarted?.Invoke(this, value);//Delegate

            this.hsv = value;
        }
        private void OnHSVChangeDelta(HSV value)
        {
            this.ColorChangedDelta?.Invoke(this, HSV.HSVtoRGB(value.A, value.H, value.S, value.V));//Delegate
            this.HSVChangeDelta?.Invoke(this, value);//Delegate

            this.hsv = value;
        }
        private void OnHSVChangeCompleted(HSV value)
        {
            this.ColorChangedCompleted?.Invoke(this, HSV.HSVtoRGB(value.A, value.H, value.S, value.V));//Delegate
            this.HSVChangeCompleted?.Invoke(this, value);//Delegate

            this.hsv = value;
        }


        #endregion

        #region DependencyProperty


        /// <summary> Get or set the text style. </summary>
        public Style TextStyle
        {
            get { return (Style)GetValue(TextStyleProperty); }
            set { SetValue(TextStyleProperty, value); }
        }
        /// <summary> Identifies the <see cref = "HSVPicker.ButtonStyle" /> dependency property. </summary>
        public static readonly DependencyProperty TextStyleProperty = DependencyProperty.Register(nameof(TextStyle), typeof(Style), typeof(HSVPicker), new PropertyMetadata(null));


        /// <summary> Get or set the button style. </summary>
        public Style ButtonStyle
        {
            get { return (Style)GetValue(ButtonStyleProperty); }
            set { SetValue(ButtonStyleProperty, value); }
        }
        /// <summary> Identifies the <see cref = "HSVPicker.ButtonStyle" /> dependency property. </summary>
        public static readonly DependencyProperty ButtonStyleProperty = DependencyProperty.Register(nameof(ButtonStyle), typeof(Style), typeof(HSVPicker), new PropertyMetadata(null));


        /// <summary> Get or set the flyout style. </summary>
        public Style FlyoutPresenterStyle
        {
            get { return (Style)GetValue(FlyoutPresenterStyleProperty); }
            set { SetValue(FlyoutPresenterStyleProperty, value); }
        }
        /// <summary> Identifies the <see cref = "HSVPicker.FlyoutPresenterStyle" /> dependency property. </summary>
        public static readonly DependencyProperty FlyoutPresenterStyleProperty = DependencyProperty.Register(nameof(FlyoutPresenterStyle), typeof(Style), typeof(HSVPicker), new PropertyMetadata(null));


        /// <summary> Get or set the flyout placement. </summary>
        public FlyoutPlacementMode Placement
        {
            get { return (FlyoutPlacementMode)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }
        /// <summary> Identifies the <see cref = "HSVPicker.Placement" /> dependency property. </summary>
        public static readonly DependencyProperty PlacementProperty = DependencyProperty.Register(nameof(Placement), typeof(FlyoutPlacementMode), typeof(HSVPicker), new PropertyMetadata(FlyoutPlacementMode.Bottom));


        /// <summary>  Gets or sets a brush that describes the border fill of the control. </summary>
        public SolidColorBrush Stroke
        {
            get { return (SolidColorBrush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        /// <summary> Identifies the <see cref = "HSVPicker.Stroke" /> dependency property. </summary>
        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(nameof(Stroke), typeof(SolidColorBrush), typeof(HSVPicker), new PropertyMetadata(new SolidColorBrush(Windows.UI.Colors.Gray)));


        #endregion


        //@Construct
        /// <summary>
        /// Construct a HSVPicker.
        /// </summary>
        public HSVPicker()
        {
            this.InitializeComponent();


            //H
            this.HPicker.Unit = "º";
            this.HPicker.Minimum = 0;
            this.HPicker.Maximum = 360;
            this.HPicker.ValueChanged += (sender, value) => this.OnHSVChanged(this.Changed(value, HSVMode.NotH, true));

            this.HSlider.Minimum = 0.0d;
            this.HSlider.Maximum = 360.0d;
            this.HSlider.ValueChangeStarted += (sender, value) => this.OnHSVChangeStarted(this.Changed((float)value, HSVMode.NotH, false));
            this.HSlider.ValueChangeDelta += (sender, value) => this.OnHSVChangeDelta(this.Changed((float)value, HSVMode.NotH, false));
            this.HSlider.ValueChangeCompleted += (sender, value) => this.OnHSVChangeCompleted(this.Changed((float)value, HSVMode.NotH, false));


            //S
            this.SPicker.Unit = "%";
            this.SPicker.Minimum = 0;
            this.SPicker.Maximum = 100;
            this.SPicker.ValueChanged += (sender, value) => this.OnHSVChanged(this.Changed(value, HSVMode.NotS, true));

            this.SSlider.Minimum = 0.0d;
            this.SSlider.Maximum = 100.0d;
            this.SSlider.ValueChangeStarted += (sender, value) => this.OnHSVChangeStarted(this.Changed((float)value, HSVMode.NotS, false));
            this.SSlider.ValueChangeDelta += (sender, value) => this.OnHSVChangeDelta(this.Changed((float)value, HSVMode.NotS, false));
            this.SSlider.ValueChangeCompleted += (sender, value) => this.OnHSVChangeCompleted(this.Changed((float)value, HSVMode.NotS, false));


            //V
            this.VPicker.Unit = "%";
            this.VPicker.Minimum = 0;
            this.VPicker.Maximum = 100;
            this.VPicker.ValueChanged += (sender, value) => this.OnHSVChanged(this.Changed(value, HSVMode.NotV, true));

            this.VSlider.Minimum = 0.0d;
            this.VSlider.Maximum = 100.0d;
            this.VSlider.ValueChangeStarted += (sender, value) => this.OnHSVChangeStarted(this.Changed((float)value, HSVMode.NotV, false));
            this.VSlider.ValueChangeDelta += (sender, value) => this.OnHSVChangeDelta(this.Changed((float)value, HSVMode.NotV, false));
            this.VSlider.ValueChangeCompleted += (sender, value) => this.OnHSVChangeCompleted(this.Changed((float)value, HSVMode.NotV, false));
        }

        #region Changed


        private HSV Changed(float value, HSVMode hsvMode = HSVMode.All, bool? sliderOrPicker = null)
        {
            HSV hsv = this.hsv;

            switch (hsvMode)
            {
                case HSVMode.NotH: hsv.H = value; break;
                case HSVMode.NotS: hsv.S = value; break;
                case HSVMode.NotV: hsv.V = value; break;
            }

            this.Changed(hsv, hsvMode, sliderOrPicker);
            return hsv;
        }

        private void Changed(HSV hsv, HSVMode hsvMode = HSVMode.All, bool? sliderOrPicker = null)
        {
            float H = hsv.H;
            float S = hsv.S;
            float V = hsv.V;

            //Brush
            {
                if (hsvMode != HSVMode.NotH)
                {
                    this.HG.Color = this.HA.Color = HSV.HSVtoRGB(255, 0, S, V);
                    this.HB.Color = HSV.HSVtoRGB(255, 60, S, V);
                    this.HC.Color = HSV.HSVtoRGB(255, 120, S, V);
                    this.HD.Color = HSV.HSVtoRGB(255, 180, S, V);
                    this.HE.Color = HSV.HSVtoRGB(255, 240, S, V);
                    this.HF.Color = HSV.HSVtoRGB(255, 300, S, V);
                }
                if (hsvMode != HSVMode.NotS)
                {
                    this.SLeft.Color = HSV.HSVtoRGB(255, H, 0.0f, V);
                    this.SRight.Color = HSV.HSVtoRGB(255, H, 100.0f, V);
                }
                if (hsvMode != HSVMode.NotV)
                {
                    this.VLeft.Color = HSV.HSVtoRGB(255, H, S, 0.0f);
                    this.VRight.Color = HSV.HSVtoRGB(255, H, S, 100.0f);
                }
            }


            //Value
            {
                if (hsvMode == HSVMode.All || hsvMode == HSVMode.NotH)
                {
                    if (sliderOrPicker != false) this.HSlider.Value = H;
                    if (sliderOrPicker != true) this.HPicker.Value = (int)H;
                }
                if (hsvMode == HSVMode.All || hsvMode == HSVMode.NotS)
                {
                    if (sliderOrPicker != false) this.SSlider.Value = S;
                    if (sliderOrPicker != true) this.SPicker.Value = (int)S;
                }
                if (hsvMode == HSVMode.All || hsvMode == HSVMode.NotV)
                {
                    if (sliderOrPicker != false) this.VSlider.Value = V;
                    if (sliderOrPicker != true) this.VPicker.Value = (int)V;
                }
            }
        }


        #endregion

    }
}
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
        /// <summary> Occurs when the color value changes. </summary>
        public event ColorChangeHandler ColorChange = null;
        /// <summary> Occurs when the hsv value changes. </summary>
        public event HSVChangeHandler HSVChange = null;


        /// <summary> Gets picker's type name. </summary>
        public string Type => "HSV";
        /// <summary> Gets picker self. </summary>
        public UserControl Self => this;


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
                this.Change(value);
                this.hsv = value;            
            }
        }
        private HSV hsv = new HSV(255, 0, 100, 100);
               

        private HSV _HSV
        {
            set
            {
                this.ColorChange?.Invoke(this, HSV.HSVtoRGB(value.A, value.H, value.S, value.V));//Delegate
                this.HSVChange?.Invoke(this, value);//Delegate

                this.hsv = value;
            }
        }
        

        #endregion
        
        #region DependencyProperty


        /// <summary> Get or set the flyout style. </summary>
        public Style FlyoutPresenterStyle
        {
            get { return (Style)GetValue(FlyoutPresenterStyleProperty); }
            set { SetValue(FlyoutPresenterStyleProperty, value); }
        }
        /// <summary> Identifies the <see cref = "NumberPicker.FlyoutPresenterStyle" /> dependency property. </summary>
        public static readonly DependencyProperty FlyoutPresenterStyleProperty = DependencyProperty.Register(nameof(FlyoutPresenterStyle), typeof(Style), typeof(HSVPicker), new PropertyMetadata(null));


        /// <summary> Get or set the flyout placement. </summary>
        public FlyoutPlacementMode Placement
        {
            get { return (FlyoutPlacementMode)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }
        /// <summary> Identifies the <see cref = "NumberPicker.Placement" /> dependency property. </summary>
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

            //Slider
            this.HSlider.ValueChangeDelta += (sender, value) => this._HSV = this.Change((float)value, HSVMode.NotH, false);
            this.SSlider.ValueChangeDelta += (sender, value) => this._HSV = this.Change((float)value, HSVMode.NotS, false);
            this.VSlider.ValueChangeDelta += (sender, value) => this._HSV = this.Change((float)value, HSVMode.NotV, false);

            //Picker
            this.HPicker.ValueChange += (sender, value) => this._HSV = this.Change(value, HSVMode.NotH, true);
            this.SPicker.ValueChange += (sender, value) => this._HSV = this.Change(value, HSVMode.NotS, true);
            this.VPicker.ValueChange += (sender, value) => this._HSV = this.Change(value, HSVMode.NotV, true);
        }

        #region Change


        private HSV Change(float value, HSVMode hsvMode = HSVMode.All, bool? sliderOrPicker = null)
        {
            HSV hsv = this.hsv;

            switch (hsvMode)
            {
                case HSVMode.NotH: hsv.H = value; break;
                case HSVMode.NotS: hsv.S = value; break;
                case HSVMode.NotV: hsv.V = value; break;
            }

            this.Change(hsv, hsvMode, sliderOrPicker);
            return hsv;
        }
         
        private void Change(HSV hsv, HSVMode hsvMode = HSVMode.All, bool? sliderOrPicker = null)
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
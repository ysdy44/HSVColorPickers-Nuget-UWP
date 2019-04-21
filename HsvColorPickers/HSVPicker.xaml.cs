using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace HSVColorPickers
{
    /// <summary>
    /// HSVPicker:
    ///    HSV of color picker.
    /// </summary>
    public sealed partial class HSVPicker : UserControl, IPicker
    {

        //Delegate
        public event ColorChangeHandler ColorChange = null;
        public Color GetColor() => HSV.HSVtoRGB(this.HSV);
        public void SetColor(Color value) => this.HSV = HSV.RGBtoHSV(value);


        #region DependencyProperty


        private HSV hsl = new HSV { A = 255, H = 0, S = 1, V = 1 };
        private HSV _HSL
        {
            get => this.hsl;
            set
            {
                this.ColorChange?.Invoke(this, HSV.HSVtoRGB(value.A, value.H, value.S, value.V));

                this.hsl = value;
            }
        }
        public HSV HSV
        {
            get => this.hsl;
            set
            {
                byte A = value.A;
                float H = value.H;
                float S = value.S;
                float L = value.V;

                //H          
                this.HSlider.Value = this.HPicker.Value = (int)H;
                this.HG.Color = this.HA.Color = HSV.HSVtoRGB(A, 0, S, L);
                this.HB.Color = HSV.HSVtoRGB(A, 60, S, L);
                this.HC.Color = HSV.HSVtoRGB(A, 120, S, L);
                this.HD.Color = HSV.HSVtoRGB(A, 180, S, L);
                this.HE.Color = HSV.HSVtoRGB(A, 240, S, L);
                this.HF.Color = HSV.HSVtoRGB(A, 300, S, L);
                //S
                this.SSlider.Value = SPicker.Value = (int)S;
                this.SLeft.Color = HSV.HSVtoRGB(A, H, 0.0f, L);
                this.SRight.Color = HSV.HSVtoRGB(A, H, 100.0f, L);
                //L
                this.VSlider.Value = VPicker.Value = (int)L;
                this.VLeft.Color = HSV.HSVtoRGB(A, H, S, 0.0f);
                this.VRight.Color = HSV.HSVtoRGB(A, H, S, 100.0f);

                this.hsl = value;
            }
        }
        public Color Color
        {
            get => this.GetColor();
            set => this.SetColor(value);
        }


        #endregion


        public HSVPicker()
        {
            this.InitializeComponent();

            //Slider
            this.HSlider.ValueChangeDelta += (sender, value) => this.HSV = this._HSL = new HSV(this.hsl.A, (float)value, this.hsl.S, this.hsl.V);
            this.SSlider.ValueChangeDelta += (sender, value) => this.HSV = this._HSL = new HSV(this.hsl.A, this.hsl.H, (float)value, this.hsl.V);
            this.VSlider.ValueChangeDelta += (sender, value) => this.HSV = this._HSL = new HSV(this.hsl.A, this.hsl.H, this.hsl.S, (float)value);

            //Picker
            this.HPicker.ValueChange += (sender, Value) => this.HSV = this._HSL = new HSV(this.hsl.A, Value, this.hsl.S, this.HSV.V);
            this.SPicker.ValueChange += (sender, Value) => this.HSV = this._HSL = new HSV(this.hsl.A, this.hsl.H, Value, this.hsl.V);
            this.VPicker.ValueChange += (sender, Value) => this.HSV = this._HSL = new HSV(this.hsl.A, this.hsl.H, this.hsl.S, Value);
        }
    }
}

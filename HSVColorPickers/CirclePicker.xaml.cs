using System;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
#if WINDOWS_PHONE
using System.Windows.Media;
using System.Windows.Media.Imaging;
#else
#endif

namespace HSVColorPickers
{
    /// <summary>
    /// Pick a color in the circle hue wheel.
    /// </summary>
    public sealed partial class CirclePicker : UserControl, IColorPicker, IHSVPicker
    {

        #region Helpher


        private sealed class CircleSize
        {
            public static float VectorToH(Vector2 vector) => ((((float)Math.Atan2(vector.Y, vector.X)) * 180.0f / (float)Math.PI) + 360.0f) % 360.0f;
            public static Vector2 HSToVector(float h, float s, float radio, Vector2 center) => new Vector2((float)Math.Cos(h), (float)Math.Sin(h)) * radio * s / 100.0f + center;
            public static float VectorToS(Vector2 vector, float radio)
            {
                float s = vector.Length() / radio;
                if (s < 0) return 0.0f;
                if (s > 1) return 100.0f;
                return s * 100.0f;
            }
        }


        private static class HueWheelHelpher
        {
            public static Task CreateHueCircle(WriteableBitmap bmp)
            {
                return HueWheelHelpher.FillBitmap(bmp, (x, y) =>
                {
                    return HueWheelHelpher.CalcWheelColor(x, y);
                });
            }

            public static async Task FillBitmap(WriteableBitmap bmp, Func<float, float, Color> fillPixel)
            {
#if WINDOWS_PHONE
#else
                var stream = bmp.PixelBuffer.AsStream();
#endif
                int width = bmp.PixelWidth;
                int height = bmp.PixelHeight;
                await Task.Run(() =>
                {
                    for (int y = 0; y < width; y++)
                    {
                        for (int x = 0; x < height; x++)
                        {
                            var color = fillPixel((float)x / width, (float)y / height);

#if WINDOWS_PHONE
                        bmp.Pixels[x + y * width] = WriteableBitmapExtensions.ConvertColor(color);
#else
                            WriteBGRA(stream, color);
#endif

                        }
                    }
                });

#if !WINDOWS_PHONE
                stream.Dispose();
#endif
                bmp.Invalidate();
            }

            private static void WriteBGRA(Stream stream, Color color)
            {
                stream.WriteByte(color.B);
                stream.WriteByte(color.G);
                stream.WriteByte(color.R);
                stream.WriteByte(color.A);
            }

            public static Color CalcWheelColor(float x, float y)
            {
                x = x - 0.5f;
                y = y - 0.5f;

                float saturation = 200.0f * (float)Math.Sqrt(x * x + y * y);
                if (saturation > 100.0f) saturation = 100.0f;

                float hue = y < 0 ?
                    (float)(Math.Atan2(-y, -x) / Math.PI) * 180.0f + 180.0f :
                    (float)(Math.Atan2(y, x) / Math.PI) * 180.0f;

                return HSV.HSVtoRGB(255, hue, saturation, 100.0f);
            }
        }


        #endregion


        //@Delegate
        /// <summary> Occurs when the color value changes. </summary>
        public event ColorChangeHandler ColorChange = null;
        /// <summary> Occurs when the hsv value changes. </summary>
        public event HSVChangeHandler HSVChange = null;

        
        /// <summary> Gets picker's type name. </summary>
        public string Type => "Circle";
        /// <summary> Gets picker self. </summary>
        public UserControl Self => this;

        /// <summary> Gets or sets picker's color. </summary>
        public Color Color
        {
            get => HSV.HSVtoRGB(this.HSV);
            set => this.HSV = HSV.RGBtoHSV(value);
        }


        #region Color


        /// <summary> Gets or sets picker's hsv. </summary>
        public HSV HSV
        {
            get => this.hsv;
            set
            {
                byte A = value.A;
                float H = value.H;
                float S = value.S;
                float V = value.V;

                
                this.UpdateStop(H);
                this.UpdateSlider(V);

                this.UpdateColor(HSV.HSVtoRGB(value));
                this.UpdateThumb(H, S);

                this.hsv = value;
            }
        }

        private HSV hsv = new HSV(255, 0, 100, 100);
        private HSV _HSV
        {
            get => this.hsv;
            set
            {
                this.ColorChange?.Invoke(this, HSV.HSVtoRGB(value));//Delegate
                this.HSVChange?.Invoke(this, value);//Delegate

                this.hsv = value;
            }
        }


        #endregion


        #region DependencyProperty


        /// <summary>  Gets or sets a brush that describes the border fill of the control. </summary>
        public SolidColorBrush Stroke
        {
            get { return (SolidColorBrush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        /// <summary> Identifies the <see cref = "CirclePicker.Stroke" /> dependency property. </summary>
        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(nameof(Stroke), typeof(SolidColorBrush), typeof(CirclePicker), new PropertyMetadata(new SolidColorBrush(Windows.UI.Colors.Gray)));


        #endregion


        float _canvasWidth;
        float _canvasHeight;

        //Circle
        Vector2 _center;// Circle's center
        float _radio;// Circle's radio
        float _maxRadio;

        //Manipulation
        Vector2 _position;


        //@Construct
        /// <summary>
        /// Construct a CirclePicker.
        /// </summary>
        public CirclePicker()
        {
            this.InitializeComponent();
            this.Loaded += async (s, e) =>
            {
                float width = (float)this.RootGrid.ActualWidth;
                float height = (float)this.RootGrid.ActualHeight;

                this._canvasWidth = width;
                this._canvasHeight = height;
                this._center = new Vector2(width, height) / 2;

                float radio = Math.Min(width, height) / 2;
                this._radio = radio;

                int size = (int)(radio * 2);

                await this.UpdateImage(size);
                this.UpdateEllipse(size);
                this.UpdateSlider(this.hsv.V);
                this.Change(this.hsv);
            };

            //Image
            this.RootGrid.SizeChanged += async (s, e) =>
            {
                if (e.NewSize == e.PreviousSize) return;
                float width = (float)e.NewSize.Width;
                float height = (float)e.NewSize.Height;

                this._canvasWidth = width;
                this._canvasHeight = height;
                this._center = new Vector2(width, height) / 2;

                float radio = Math.Min(width, height) / 2;
                this._radio = radio;

                int size = (int)(radio * 2);

                if (this._maxRadio + 30 < radio)//30 cache width, for performance optimization.
                {
                    this._maxRadio = radio;
                    await this.UpdateImage(size);
                }

                this.UpdateEllipse(size);
                this.UpdateThumb(this.hsv.H, this.hsv.S);
            };

            //Manipulation
            this.Canvas.ManipulationMode = ManipulationModes.All;
            this.Canvas.ManipulationStarted += (s, e) =>
            {
                this._position = e.Position.ToVector2() - this._center;
            };
            this.Canvas.ManipulationDelta += (s, e) =>
            {
                this._position += e.Delta.Translation.ToVector2();
                this._HSV = this.Change(this._position); 
            };
            this.Canvas.ManipulationCompleted += (s, e) => { };

            //Slider
            this.VSlider.ValueChangeDelta += (sender, value) => this._HSV = this.Change((float)value);

        }


        #region Change


        private HSV Change(Vector2 position)
        {
            float H = CircleSize.VectorToH( position);
            float S = CircleSize.VectorToS( position, this._radio);
            HSV hsv = new HSV(255,H,S,this.hsv.V);

            this.Change(hsv);
            return hsv;
        }
        private HSV Change(float value)
        {
            float V = value;
            HSV hsv = new HSV(255, this.hsv.H, this.hsv.S, V);

            this.Change(hsv);
            return hsv;
        }
        private void Change(HSV hsv)
        {
            this.UpdateStop(hsv.H);

            this.UpdateColor(HSV.HSVtoRGB(hsv));
            this.UpdateThumb(hsv.H, hsv.S);
        }


        #endregion

        #region Update


        private void UpdateStop(float H) => this.VRight.Color = HSV.HSVtoRGB(H);
        private void UpdateSlider(float V) => this.VSlider.Value = this.hsv.V;

        private void UpdateColor(Color color)=>    this.SolidColorBrush.Color = color;
        private void UpdateThumb(float H, float S)
        {
            Vector2 wheel = CircleSize.HSToVector((float)((H + 360.0) * Math.PI / 180.0), S, this._radio, this._center);
            Thumb thumb = this.HSThumb;
            Canvas.SetLeft(thumb, wheel.X - thumb.ActualWidth / 2);
            Canvas.SetTop(thumb, wheel.Y - thumb.ActualHeight / 2);
        }

        private void UpdateEllipse(int size) => this.HSEllipse.Width = this.HSEllipse.Height = size;
        private async Task UpdateImage(int size)
        {
            if (size < 32) return;

            try
            {
                WriteableBitmap bmp = new WriteableBitmap(size, size);
                await HueWheelHelpher.CreateHueCircle(bmp);
                this.ImageBrush.ImageSource = bmp;
            }
            catch (Exception)
            {
            }
        }


        #endregion

    }
}
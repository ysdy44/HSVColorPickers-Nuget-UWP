using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace HSVColorPickers
{
    /// <summary>
    /// Pick a color in the wheel.
    /// </summary>
    public sealed partial class WheelPicker : UserControl, IColorPicker, IHSVPicker
    {

        #region Helpher


        private sealed class WheelSize
        {
            public static float VectorToH(Vector2 vector) => ((((float)Math.Atan2(vector.Y, vector.X)) * 180.0f / (float)Math.PI) + 360.0f) % 360.0f;
            public static float VectorToS(float vectorX, float squareRadio) => vectorX * 50 / squareRadio + 50;
            public static float VectorToV(float vectorY, float squareRadio) => 50 - vectorY * 50 / squareRadio;

            public static Vector2 HToVector(float h, float radio, Vector2 center) => new Vector2((float)Math.Cos(h) * radio + center.X, (float)Math.Sin(h) * radio + center.Y);
            public static float SToVector(float s, float squareRadio, float centerX) => ((float)s - 50) * squareRadio / 50 + centerX;
            public static float VToVector(float v, float squareRadio, float centerY) => (50 - (float)v) * squareRadio / 50 + centerY;
        }


        private sealed class PaletteBrush
        {
            public CanvasLinearGradientBrush Brush;
            public Color Color = Colors.Black;

            private Vector2 startPoint;
            public Vector2 StartPoint
            {
                get => this.startPoint;
                set
                {
                    if (this.Brush != null) this.Brush.StartPoint = value;
                    this.startPoint = value;
                }
            }
            private Vector2 endPoint;
            public Vector2 EndPoint
            {
                get => this.endPoint;
                set
                {
                    if (this.Brush != null) this.Brush.EndPoint = value;
                    this.endPoint = value;
                }
            }


            public CanvasLinearGradientBrush GetBrush(ICanvasResourceCreator creator) => new CanvasLinearGradientBrush(creator, Windows.UI.Colors.Transparent, this.Color)
            {
                StartPoint = this.StartPoint,
                EndPoint = this.EndPoint
            };
            public void SetBrush(ICanvasResourceCreator creator)
            {
                if (this.Brush != null)
                {
                    this.Brush = this.GetBrush(creator);
                }
            }
        }


        #endregion


        //@Delegate
        /// <summary> Occurs when the color value changed. </summary>
        public event ColorChangeHandler ColorChanged;
        /// <summary> Occurs when the color change starts. </summary>
        public event ColorChangeHandler ColorChangeStarted;
        /// <summary> Occurs when color change. </summary>
        public event ColorChangeHandler ColorChangeDelta;
        /// <summary> Occurs when the color change is complete. </summary>
        public event ColorChangeHandler ColorChangeCompleted;
        /// <summary> Occurs when the hsv value changed. </summary>
        public event HSVChangeHandler HSVChanged;
        /// <summary> Occurs when the hsv change starts. </summary>
        public event HSVChangeHandler HSVChangeStarted;
        /// <summary> Occurs when hsv change. </summary>
        public event HSVChangeHandler HSVChangeDelta;
        /// <summary> Occurs when the hsv change is complete. </summary>
        public event HSVChangeHandler HSVChangeCompleted;


        /// <summary> Gets picker's type name. </summary>
        public string Type => "Wheel";
        /// <summary> Gets picker self. </summary>
        public UserControl Self => this;

        /// <summary> Gets or sets picker's color. </summary>
        public Color Color
        {
            get => HSV.HSVtoRGB(this.HSV);
            set => this.HSV = HSV.RGBtoHSV(value);
        }


        #region Color


        private void Invalidate(HSV value)
        {
            this._horizontalBrush.Color = HSV.HSVtoRGB(value.H);
            this._horizontalBrush.SetBrush(this.CanvasControl);
            this.CanvasControl.Invalidate();
        }

        /// <summary> Gets or sets picker's hsv. </summary>
        public HSV HSV
        {
            get => this.hsv;
            set
            {
                this.Invalidate(value);
                this.hsv = value;
            }
        }

        private HSV hsv = new HSV(255, 0, 100, 100);
        private HSV _HSV
        {
            get => this.hsv;
            set
            {
                this.ColorChanged?.Invoke(this, HSV.HSVtoRGB(value));//Delegate
                this.HSVChanged?.Invoke(this, value);//Delegate

                this.Invalidate(value);
                this.hsv = value;
            }
        }
        private HSV _HSVStarted
        {
            get => this.hsv;
            set
            {
                this.ColorChangeStarted?.Invoke(this, HSV.HSVtoRGB(value));//Delegate
                this.HSVChangeStarted?.Invoke(this, value);//Delegate

                this.Invalidate(value);
                this.hsv = value;
            }
        }
        private HSV _HSVDelta
        {
            get => this.hsv;
            set
            {
                this.ColorChangeDelta?.Invoke(this, HSV.HSVtoRGB(value));//Delegate
                this.HSVChangeDelta?.Invoke(this, value);//Delegate

                this.Invalidate(value);
                this.hsv = value;
            }
        }
        private HSV _HSVCompleted
        {
            get => this.hsv;
            set
            {
                this.ColorChangeCompleted?.Invoke(this, HSV.HSVtoRGB(value));//Delegate
                this.HSVChangeCompleted?.Invoke(this, value);//Delegate

                this.Invalidate(value);
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

        /// <summary> Identifies the <see cref = "WheelPicker.Stroke" /> dependency property. </summary>
        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(nameof(Stroke), typeof(SolidColorBrush), typeof(WheelPicker), new PropertyMetadata(new SolidColorBrush(Windows.UI.Colors.Gray)));

        
        #endregion


        readonly float _strokeWidth = 8;
        float _canvasWidth;
        float _canvasHeight;
        
        //Wheel
        Vector2 _center;// Wheel's center
        float _radio;// Wheel's radio
        float _radioSpace; 
         CanvasRenderTarget _wheelCanvas;

        //Palette  
        float _square;
        Rect _rect;
        PaletteBrush _horizontalBrush = new PaletteBrush();
        PaletteBrush _verticalBrush = new PaletteBrush();
        
        //Manipulation
        bool _isWheel;
        bool _isPalette;
        Vector2 _position;


        //@Construct
        /// <summary>
        /// Construct a WheelPicker.
        /// </summary>
        public WheelPicker()
        {
            this.InitializeComponent();

            //Canvas
            this.CanvasControl.SizeChanged += (s, e) =>
            {
                if (e.NewSize == e.PreviousSize) return;

                float width = (float)e.NewSize.Width;
                float height = (float)e.NewSize.Height;

                if (this._wheelCanvas!=null)
                {
                    if (this._canvasWidth != width || this._canvasHeight != height)
                    {
                        //Wheel
                        this._wheelCanvas = new CanvasRenderTarget(this.CanvasControl, width, height);
                    }
                }
                this._canvasWidth = width;
                this._canvasHeight = height;

                //Wheel
                this._center.X = width / 2;
                this._center.Y = height / 2;
                this._radio = Math.Min(width, height) / 2 - this._strokeWidth;
                this._radio = Math.Max(this._radio, 20);
                this._radioSpace = (float)(2 * Math.PI) / (int)(Math.PI * this._radio * 2 / this._strokeWidth);
                if (this._wheelCanvas != null)
                {
                    using (CanvasDrawingSession ds = this._wheelCanvas.CreateDrawingSession())
                    {
                        this.DrawWheelCanvas(ds); 
                    }
                }

                // Palette          
                this._square = (this._radio - this._strokeWidth) / 1.414213562373095f;
                this._square = Math.Max(this._square, 20);
                this._rect = new Rect(this._center.X - this._square, this._center.Y - this._square, this._square * 2, this._square * 2);

                this._horizontalBrush.StartPoint = new Vector2(this._center.X - this._square, this._center.Y);//Left
                this._horizontalBrush.EndPoint = new Vector2(this._center.X + this._square, this._center.Y);//Right
                this._horizontalBrush.SetBrush(this.CanvasControl);

                this._verticalBrush.StartPoint = new Vector2(this._center.X, this._center.Y - this._square);//Top
                this._verticalBrush.EndPoint = new Vector2(this._center.X, this._center.Y + this._square);//Bottom
            };
            this.CanvasControl.CreateResources += (sender, args) =>
            {
                //Wheel
                this._wheelCanvas = new CanvasRenderTarget(sender, this._canvasWidth, this._canvasHeight);
                using (CanvasDrawingSession ds = this._wheelCanvas.CreateDrawingSession())
                {
                    this.DrawWheelCanvas(ds);
                }

                // Palette       
                this._horizontalBrush.Color = HSV.HSVtoRGB(this.hsv.H);
                this._horizontalBrush.Brush = this._horizontalBrush.GetBrush(sender);
                this._verticalBrush.Brush = this._verticalBrush.GetBrush(sender);
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                //Wheel           
                args.DrawingSession.DrawImage(this._wheelCanvas);

                //Thumb
                Vector2 wheel = WheelSize.HToVector((float)((this.HSV.H + 360.0) * Math.PI / 180.0), this._radio, this._center);
                this.DrawThumb(args.DrawingSession, wheel);
                
                //Palette
                args.DrawingSession.FillRoundedRectangle(this._rect, 4, 4, Colors.White);
                args.DrawingSession.FillRoundedRectangle(this._rect, 4, 4, this._horizontalBrush.Brush);
                args.DrawingSession.FillRoundedRectangle(this._rect, 4, 4, this._verticalBrush.Brush);
                args.DrawingSession.DrawRoundedRectangle(this._rect, 4, 4, this.Stroke.Color);

                //Thumb 
                float paletteX = WheelSize.SToVector(this.HSV.S, this._square, this._center.X);
                float paletteY = WheelSize.VToVector(this.HSV.V, this._square, this._center.Y);
                this.DrawThumb(args.DrawingSession, new Vector2(paletteX, paletteY));
            };


            //Manipulation
            this.CanvasControl.ManipulationMode = ManipulationModes.All;
            this.CanvasControl.ManipulationStarted += (s, e) =>
            {
                this._position = e.Position.ToVector2() - this._center;

                this._isWheel = this._position.Length() + this._strokeWidth > this._radio && this._position.Length() - this._strokeWidth < this._radio;
                this._isPalette = Math.Abs(this._position.X) < this._square && Math.Abs(this._position.Y) < this._square;

                if (this._isWheel) this._HSVStarted = new HSV(this.hsv.A, WheelSize.VectorToH(this._position), this.hsv.S, this.hsv.V);
                if (this._isPalette) this._HSVStarted = new HSV(this.hsv.A, this.hsv.H, WheelSize.VectorToS(this._position.X, this._square), WheelSize.VectorToV(this._position.Y, this._square));
            };
            this.CanvasControl.ManipulationDelta += (s, e) =>
            {
                this._position += e.Delta.Translation.ToVector2();

                if (this._isWheel) this._HSVDelta = new HSV(this.hsv.A, WheelSize.VectorToH(this._position), this.hsv.S, this.hsv.V);
                if (this._isPalette) this._HSVDelta = new HSV(this.hsv.A, this.hsv.H, WheelSize.VectorToS(this._position.X, this._square), WheelSize.VectorToV(this._position.Y, this._square));
            };
            this.CanvasControl.ManipulationCompleted += (s, e) =>
            {
                if (this._isWheel) this._HSVCompleted = new HSV(this.hsv.A, WheelSize.VectorToH(this._position), this.hsv.S, this.hsv.V);
                if (this._isPalette) this._HSVCompleted = new HSV(this.hsv.A, this.hsv.H, WheelSize.VectorToS(this._position.X, this._square), WheelSize.VectorToV(this._position.Y, this._square));

                this._isWheel = false;
                this._isPalette = false;
            };
        }

        //Wheel
        private void DrawWheelCanvas(CanvasDrawingSession ds)
        {
            ds.Clear(Windows.UI.Colors.Transparent);
            ds.DrawCircle(this._center, this._radio, this.Stroke.Color, this._strokeWidth * 2);

            for (float angle = 0; angle < 6.2831853071795862f; angle += this._radioSpace)
            {
                Vector2 vector = WheelSize.HToVector(angle, this._radio, this._center);
                Color color = HSV.HSVtoRGB(angle * 180.0f / (float)Math.PI);
                ds.FillCircle(vector, this._strokeWidth, color);
            }

            ds.DrawCircle(this._center, this._radio - this._strokeWidth, this.Stroke.Color);
            ds.DrawCircle(this._center, this._radio + this._strokeWidth, this.Stroke.Color);
        }


        //Thumb
        private void DrawThumb(CanvasDrawingSession ds, Vector2 vector)
        {
            ds.DrawCircle(vector, 9, Windows.UI.Colors.Black, 5);
            ds.DrawCircle(vector, 9, Windows.UI.Colors.White, 3);
        }

    }
}
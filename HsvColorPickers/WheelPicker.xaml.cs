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
    /// WheelPicker:
    ///    Color wheel picker.
    /// </summary>
    public sealed partial class WheelPicker : UserControl, IPicker
    {
        /// <summary> Size. </summary>
        private class WheelSize
        {
            public static float VectorToH(Vector2 vector) => ((((float)Math.Atan2(vector.Y, vector.X)) * 180.0f / (float)Math.PI) + 360.0f) % 360.0f;
            public static float VectorToS(float vectorX, float squareRadio) => vectorX * 50 / squareRadio + 50;
            public static float VectorToV(float vectorY, float squareRadio) => 50 - vectorY * 50 / squareRadio;

            public static Vector2 HToVector(float h, float radio, Vector2 center) => new Vector2((float)Math.Cos(h) * radio + center.X, (float)Math.Sin(h) * radio + center.Y);
            public static float SToVector(float s, float squareRadio, float centerX) => ((float)s - 50) * squareRadio / 50 + centerX;
            public static float VToVector(float l, float squareRadio, float centerY) => (50 - (float)l) * squareRadio / 50 + centerY;
        }

        /// <summary> Brush. </summary>
        private class PaletteBrush
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


        //Delegate
        public event ColorChangeHandler ColorChange = null;
        public event HSVChangeHandler HSVChange = null;
                
        public Color GetColor() => HSV.HSVtoRGB(this.HSV);
        public void SetColor(Color value) => this.HSV = HSV.RGBtoHSV(value);

        readonly float StrokeWidth = 8;
        float CanvasWidth;
        float CanvasHeight;
        
        //Wheel
        Vector2 Center;// Wheel's center
        float Radio;// Wheel's radio
        float RadioSpace;
        CanvasRenderTarget WheelCanvas;

        //Palette  
        float Square;
        Rect Rect;
        PaletteBrush HorizontalBrush = new PaletteBrush();
        PaletteBrush VerticalBrush = new PaletteBrush();
        
        //Manipulation
        bool IsWheel;
        bool IsPalette;
        Vector2 Position;
        
        #region DependencyProperty


        private HSV hsv = new HSV(255, 0, 1, 1);
        private HSV _HSV
        {
            get => this.hsv;
            set
            {
                //Palette  
                this.HorizontalBrush.Color = HSV.HSVtoRGB(value.H);
                this.HorizontalBrush.SetBrush(this.CanvasControl);
                             
                this.ColorChange?.Invoke(this, HSV.HSVtoRGB(value));//Delegate
                this.HSVChange?.Invoke(this, value);//Delegate

                this.CanvasControl.Invalidate();

                this.hsv = value;
            }
        }
        public HSV HSV
        {
            get => this.hsv;
            set
            {
                //Palette                
                this.HorizontalBrush.Color = HSV.HSVtoRGB(value.H);
                this.HorizontalBrush.SetBrush(this.CanvasControl);

                this.CanvasControl.Invalidate();
                this.hsv = value;
            }
        }

        public Color Color
        {
            get => this.GetColor();
            set => this.SetColor(value);
        }



        public SolidColorBrush Stroke
        {
            get { return (SolidColorBrush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }
        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(nameof(Stroke), typeof(SolidColorBrush), typeof(WheelPicker), new PropertyMetadata(new SolidColorBrush(Windows.UI.Colors.Gray)));


        #endregion


        public WheelPicker()
        {
            this.InitializeComponent();


            //Canvas
            this.CanvasControl.SizeChanged += (s, e) =>
            {
                if (e.NewSize == e.PreviousSize) return;

                float width = (float)e.NewSize.Width;
                float height = (float)e.NewSize.Height;

                if (this.WheelCanvas!=null)
                {
                    if (this.CanvasWidth != width || this.CanvasHeight != height)
                    {
                        //Wheel
                        this.WheelCanvas = new CanvasRenderTarget(this.CanvasControl, width, height);
                    }
                }
                this.CanvasWidth = width;
                this.CanvasHeight = height;

                //Wheel
                this.Center.X = width / 2;
                this.Center.Y = height / 2;
                this.Radio = Math.Min(width, height) / 2 - this.StrokeWidth;
                this.Radio = Math.Max(this.Radio, 20);
                this.RadioSpace = (float)(2 * Math.PI) / (int)(Math.PI * this.Radio * 2 / this.StrokeWidth);
                if (this.WheelCanvas != null)
                {
                    using (CanvasDrawingSession ds = this.WheelCanvas.CreateDrawingSession())
                    {
                        this.DrawWheelCanvas(ds); 
                    }
                }

                // Palette          
                this.Square = (this.Radio - this.StrokeWidth) / 1.414213562373095f;
                this.Square = Math.Max(this.Square, 20);
                this.Rect = new Rect(this.Center.X - this.Square, this.Center.Y - this.Square, this.Square * 2, this.Square * 2);

                this.HorizontalBrush.StartPoint = new Vector2(this.Center.X - this.Square, this.Center.Y);//Left
                this.HorizontalBrush.EndPoint = new Vector2(this.Center.X + this.Square, this.Center.Y);//Right
                this.HorizontalBrush.SetBrush(this.CanvasControl);

                this.VerticalBrush.StartPoint = new Vector2(this.Center.X, this.Center.Y - this.Square);//Top
                this.VerticalBrush.EndPoint = new Vector2(this.Center.X, this.Center.Y + this.Square);//Bottom
            };
            this.CanvasControl.CreateResources += (sender, args) =>
            {
                //Wheel
                this.WheelCanvas = new CanvasRenderTarget(sender, this.CanvasWidth, this.CanvasHeight);
                using (CanvasDrawingSession ds = this.WheelCanvas.CreateDrawingSession())
                {
                    this.DrawWheelCanvas(ds);
                }

                // Palette       
                this.HorizontalBrush.Color = HSV.HSVtoRGB(this.hsv.H);
                this.HorizontalBrush.Brush = this.HorizontalBrush.GetBrush(sender);
                this.VerticalBrush.Brush = this.VerticalBrush.GetBrush(sender);
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                //Wheel           
                args.DrawingSession.DrawImage(this.WheelCanvas);

                //Thumb
                Vector2 wheel = WheelSize.HToVector((float)((this.HSV.H + 360.0) * Math.PI / 180.0), this.Radio, this.Center);
                this.DrawThumb(args.DrawingSession, wheel);
                
                //Palette
                args.DrawingSession.FillRoundedRectangle(this.Rect, 4, 4, Colors.White);
                args.DrawingSession.FillRoundedRectangle(this.Rect, 4, 4, this.HorizontalBrush.Brush);
                args.DrawingSession.FillRoundedRectangle(this.Rect, 4, 4, this.VerticalBrush.Brush);
                args.DrawingSession.DrawRoundedRectangle(this.Rect, 4, 4, this.Stroke.Color);

                //Thumb 
                float paletteX = WheelSize.SToVector(this.HSV.S, this.Square, this.Center.X);
                float paletteY = WheelSize.VToVector(this.HSV.V, this.Square, this.Center.Y);
                this.DrawThumb(args.DrawingSession, new Vector2(paletteX, paletteY));
            };


            //Manipulation
            this.CanvasControl.ManipulationMode = ManipulationModes.All;
            this.CanvasControl.ManipulationStarted += (s, e) =>
            {
                this.Position = e.Position.ToVector2() - this.Center;

                this.IsWheel = this.Position.Length() + this.StrokeWidth > this.Radio && this.Position.Length() - this.StrokeWidth < this.Radio;
                this.IsPalette = Math.Abs(this.Position.X) < this.Square && Math.Abs(this.Position.Y) < this.Square;

                if (this.IsWheel) this._HSV = new HSV(hsv.A, WheelSize.VectorToH(this.Position), hsv.S, hsv.V);
                if (this.IsPalette) this._HSV = new HSV(hsv.A, hsv.H, WheelSize.VectorToS(this.Position.X, this.Square), WheelSize.VectorToV(this.Position.Y, this.Square));
            };
            this.CanvasControl.ManipulationDelta += (s, e) =>
            {
                this.Position += e.Delta.Translation.ToVector2();

                if (this.IsWheel) this._HSV = new HSV(hsv.A, WheelSize.VectorToH(this.Position), hsv.S, hsv.V);
                if (this.IsPalette) this._HSV = new HSV(hsv.A, hsv.H, WheelSize.VectorToS(this.Position.X, this.Square), WheelSize.VectorToV(this.Position.Y, this.Square));
            };
            this.CanvasControl.ManipulationCompleted += (s, e) =>
            {
                this.IsWheel = false;
                this.IsPalette = false;
            };
        }

        //Wheel
        private void DrawWheelCanvas(CanvasDrawingSession ds)
        {
            ds.Clear(Windows.UI.Colors.Transparent);
            ds.DrawCircle(this.Center, this.Radio, this.Stroke.Color, this.StrokeWidth * 2);

            for (float angle = 0; angle < 6.2831853071795862f; angle += this.RadioSpace)
            {
                Vector2 vector = WheelSize.HToVector(angle, this.Radio, this.Center);
                Color color = HSV.HSVtoRGB(angle * 180.0f / (float)Math.PI);
                ds.FillCircle(vector, this.StrokeWidth, color);
            }

            ds.DrawCircle(this.Center, this.Radio - this.StrokeWidth, this.Stroke.Color);
            ds.DrawCircle(this.Center, this.Radio + this.StrokeWidth, this.Stroke.Color);
        }


        //Thumb
        private void DrawThumb(CanvasDrawingSession ds, Vector2 vector)
        {
            ds.DrawCircle(vector, 9, Windows.UI.Colors.Black, 5);
            ds.DrawCircle(vector, 9, Windows.UI.Colors.White, 3);
        }

    }
}

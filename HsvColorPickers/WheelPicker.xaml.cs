using Microsoft.Graphics.Canvas.Brushes;
using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace HSVColorPickers
{
    /// <summary> Size. </summary>
    public class WheelSize
    {
        public static double VectorToH(Vector2 vector) => (((Math.Atan2(vector.Y, vector.X)) * 180.0 / Math.PI) + 360.0) % 360.0;
        public static float VectorToS(float vectorX, float squareRadio) => vectorX * 50 / squareRadio + 50;
        public static float VectorToL(float vectorY, float squareRadio) => 50 - vectorY * 50 / squareRadio;

        public static Vector2 HToVector(float h, float radio, Vector2 center) => new Vector2((float)Math.Cos(h) * radio + center.X, (float)Math.Sin(h) * radio + center.Y);
        public static float SToVector(double s, float squareRadio, float centerX) => ((float)s - 50) * squareRadio / 50 + centerX;
        public static float LToVector(double l, float squareRadio, float centerY) => (50 - (float)l) * squareRadio / 50 + centerY;
    }

    public sealed partial class WheelPicker : UserControl, IPicker
    {
        //Delegate
        public event ColorChangeHandler ColorChange = null;
        public Color GetColor() => HSV.HSVtoRGB(this.HSV);
        public void SetColor(Color value) => this.HSV = HSV.RGBtoHSV(value);

        readonly float StrokeWidth = 8;

        //Wheel
        Vector2 Center;// Wheel's center
        float Radio;// Wheel's radio
        float RadioSpace;

        //Palette  
        float Square;
        Rect Rect;
        CanvasLinearGradientBrush HorizontalBrush;
        CanvasLinearGradientBrush VerticalBrush;

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
                this.HorizontalBrush.Stops[1].Color = HSV.HSVtoRGB(value.H);

                this.CanvasControl.Invalidate();
                this.ColorChange?.Invoke(this, HSV.HSVtoRGB(value));//Delegate

                this.hsv = value;
            }
        }
        public HSV HSV
        {
            get => this.hsv;
            set
            {
                //Palette  
                this.HorizontalBrush.Stops[1].Color = HSV.HSVtoRGB(value.H);

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
            this.HorizontalBrush = new CanvasLinearGradientBrush(this.CanvasControl, Windows.UI.Colors.White, HSV.HSVtoRGB(this.HSV.H));
            this.VerticalBrush = new CanvasLinearGradientBrush(this.CanvasControl, Windows.UI.Colors.Transparent, Windows.UI.Colors.Black);

            //height
            this.CanvasControl.SizeChanged += (s, e) =>
            {
                if (e.NewSize == e.PreviousSize) return;
                float width = (float)e.NewSize.Width;
                float height = (float)e.NewSize.Height;

                //Wheel
                this.Center.X = width;
                this.Center.Y = height;
                this.Radio = Math.Min(width, height) / 2 - this.StrokeWidth;
                this.RadioSpace = (float)(2 * Math.PI) / (int)(Math.PI * this.Radio * 2 / this.StrokeWidth);

                // Palette          
                this.Square=(this.Radio - this.StrokeWidth) / 1.414213562373095f;
                this.Rect = new Rect(this.Center.X - this.Square, this.Center.Y - this.Square, this.Square * 2, this.Square * 2);
                this.HorizontalBrush.StartPoint = new Vector2(this.Center.X - this.Square, this.Center.Y);
                this.HorizontalBrush.EndPoint = new Vector2(this.Center.X + this.Square, this.Center.Y);
                this.VerticalBrush.StartPoint = new Vector2(this.Center.X, this.Center.Y - this.Square);
                this.VerticalBrush.EndPoint = new Vector2(this.Center.X, this.Center.Y + this.Square);
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                //Wheel           
                args.DrawingSession.DrawCircle(this.Center, this.Radio, this.Stroke.Color, this.StrokeWidth * 2);
                for (float angle = 0; angle < 6.2831853071795862f; angle += this.RadioSpace)
                {
                    Vector2 vector = WheelSize.HToVector(angle, this.Radio, this.Center);
                    Color color = HSV.HSVtoRGB(angle * 180.0 / Math.PI);
                    args.DrawingSession.FillCircle(vector, this.StrokeWidth, color);
                }
                args.DrawingSession.DrawCircle(this.Center, this.Radio - this.StrokeWidth, this.Stroke.Color);
                args.DrawingSession.DrawCircle(this.Center, this.Radio + this.StrokeWidth, this.Stroke.Color);
                
                //Thumb
                Vector2 wheel = WheelSize.HToVector((float)((this.HSV.H + 360.0) * Math.PI / 180.0), this.Radio, this.Center);
                args.DrawingSession.DrawCircle(wheel, 9, Windows.UI.Colors.Black, 5);
                args.DrawingSession.DrawCircle(wheel, 9, Windows.UI.Colors.White, 3);
                
                //Palette
                args.DrawingSession.FillRoundedRectangle(this.Rect, 4, 4, this.HorizontalBrush);
                args.DrawingSession.FillRoundedRectangle(this.Rect, 4, 4, this.VerticalBrush);
                args.DrawingSession.DrawRoundedRectangle(this.Rect, 4, 4, this.Stroke.Color);
                
                //Thumb 
                float paletteX = WheelSize.SToVector(this.HSV.S, this.Square, this.Center.X);
                float paletteY = WheelSize.LToVector(this.HSV.V, this.Square, this.Center.Y);
                args.DrawingSession.DrawCircle(paletteX, paletteY, 9, Windows.UI.Colors.Black, 5);
                args.DrawingSession.DrawCircle(paletteX, paletteY, 9, Windows.UI.Colors.White, 3);
            };


            //Manipulation
            this.CanvasControl.ManipulationStarted += (s, e) =>
            {
                this.Position = e.Position.ToVector2() - this.Center;

                this.IsWheel = this.Position.Length() + this.StrokeWidth > this.Radio && this.Position.Length() - this.StrokeWidth < this.Radio;
                this.IsPalette = Math.Abs(this.Position.X) < this.Square && Math.Abs(this.Position.Y) < this.Square;

                if (this.IsWheel)  this._HSV = new HSV(hsv.A, WheelSize.VectorToH(this.Position), hsv.S, hsv.V);
                if (this.IsPalette) this._HSV = new HSV(hsv.A, hsv.H, WheelSize.VectorToS(this.Position.X, this.Square), WheelSize.VectorToL(this.Position.Y, this.Square));
            };
            this.CanvasControl.ManipulationDelta += (s, e) =>
            {
                this.Position += e.Delta.Translation.ToVector2();

                if (this.IsWheel) this._HSV = new HSV(hsv.A, WheelSize.VectorToH(this.Position), hsv.S, hsv.V);
                if (this.IsPalette) this._HSV = new HSV(hsv.A, hsv.H, WheelSize.VectorToS(this.Position.X, this.Square), WheelSize.VectorToL(this.Position.Y, this.Square));
            };
            this.CanvasControl.ManipulationCompleted += (s, e) =>
            {
                this.IsWheel = false;
                this.IsPalette = false;
            };
        } 
    }
}

using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Numerics;
using Windows.Foundation;
using Windows.UI.Xaml.Media;

namespace HSVColorPickers
{
    /// <summary>
    /// Represents a PalettePicker for Saturation.
    /// </summary>
    public class PaletteSaturationPicker : PalettePicker
    {

        private readonly CanvasGradientStop[] BackgroundStops = new CanvasGradientStop[]
        {
            new CanvasGradientStop { Position = 0.0f, Color =  Windows.UI.Colors.Red },
            new CanvasGradientStop { Position = 0.16666667f, Color =Windows.UI.Colors.Yellow},
            new CanvasGradientStop { Position = 0.33333333f, Color = Windows.UI.Color.FromArgb(255,0,255,0) },
            new CanvasGradientStop { Position = 0.5f, Color = Windows.UI.Colors.Cyan },
            new CanvasGradientStop { Position = 0.66666667f, Color = Windows.UI.Colors.Blue},
            new CanvasGradientStop { Position = 0.83333333f, Color =  Windows.UI.Colors.Magenta },
            new CanvasGradientStop { Position = 1.0f, Color =  Windows.UI.Colors.Red },
        };
        private readonly CanvasGradientStop[] ForegroundStops = new CanvasGradientStop[]
        {
            new CanvasGradientStop { Position = 0.0f, Color = Windows.UI.Colors.Transparent },
            new CanvasGradientStop { Position = 1.0f, Color = Windows.UI.Colors.Black }
        };


        /// <summary> Gets picker's type name. </summary>
        public override string Type { get; set; } = "PaletteSaturation";
        /// <summary> Unit </summary>
        public override string Unit { get; set; } = "%";
        /// <summary> Minimum </summary>
        public override double Minimum { get; set; } = 0;
        /// <summary> Maximum </summary>
        public override double Maximum { get; set; } = 100;


        /// <summary> Override <see cref="PalettePicker.GetHSV"/>. </summary>
        public override HSV GetHSV(HSV hsv, float value) => new HSV(hsv.A, hsv.H, value, hsv.V);
        /// <summary> Override <see cref="PalettePicker.GetValue"/>. </summary>
        public override float GetValue(HSV hsv) => hsv.S;

        /// <summary> Override <see cref="PalettePicker.GetSliderBrush"/>. </summary>
        public override GradientStopCollection GetSliderBrush(HSV hsv)
        {
            byte A = HSV.A;
            float H = HSV.H;
            float S = HSV.S;
            float V = HSV.V;

            return new GradientStopCollection()
            {
                new GradientStop()
                {
                    Offset = 0,
                    Color = HSV.HSVtoRGB(A, H, 0.0f, V)
                },
               new GradientStop()
                {
                    Offset = 1,
                    Color =HSV.HSVtoRGB(A, H, 100.0f, V)
                },
            };
        }

        /// <summary> Override <see cref="PalettePicker.Draw"/>. </summary>
        public override void Draw(CanvasControl sender, CanvasDrawingSession ds, HSV hsv, Vector2 center, float squareHalfWidth, float squareHalfHeight, SolidColorBrush stroke)
        {
            //Palette
            Rect rect = new Rect(center.X - squareHalfWidth, center.Y - squareHalfHeight, squareHalfWidth * 2, squareHalfHeight * 2);
            using (CanvasLinearGradientBrush rainbow = new CanvasLinearGradientBrush(sender, this.BackgroundStops))
            {
                rainbow.StartPoint = new Vector2(center.X - squareHalfWidth, center.Y);
                rainbow.EndPoint = new Vector2(center.X + squareHalfWidth, center.Y);
                ds.FillRoundedRectangle(rect, 4, 4, rainbow);
            }
            using (CanvasLinearGradientBrush brush = new CanvasLinearGradientBrush(sender, this.ForegroundStops))
            {
                brush.StartPoint = new Vector2(center.X, center.Y - squareHalfHeight);
                brush.EndPoint = new Vector2(center.X, center.Y + squareHalfHeight);
                ds.FillRoundedRectangle(rect, 4, 4, brush);
            }
            ds.DrawRoundedRectangle(rect, 4, 4, stroke.Color);

            //Thumb 
            float px = ((float)hsv.H - 180) * squareHalfWidth / 180 + center.X;
            float py = ((float)(50 - hsv.V)) * squareHalfHeight / 50 + center.Y;
            ds.FillCircle(px, py, 12, stroke.Color);
            ds.FillCircle(px, py, 11, HSV.HSVtoRGB(hsv));
        }
        /// <summary> Override <see cref="PalettePicker.Delta"/>. </summary>
        public override HSV Delta(HSV hsv, Vector2 position, float squareHalfWidth, float squareHalfHeight)
        {
            float H = position.X * 180 / squareHalfWidth + 180;
            float V = 50 - position.Y * 50 / squareHalfHeight;

            return new HSV(hsv.A, H, hsv.S, V);
        }
    }
}
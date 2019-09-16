using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace HSVColorPickers
{
    /// <summary> 
    /// Palette Value. 
    /// </summary>
    public class PaletteValue : PaletteBase
    {
        private readonly CanvasGradientStop[] BackgroundStops = new CanvasGradientStop[]
        {
            new CanvasGradientStop { Position = 0.0f, Color =  Windows.UI.Colors.Red },
            new CanvasGradientStop { Position = 0.16666667f, Color =Windows.UI.Colors.Yellow},
            new CanvasGradientStop { Position = 0.33333333f, Color = Color.FromArgb(255,0,255,0) },
            new CanvasGradientStop { Position = 0.5f, Color = Windows.UI.Colors.Cyan },
            new CanvasGradientStop { Position = 0.66666667f, Color = Windows.UI.Colors.Blue},
            new CanvasGradientStop { Position = 0.83333333f, Color =  Windows.UI.Colors.Magenta },
            new CanvasGradientStop { Position = 1.0f, Color =  Windows.UI.Colors.Red },
        };
        private readonly CanvasGradientStop[] ForegroundStops = new CanvasGradientStop[]
        {
            new CanvasGradientStop { Position = 0.0f, Color = Color.FromArgb(0,128,128,128) },
            new CanvasGradientStop { Position = 1.0f, Color = Windows.UI.Colors.White }
        };

        //@Construct
        /// <summary>
        /// Construct a PaletteValue.
        /// </summary>
        public PaletteValue()
        {
            this.Type = "Palette Value";
            this.Unit = "%";
            this.Minimum = 0;
            this.Maximum = 100;
        }

        /// <summary> Override <see cref="PaletteBase.GetHSV"/>. </summary>
        public override HSV GetHSV(HSV hsv, float value) => new HSV(hsv.A, hsv.H, hsv.S, value);
        /// <summary> Override <see cref="PaletteBase.GetValue"/>. </summary>
        public override float GetValue(HSV hsv) => hsv.V;

        /// <summary> Override <see cref="PaletteBase.GetSliderBrush"/>. </summary>
        public override GradientStopCollection GetSliderBrush(HSV HSV)
        {
            byte A = HSV.A;
            float H = HSV.H;
            float S = HSV.S;
            float V = HSV.V;

            return new GradientStopCollection()
            {
                new GradientStop()
                {
                    Offset = 0.0f ,
                    Color = HSV.HSVtoRGB(A, H, S, 0)
                },
                new GradientStop()
                {
                    Offset = 1.0f,
                    Color = HSV.HSVtoRGB(A, H, S, 100)
                },
           };
        }

        /// <summary> Override <see cref="PaletteBase.Draw"/>. </summary>
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
            float py = (50 - (float)hsv.S) * squareHalfHeight / 50 + center.Y;
            ds.DrawCircle(px, py, 9, Windows.UI.Colors.Black, 5);
            ds.DrawCircle(px, py, 9, Windows.UI.Colors.White, 3);
        }
        /// <summary> Override <see cref="PaletteBase.Delta"/>. </summary>
        public override HSV Delta(HSV hsv, Vector2 position, float squareHalfWidth, float squareHalfHeight)
        {
            float H = position.X * 180 / squareHalfWidth + 180;
            float S = 50 - position.Y * 50 / squareHalfHeight;
            return new HSV(hsv.A, H, S, hsv.V);
        }
    }
}

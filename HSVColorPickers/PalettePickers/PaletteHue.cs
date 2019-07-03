using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Numerics;
using Windows.Foundation;
using Windows.UI.Xaml.Media;

namespace HSVColorPickers
{
    /// <summary>
    /// Palette Hue.
    /// </summary>
    public class PaletteHue : PaletteBase
    {
        //@Construct
        public PaletteHue()
        {
            this.Name = "Hue";
            this.Unit = "º";
            this.Minimum = 0;
            this.Maximum = 360;
        }

        public override HSV GetHSL(HSV HSV, float value) => new HSV(HSV.A, value, HSV.S, HSV.V);
        public override float GetValue(HSV HSV) => HSV.H;

        public override GradientStopCollection GetSliderBrush(HSV HSV)
        {
            byte A = HSV.A;
            float H = HSV.H;
            float S = HSV.S;
            float L = HSV.V;

            return new GradientStopCollection()
            {
                new GradientStop()
                {
                    Offset = 0,
                    Color = HSV.HSVtoRGB(A, 0, S, L)
                },
                new GradientStop()
                {
                    Offset = 0.16666667,
                    Color = HSV.HSVtoRGB(A, 60, S, L)
                },
                 new GradientStop()
                {
                    Offset = 0.33333333 ,
                    Color = HSV.HSVtoRGB(A, 120, S, L)
                },
                new GradientStop()
                {
                    Offset = 0.5 ,
                    Color = HSV.HSVtoRGB(A, 180, S, L)
                },
                new GradientStop()
                {
                    Offset = 0.66666667 ,
                    Color = HSV.HSVtoRGB(A, 240, S, L)
                },
                new GradientStop()
                {
                    Offset = 0.83333333 ,
                    Color = HSV.HSVtoRGB(A, 300, S, L)
                },
                new GradientStop()
                {
                    Offset = 1 ,
                    Color = HSV.HSVtoRGB(A, 0, S, L)
                },
            };
        }

        public override void Draw(CanvasControl CanvasControl, CanvasDrawingSession ds, HSV HSV, Vector2 Center, float SquareHalfWidth, float SquareHalfHeight)
        {
            //Palette
            Rect rect = new Rect(Center.X - SquareHalfWidth, Center.Y - SquareHalfHeight, SquareHalfWidth * 2, SquareHalfHeight * 2);
            ds.FillRoundedRectangle(rect, 4, 4, new CanvasLinearGradientBrush(CanvasControl, Windows.UI.Colors.White, HSV.HSVtoRGB(HSV.H)) { StartPoint = new Vector2(Center.X - SquareHalfWidth, Center.Y), EndPoint = new Vector2(Center.X + SquareHalfWidth, Center.Y) });
            ds.FillRoundedRectangle(rect, 4, 4, new CanvasLinearGradientBrush(CanvasControl, Windows.UI.Colors.Transparent, Windows.UI.Colors.Black) { StartPoint = new Vector2(Center.X, Center.Y - SquareHalfHeight), EndPoint = new Vector2(Center.X, Center.Y + SquareHalfHeight) });
            ds.DrawRoundedRectangle(rect, 4, 4, Windows.UI.Colors.Gray);

            //Thumb 
            float px = ((float)HSV.S - 50) * SquareHalfWidth / 50 + Center.X;
            float py = (50 - (float)HSV.V) * SquareHalfHeight / 50 + Center.Y;
            ds.DrawCircle(px, py, 9, Windows.UI.Colors.Black, 5);
            ds.DrawCircle(px, py, 9, Windows.UI.Colors.White, 3);
        }
        public override HSV Delta(HSV HSV, Vector2 v, float SquareHalfWidth, float SquareHalfHeight)
        {
            float S = 50 + v.X * 50 / SquareHalfWidth;
            float L = 50 - v.Y * 50 / SquareHalfHeight;

            return new HSV(HSV.A, HSV.H, S, L);
        }
    }
}

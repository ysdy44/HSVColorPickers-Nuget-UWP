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
        /// <summary>
        /// Construct a PaletteHue.
        /// </summary>
        public PaletteHue()
        {
            this.Type = "Palette Hue";
            this.Unit = "º";
            this.Minimum = 0;
            this.Maximum = 360;
        }

        /// <summary> Override <see cref="PaletteBase.GetHSV"/>. </summary>
        public override HSV GetHSV(HSV hsv, float value) => new HSV(hsv.A, value, hsv.S, hsv.V);
        /// <summary> Override <see cref="PaletteBase.GetValue"/>. </summary>
        public override float GetValue(HSV HSV) => HSV.H;

        /// <summary> Override <see cref="PaletteBase.GetSliderBrush"/>. </summary>
        public override GradientStopCollection GetSliderBrush(HSV hsv)
        {
            byte A = hsv.A;
            float H = hsv.H;
            float S = hsv.S;
            float V = hsv.V;

            return new GradientStopCollection()
            {
                new GradientStop()
                {
                    Offset = 0,
                    Color = HSV.HSVtoRGB(A, 0, S, V)
                },
                new GradientStop()
                {
                    Offset = 0.16666667,
                    Color = HSV.HSVtoRGB(A, 60, S, V)
                },
                 new GradientStop()
                {
                    Offset = 0.33333333 ,
                    Color = HSV.HSVtoRGB(A, 120, S, V)
                },
                new GradientStop()
                {
                    Offset = 0.5 ,
                    Color = HSV.HSVtoRGB(A, 180, S, V)
                },
                new GradientStop()
                {
                    Offset = 0.66666667 ,
                    Color = HSV.HSVtoRGB(A, 240, S, V)
                },
                new GradientStop()
                {
                    Offset = 0.83333333 ,
                    Color = HSV.HSVtoRGB(A, 300, S, V)
                },
                new GradientStop()
                {
                    Offset = 1 ,
                    Color = HSV.HSVtoRGB(A, 0, S, V)
                },
            };
        }

        /// <summary> Override <see cref="PaletteBase.Draw"/>. </summary>
        public override void Draw(CanvasControl sender, CanvasDrawingSession ds, HSV hsv, Vector2 Center, float squareHalfWidth, float squareHalfHeight, SolidColorBrush stroke)
        {
            //Palette
            Rect rect = new Rect(Center.X - squareHalfWidth, Center.Y - squareHalfHeight, squareHalfWidth * 2, squareHalfHeight * 2);
            ds.FillRoundedRectangle(rect, 4, 4, new CanvasLinearGradientBrush(sender, Windows.UI.Colors.White, HSV.HSVtoRGB(hsv.H)) { StartPoint = new Vector2(Center.X - squareHalfWidth, Center.Y), EndPoint = new Vector2(Center.X + squareHalfWidth, Center.Y) });
            ds.FillRoundedRectangle(rect, 4, 4, new CanvasLinearGradientBrush(sender, Windows.UI.Colors.Transparent, Windows.UI.Colors.Black) { StartPoint = new Vector2(Center.X, Center.Y - squareHalfHeight), EndPoint = new Vector2(Center.X, Center.Y + squareHalfHeight) });
            ds.DrawRoundedRectangle(rect, 4, 4, stroke.Color);

            //Thumb 
            float px = ((float)hsv.S - 50) * squareHalfWidth / 50 + Center.X;
            float py = (50 - (float)hsv.V) * squareHalfHeight / 50 + Center.Y;
            ds.DrawCircle(px, py, 9, Windows.UI.Colors.Black, 5);
            ds.DrawCircle(px, py, 9, Windows.UI.Colors.White, 3);
        }
        /// <summary> Override <see cref="PaletteBase.Delta"/>. </summary>
        public override HSV Delta(HSV hsv, Vector2 position, float squareHalfWidth, float squareHalfHeight)
        {
            float S = 50 + position.X * 50 / squareHalfWidth;
            float V = 50 - position.Y * 50 / squareHalfHeight;

            return new HSV(hsv.A, hsv.H, S, V);
        }
    }
}

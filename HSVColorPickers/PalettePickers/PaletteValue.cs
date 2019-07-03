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
        public readonly CanvasGradientStop[] BackgroundStops = new CanvasGradientStop[]
        {
            new CanvasGradientStop { Position = 0.0f, Color =  Windows.UI.Colors.Red },
            new CanvasGradientStop { Position = 0.16666667f, Color =Windows.UI.Colors.Yellow},
            new CanvasGradientStop { Position = 0.33333333f, Color = Color.FromArgb(255,0,255,0) },
            new CanvasGradientStop { Position = 0.5f, Color = Windows.UI.Colors.Cyan },
            new CanvasGradientStop { Position = 0.66666667f, Color = Windows.UI.Colors.Blue},
            new CanvasGradientStop { Position = 0.83333333f, Color =  Windows.UI.Colors.Magenta },
            new CanvasGradientStop { Position = 1.0f, Color =  Windows.UI.Colors.Red },
        };
        public readonly CanvasGradientStop[] ForegroundStops = new CanvasGradientStop[]
        {
            new CanvasGradientStop { Position = 0.0f, Color = Color.FromArgb(0,128,128,128) },
            new CanvasGradientStop { Position = 1.0f, Color = Windows.UI.Colors.White }
        };

        //@Construct
        public PaletteValue()
        {
            this.Name = "Value";
            this.Unit = "%";
            this.Minimum = 0;
            this.Maximum = 100;
        }

        public override HSV GetHSL(HSV HSV, float value) => new HSV(HSV.A, HSV.H, HSV.S, value);
        public override float GetValue(HSV HSV) => HSV.V;

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

        public override void Draw(CanvasControl CanvasControl, CanvasDrawingSession ds, HSV HSV, Vector2 Center, float SquareHalfWidth, float SquareHalfHeight)
        {
            //Palette
            Rect rect = new Rect(Center.X - SquareHalfWidth, Center.Y - SquareHalfHeight, SquareHalfWidth * 2, SquareHalfHeight * 2);
            using (CanvasLinearGradientBrush rainbow = new CanvasLinearGradientBrush(CanvasControl, this.BackgroundStops))
            {
                rainbow.StartPoint = new Vector2(Center.X - SquareHalfWidth, Center.Y);
                rainbow.EndPoint = new Vector2(Center.X + SquareHalfWidth, Center.Y);
                ds.FillRoundedRectangle(rect, 4, 4, rainbow);
            }
            using (CanvasLinearGradientBrush brush = new CanvasLinearGradientBrush(CanvasControl, this.ForegroundStops))
            {
                brush.StartPoint = new Vector2(Center.X, Center.Y - SquareHalfHeight);
                brush.EndPoint = new Vector2(Center.X, Center.Y + SquareHalfHeight);
                ds.FillRoundedRectangle(rect, 4, 4, brush);
            }
            ds.DrawRoundedRectangle(rect, 4, 4, Windows.UI.Colors.Gray);

            //Thumb 
            float px = ((float)HSV.H - 180) * SquareHalfWidth / 180 + Center.X;
            float py = (50 - (float)HSV.S) * SquareHalfHeight / 50 + Center.Y;
            ds.DrawCircle(px, py, 9, Windows.UI.Colors.Black, 5);
            ds.DrawCircle(px, py, 9, Windows.UI.Colors.White, 3);
        }
        public override HSV Delta(HSV HSV, Vector2 v, float SquareHalfWidth, float SquareHalfHeight)
        {
            float H = v.X * 180 / SquareHalfWidth + 180;
            float S = 50 - v.Y * 50 / SquareHalfHeight;
            return new HSV(HSV.A, H, S, HSV.V);
        }
    }
}

using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Numerics;
using Windows.UI.Xaml.Media;

namespace HSVColorPickers
{
    /// <summary> 
    /// Palette Base. 
    /// </summary>
    public abstract class PaletteBase
    {
        public string Name;
        public string Unit;
        public double Minimum;
        public double Maximum;

        public abstract HSV GetHSL(HSV HSV, float value);
        public abstract float GetValue(HSV HSV);

        public abstract GradientStopCollection GetSliderBrush(HSV HSV);

        public abstract void Draw(CanvasControl CanvasControl, CanvasDrawingSession ds, HSV HSV, Vector2 Center, float SquareHalfWidth, float SquareHalfHeight);
        public abstract HSV Delta(HSV HSV, Vector2 v, float SquareHalfWidth, float SquareHalfHeight);
    }
}
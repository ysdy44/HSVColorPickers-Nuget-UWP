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
        /// <summary> Type name </summary>
        public string Type;
        /// <summary> Unit </summary>
        public string Unit;
        /// <summary> Minimum </summary>
        public double Minimum;
        /// <summary> Maximum </summary>
        public double Maximum;

        /// <summary>
        /// Change the current value to get the HSV
        /// </summary>
        /// <param name="hsv"> HSV</param>
        /// <param name="value"> The source value. </param>
        /// <returns> HSV </returns>
        public abstract HSV GetHSL(HSV hsv, float value);
        /// <summary>
        /// Get the corresponding value from HSV
        /// </summary>
        /// <param name="hsv"></param>
        /// <returns> Value </returns>
        public abstract float GetValue(HSV hsv);

        /// <summary>
        /// Get the slider background brush value from HSV
        /// </summary>
        /// <param name="hsv"> HSV </param>
        /// <returns> GradientStopCollection </returns>
        public abstract GradientStopCollection GetSliderBrush(HSV hsv);

        /// <summary>
        /// Draw
        /// </summary>
        /// <param name="sender"> CanvasControl </param>
        /// <param name="ds"> DrawingSession </param>
        /// <param name="hsv"> HSV </param>
        /// <param name="center"> Center </param>
        /// <param name="squareHalfWidth"> Palette square half width. </param>
        /// <param name="squareHalfHeight"> Palette square half height. </param>
        public abstract void Draw(CanvasControl sender, CanvasDrawingSession ds, HSV hsv, Vector2 center, float squareHalfWidth, float squareHalfHeight);
        /// <summary>
        /// Occurs when dragging on a palette.
        /// </summary>
        /// <param name="hsv"> HSV </param>
        /// <param name="position"> Position </param>
        /// <param name="squareHalfWidth"> Palette square half width. </param>
        /// <param name="squareHalfHeight"> Palette square half height. </param>
        /// <returns> HSV </returns>
        public abstract HSV Delta(HSV hsv, Vector2 position, float squareHalfWidth, float squareHalfHeight);
    }
}
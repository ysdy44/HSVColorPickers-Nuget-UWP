using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using Windows.Graphics.Effects;
using Windows.UI;

namespace HSVColorPickers
{
    /// <summary>
    /// Provides static methods for gray-white spacing of the mesh.
    /// </summary>
    public static class GreyWhiteMeshHelpher
    {
        /// <summary>
        /// Get a 2x2 grey-white Mesh.
        /// </summary>
        /// <param name="resourceCreator"> The resource-creator. </param>
        /// <returns> A 2x2 mesh. </returns>
        public static CanvasBitmap GetGreyWhiteMesh(ICanvasResourceCreator resourceCreator)
        {
            Color[] colors = new Color[]
            {
                 Windows.UI.Colors.LightGray, Windows.UI.Colors.White,
                 Windows.UI.Colors.White, Windows.UI.Colors.LightGray
            };
            return CanvasBitmap.CreateFromColors(resourceCreator, colors, 2, 2);
        }

        /// <summary>
        /// Get LinearGradientBrush with Transparent and DimGray.
        /// </summary>
        /// <param name="resourceCreator"> The resource-creator. </param>
        /// <param name="startPoint"> The start-point. </param>
        /// <param name="endPoint"> The end-point. </param>
        /// <returns> The provided brush. </returns>
        public static CanvasLinearGradientBrush GetLinearGradientBrush(ICanvasResourceCreator resourceCreator, Vector2 startPoint, Vector2 endPoint)
        {
            return new CanvasLinearGradientBrush(resourceCreator, Windows.UI.Colors.Transparent, Windows.UI.Colors.DimGray)
            {
                StartPoint = startPoint,
                EndPoint = endPoint
            };
        }

        /// <summary> 
        /// Gets a CanvasGradientStop array with White and Gray. 
        /// </summary>
        /// <returns> The provided stops. </returns>
        public static CanvasGradientStop[] GetGradientStopArray() => new CanvasGradientStop[]
        {
            new CanvasGradientStop{Color= Colors.White, Position=0.0f },
            new CanvasGradientStop{Color= Colors.Gray, Position=1.0f }
        };

        /// <summary>
        /// Get a extend grey-white Mesh.
        /// </summary>
        /// <param name="scale"> The scaled vector. </param>
        /// <param name="source"> The 2x2 mesh. </param>
        /// <returns> A extend mesh. </returns>
        public static ICanvasImage GetBorderExtendMesh(float scale, IGraphicsEffectSource source)
        {
            return new DpiCompensationEffect
            {
                Source = new ScaleEffect
                {
                    Scale = new Vector2(scale),
                    InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                    Source = new BorderEffect
                    {
                        ExtendX = CanvasEdgeBehavior.Wrap,
                        ExtendY = CanvasEdgeBehavior.Wrap,
                        Source = source
                    }
                }
            };
        }

    }
}
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Graphics.DirectX;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Foundation;

namespace HSVColorPickers
{
    public sealed partial class Eyedropper : UserControl
    {

        /// <summary>
        /// Open the eyedropper.
        /// </summary>
        /// <returns>The picked color.</returns>
        public async Task<Color> OpenAsync()
        {
            await this.OpenCore();

            this.Popup.IsOpen = true;

            Color resultcolor = await this.TaskSource.Task;
            this.TaskSource = null;
            return resultcolor;
        }
        /// <summary>
        /// Open the eyedropper.
        /// </summary>
        /// <param name="placementTarget">Gets or sets the element relative to which the eyedropper is placed.</param>
        /// <returns>The picked color.</returns>
        public async Task<Color> OpenAsync(FrameworkElement placementTarget)
        {
            await this.OpenCore();

            this.Popup.IsOpen = true;

            //@Release
            Point sourcePostion = placementTarget.TransformToVisual(Window.Current.Content).TransformPoint(new Point());
            double offsetX = sourcePostion.X + base.ActualWidth / 2;
            double offsetY = sourcePostion.Y + base.ActualHeight / 2;
            Point position = new Point(offsetX, offsetY);
            this.Postion = position.ToVector2();

            Color resultcolor = await this.TaskSource.Task;
            this.TaskSource = null;
            return resultcolor;
        }
        private async Task OpenCore()
        {
            this.TaskSource = new TaskCompletionSource<Color>();

            Window.Current.CoreWindow.PointerCursor = null;
            this.CanvasWidth = Window.Current.Bounds.Width;
            this.CanvasHeight = Window.Current.Bounds.Height;

            RenderTargetBitmap imageSource = await this.RenderScreenshotAsync();
            this.ImageBrush.ImageSource = imageSource;

            CanvasBitmap screenshot = await this.GetBitmap(imageSource);
            this.ScreenShot = screenshot;
        }


        /// <summary>
        /// Updata the position and color.
        /// </summary>
        /// <param name="pointer">The initial eyedropper pointer</param>
        public void Updata(PointerRoutedEventArgs pointer)
        {
            this.Postion = pointer.GetCurrentPoint(this.RootGrid).Position.ToVector2();
        }


        /// <summary>
        /// Close the eyedropper.
        /// </summary>
        public void Close()
        {
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 0);
            this.Color = this.GetPixelColor();

            this.Popup.IsOpen = false;

            if (this.TaskSource != null && this.TaskSource.Task.IsCanceled == false)
            {
                this.TaskSource.TrySetResult(this.Color);
            }
            this.Dispose();
        }



        private void DrawGlass(CanvasControl sender, CanvasDrawingSession drawingSession)
        {
            // Radius
            float radius = (float)this.Radius;

            // Factor and Pixel
            float factor = (float)this.Factor;
            float halfPixel = factor / 2;

            // Offset = - the Postion + the Radius + half of Pixel
            Vector2 offset = new Vector2(-this.Postion.X + radius + halfPixel, -this.Postion.Y + radius + halfPixel);

            // Draw: ScaleEffect + Transform2DEffect + DpiCompensationEffect
            drawingSession.DrawImage(new DpiCompensationEffect
            {
                SourceDpi = new Vector2(sender.Dpi),
                Source = new Transform2DEffect
                {
                    TransformMatrix = Matrix3x2.CreateTranslation(offset),
                    Source = new ScaleEffect
                    {
                        Scale = new Vector2(factor),
                        CenterPoint = this.Postion,
                        InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                        BorderMode = EffectBorderMode.Hard,
                        Source = this.ScreenShot
                    }
                }
            });
        }


        private Color GetPixelColor()
        {
            if (this.ScreenShot == null) return Colors.White;
            else
            {
                int left = getLeft((int)this.ScreenShot.SizeInPixels.Width, this.Postion.X, this.CanvasWidth);
                int top = getLeft((int)this.ScreenShot.SizeInPixels.Height, this.Postion.Y, this.CanvasHeight);

                return this.ScreenShot.GetPixelColors(left, top, 1, 1).Single();
            }

            int getLeft(int bitmapWidth, float x, double windowWidth)
            {
                int left = (int)(bitmapWidth * (x / windowWidth));

                if (left < 0) return 0;
                else if (left >= bitmapWidth) return bitmapWidth - 1;
                return left;
            }
        }


        private async Task<RenderTargetBitmap> RenderScreenshotAsync()
        {
            try
            {
                RenderTargetBitmap imageSource = new RenderTargetBitmap();
                UIElement element = this.GetAppUIElement();
                await imageSource.RenderAsync(element);

                return imageSource;
            }
            catch (OutOfMemoryException)
            {
                return default;
            }
        }

        private async Task<CanvasBitmap> GetBitmap(RenderTargetBitmap imageSource) => CanvasBitmap.CreateFromBytes(this.CanvasDevice, await imageSource.GetPixelsAsync(), imageSource.PixelWidth, imageSource.PixelHeight, DirectXPixelFormat.B8G8R8A8UIntNormalized);

        private UIElement GetAppUIElement()
        {
            if (Window.Current.Content is FrameworkElement frame)
            {
                if (frame.Parent is FrameworkElement border)
                {
                    if (border.Parent is FrameworkElement rootScrollViewer)
                        return rootScrollViewer;
                    else
                        return border;
                }
                else
                    return frame;
            }
            else return Window.Current.Content;
        }

    }
}

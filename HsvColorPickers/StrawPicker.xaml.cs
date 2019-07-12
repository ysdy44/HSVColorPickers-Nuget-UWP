using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Graphics.DirectX;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace HSVColorPickers
{
    /// <summary>
    ///  Extract colors from the app's screen.
    /// </summary>
    public sealed partial class StrawPicker : UserControl, IColorPicker, IDisposable
    {
        /// <summary> 
        /// A boxed Popup with Postionand Size. 
        /// </summary>
        struct PopupSize
        {
            /// <summary> Popup control </summary>
            readonly Popup Popup;
            public bool IsOpen
            {
                get => this.Popup.IsOpen;
                set => this.Popup.IsOpen = value;
            }

            /// <summary> Popup control's Postion. </summary>
            public Vector2 Postion
            {
                get => this.postion;
                set
                {
                    this.Popup.HorizontalOffset = value.X - this.SizeHalf;
                    this.Popup.VerticalOffset = value.Y - this.SizeHalf;

                    this.postion = value;
                }
            }
            private Vector2 postion;

            readonly float Size;
            readonly Vector2 SizeVector;
            readonly float SizeHalf;
            readonly Vector2 SizeHalfVector;

            /// <summary>
            /// Get the Matrix for CanvasControl
            /// </summary>
            /// <param name="scale"></param>
            /// <returns></returns>
            public Matrix3x2 Matrix(float scale) =>
                Matrix3x2.CreateTranslation(-this.Postion - this.SizeHalfVector) *
                Matrix3x2.CreateScale(scale) *
                Matrix3x2.CreateTranslation(this.SizeVector * scale - this.SizeHalfVector);

            /// <summary>
            /// Initialize PopupSize class
            /// </summary>
            /// <param name="canvasControl"> The CanvasControl. </param>
            /// <param name="textBlock"> The TextBlock. </param>
            /// <param name="brush"> The source brush.</param>
            /// <param name="size">the width and height of popup control</param>
            /// <param name="toolTipSize"></param>
            public PopupSize(UIElement canvasControl, UIElement textBlock, SolidColorBrush brush, float size = 100, float toolTipSize = 20)
            {
                this.postion = Vector2.Zero;

                this.Size = size;
                this.SizeVector = new Vector2(size, size);

                this.SizeHalf = size / 2;
                this.SizeHalfVector = new Vector2(this.SizeHalf, this.SizeHalf);

                this.Popup = new Popup
                {
                    IsHitTestVisible = false,
                    IsOpen = false,
                    Child = new Border
                    {
                        Width = this.Size,
                        Height = this.Size,
                        CornerRadius = new CornerRadius(this.SizeHalf),
                        BorderThickness = new Thickness(1),
                        BorderBrush = new SolidColorBrush(Windows.UI.Colors.Black),
                        Child = new Grid
                        {
                            Children ={
                            canvasControl,
                            new StackPanel
                            {
                                VerticalAlignment=VerticalAlignment.Bottom,
                                Children =
                                {
                                    new Border{
                                        Child = textBlock,
                                        Height =toolTipSize,
                                        Padding=new Thickness(toolTipSize/6,0,toolTipSize/3,0),
                                        CornerRadius=new CornerRadius(toolTipSize/2),
                                        HorizontalAlignment=HorizontalAlignment.Center,
                                        Background=new SolidColorBrush{
                                            Color=Windows.UI.Colors.Black,
                                            Opacity=0.7
                                        },
                                    },
                                    new Ellipse{
                                        Fill=brush,
                                        Width=16,
                                        Height=16,
                                        StrokeThickness= 1,
                                        Margin=new Thickness(4),
                                        Stroke=new SolidColorBrush(Windows.UI.Colors.Black)
                                    }
                                }
                            }
                        }
                        }
                    }
                };
            }
        }

        /// <summary> 
        /// Screenshot library. 
        /// </summary>
        class StrawRender
        {

            public static Color GetColor(CanvasBitmap bitmap, Vector2 v)
            {
                if (bitmap == null) return Windows.UI.Colors.White;

                int left = StrawRender.GetLeft((int)bitmap.SizeInPixels.Width, v.X, Window.Current.Bounds.Width);
                int top = StrawRender.GetLeft((int)bitmap.SizeInPixels.Height, v.Y, Window.Current.Bounds.Height);

                return bitmap.GetPixelColors(left, top, 1, 1).Single();
            }


            public static async Task<CanvasBitmap> GetRenderTargetBitmap(ICanvasResourceCreator creator, UIElement element)
            {
                RenderTargetBitmap render = new RenderTargetBitmap();
                await render.RenderAsync(element);
                return CanvasBitmap.CreateFromBytes(creator, await render.GetPixelsAsync(), render.PixelWidth, render.PixelHeight, DirectXPixelFormat.B8G8R8A8UIntNormalized);
            }

            private static int GetLeft(int bitmapWidth, float x, double windowWidth)
            {
                int left = (int)(bitmapWidth * (x / windowWidth));

                if (left < 0) return 0;
                else if (left >= bitmapWidth) return bitmapWidth - 1;
                return left;
            }

        }

        
        //@Delegate
        /// <summary> Occurs when the color value changes. </summary>
        public event ColorChangeHandler ColorChange = null;


        /// <summary> Gets picker's type name. </summary>
        public string Type => "Straw";
        /// <summary> Gets picker self. </summary>
        public UserControl Self => this;

        /// <summary> Gets or Sets picker's color. </summary>
        public Color Color
        {
            get => this.SolidColorBrushName.Color;
            set => this.SolidColorBrushName.Color = value;
        }


        //Bitmap
        CanvasDevice Device = new CanvasDevice();
        CanvasBitmap Bitmap;

        //Canvas
        PopupSize Size;
        CanvasControl CanvasControl;

        TextBlock TextBlock = new TextBlock
        {
            FontSize = 12,
            Foreground = new SolidColorBrush(Windows.UI.Colors.White),
            VerticalAlignment = VerticalAlignment.Center
        };
        

        //@Construct
        /// <summary>
        /// Construct a StrawPicker.
        /// </summary>
        public StrawPicker()
        {
            this.InitializeComponent();
            this.Ellipse.Tapped += (sender, e) => this.ColorChange?.Invoke(this, this.Color);//Delegate

            //Canvas
            this.CanvasControl = new CanvasControl { UseSharedDevice = true, CustomDevice = this.Device };
            this.Size = new PopupSize(this.CanvasControl, this.TextBlock, this.SolidColorBrushName, 100);
            this.CanvasControl.Draw += (sender, args) =>
            {
                if (this.Bitmap == null) return;

                args.DrawingSession.DrawImage(new Transform2DEffect
                {
                    TransformMatrix = this.Size.Matrix(2),
                    Source = new DpiCompensationEffect
                    {
                        SourceDpi = new Vector2(sender.Dpi),
                        Source = this.Bitmap
                    }
                });
            };

            //Manipulation
            this.IconBorder.ManipulationMode = ManipulationModes.All;
            this.IconBorder.PointerPressed += (s, e) => this.Size.Postion = e.GetCurrentPoint(Window.Current.Content).Position.ToVector2();
            this.IconBorder.ManipulationStarted += async (s, e) =>
            {
                this.Bitmap = await StrawRender.GetRenderTargetBitmap(this.Device, Window.Current.Content);

                this.Color = StrawRender.GetColor(this.Bitmap, this.Size.Postion);

                this.Size.IsOpen = true;
            };
            this.IconBorder.ManipulationDelta += (s, e) =>
            {
                this.Size.Postion += e.Delta.Translation.ToVector2();

                this.Color = StrawRender.GetColor(this.Bitmap, this.Size.Postion);
                this.CanvasControl.Invalidate();
                this.TextBlock.Text = this.Color.ToString();
            };
            this.IconBorder.ManipulationCompleted += (s, e) =>
            {
                this.Color = StrawRender.GetColor(this.Bitmap, this.Size.Postion);

                this.Dispose();

                this.Size.IsOpen = false;
            };
        }

        /// <summary>
        /// Performs the task defined by the application associated with releasing or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.Bitmap == null) return;

            this.Bitmap.Dispose();
            this.Bitmap = null;
        }
    }
}
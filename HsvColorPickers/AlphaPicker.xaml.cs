using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace HSVColorPickers
{
    public sealed partial class AlphaPicker : UserControl
    {
        //Delegate
        public event AlphaChangeHandler AlphaChange;

        float CanvasWidth;
        float CanvasHeight;

        #region DependencyProperty


        private byte alpha = 255;
        private byte _Alpha
        {
            get => this.alpha;
            set
            {
                this.AlphaChange?.Invoke(this, value);
                CanvasControl.Invalidate();

                this.alpha = value;
            }
        }
        public byte Alpha
        {
            get => this.alpha;
            set
            {
                //A
                this.ASlider.Value = this.APicker.Value = (int)value;

                this.alpha = value;
            }
        }


        #endregion

        public AlphaPicker()
        {
            this.InitializeComponent();
           
            //Slider
            this.ASlider.ValueChangeDelta += (object sender, double value) => this.Alpha = this._Alpha = (byte)value;
            //Picker
            this.APicker.ValueChange += (object sender, int value) => this.Alpha = this._Alpha = (byte)value;

            //Canvas
            this.CanvasControl.SizeChanged += (s, e) =>
            {
                if (e.NewSize == e.PreviousSize) return;
                this.CanvasWidth = (float)e.NewSize.Width;
                this.CanvasHeight = (float)e.NewSize.Height;
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                args.DrawingSession.DrawImage(new DpiCompensationEffect
                {
                    Source = new ScaleEffect
                    {
                        Scale = new Vector2(this.CanvasHeight / 3),
                        InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                        Source = new BorderEffect
                        {
                            ExtendX = CanvasEdgeBehavior.Wrap,
                            ExtendY = CanvasEdgeBehavior.Wrap,
                            Source = CanvasBitmap.CreateFromColors
                            (
                                resourceCreator: sender,
                                widthInPixels: 2,
                                heightInPixels: 2,
                                colors: new Color[]
                                {
                                 Windows.UI.Colors.LightGray,
                                 Windows.UI.Colors.White,
                                 Windows.UI.Colors.White,
                                 Windows.UI.Colors.LightGray
                                }
                             )
                        }
                    }
                });
                
                args.DrawingSession.FillRectangle(0, 0, this.CanvasWidth, this.CanvasHeight, new CanvasLinearGradientBrush(sender, Windows.UI.Colors.Transparent, Windows.UI.Colors.DimGray)
                {
                    StartPoint = new Vector2(0, this.CanvasHeight / 2),
                    EndPoint = new Vector2(this.CanvasWidth, this.CanvasHeight / 2)
                });
            };
        }       
    }
}

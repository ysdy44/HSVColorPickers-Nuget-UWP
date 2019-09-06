using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace HSVColorPickers
{
    /// <summary>
    /// Color alpha picker.
    /// </summary>
    public sealed partial class AlphaPicker : UserControl, IColorPicker, IAlphaPicker
    {
        //@Delegate
        /// <summary> Occurs when the color value changes. </summary>
        public event ColorChangeHandler ColorChange;
        /// <summary> Occurs when the alpha value changes. </summary>
        public event AlphaChangeHandler AlphaChange;


        /// <summary> Gets picker's type name. </summary>
        public string Type => "Alpha";
        /// <summary> Gets picker self. </summary>
        public UserControl Self => this;

        /// <summary> Gets or Sets picker's color. </summary>
        public Color Color
        {
            get => Color.FromArgb(this.Alpha, 255, 255, 255);
            set => this.Alpha = value.A;
        }


        #region DependencyProperty 


        /// <summary> Gets or Sets picker's alpha. </summary>
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

        private byte alpha = 255;
        private byte _Alpha
        {
            get => this.alpha;
            set
            {
                this.ColorChange?.Invoke(this, Color.FromArgb(value, 255, 255, 255));//Delegate
                this.AlphaChange?.Invoke(this, value);//Delegate

                this.CanvasControl.Invalidate();

                this.alpha = value;
            }
        }
        
        /// <summary> Get or set the flyout style. </summary>
        public Style FlyoutPresenterStyle
    {
        get { return (Style)GetValue(FlyoutPresenterStyleProperty); }
        set { SetValue(FlyoutPresenterStyleProperty, value); }
    }
    /// <summary> Identifies the <see cref = "NumberPicker.FlyoutPresenterStyle" /> dependency property. </summary>
    public static readonly DependencyProperty FlyoutPresenterStyleProperty = DependencyProperty.Register(nameof(FlyoutPresenterStyle), typeof(Style), typeof(AlphaPicker), new PropertyMetadata(null));


    /// <summary> Get or set the flyout placement. </summary>
    public FlyoutPlacementMode Placement
    {
        get { return (FlyoutPlacementMode)GetValue(PlacementProperty); }
        set { SetValue(PlacementProperty, value); }
    }
    /// <summary> Identifies the <see cref = "NumberPicker.Placement" /> dependency property. </summary>
    public static readonly DependencyProperty PlacementProperty = DependencyProperty.Register(nameof(Placement), typeof(FlyoutPlacementMode), typeof(AlphaPicker), new PropertyMetadata(FlyoutPlacementMode.Bottom));


    #endregion


    float CanvasWidth;
        float CanvasHeight;
        CanvasBitmap Bitmap;

        CanvasLinearGradientBrush Brush;
        Vector2 StartPoint;
        Vector2 EndPoint;


        //@Construct
        /// <summary>
        /// Construct a AlphaPicker.
        /// </summary>
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

                this.StartPoint = new Vector2(0, this.CanvasHeight / 2);
                this.EndPoint = new Vector2(this.CanvasWidth, this.CanvasHeight / 2);

                if (this.Brush != null)
                {
                    this.Brush.StartPoint = this.StartPoint;
                    this.Brush.EndPoint = this.EndPoint;
                }
            };
            this.CanvasControl.CreateResources += (sender, args) =>
            {
                Color[] colors = new Color[]
                {
                     Windows.UI.Colors.LightGray, Windows.UI.Colors.White,
                     Windows.UI.Colors.White, Windows.UI.Colors.LightGray
                };
                this.Bitmap = CanvasBitmap.CreateFromColors(sender, colors, 2, 2);

                this.Brush = new CanvasLinearGradientBrush(sender, Windows.UI.Colors.Transparent, Windows.UI.Colors.DimGray)
                {
                    StartPoint = this.StartPoint,
                    EndPoint = this.EndPoint
                };
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
                            Source = this.Bitmap
                        }
                    }
                });

                args.DrawingSession.FillRectangle(0, 0, this.CanvasWidth, this.CanvasHeight, this.Brush);
            };
        }
    }
}
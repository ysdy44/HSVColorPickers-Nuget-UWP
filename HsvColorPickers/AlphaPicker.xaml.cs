using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using Windows.Graphics.Effects;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace HSVColorPickers
{
    /// <summary>
    /// Color alpha picker.
    /// </summary>
    public sealed partial class AlphaPicker : UserControl, IAlphaPicker
    {

        //@Delegate
        /// <summary> Occurs when alpha changed. </summary>
        public event AlphaChangeHandler AlphaChanged;
        /// <summary> Occurs when the alpha changed starts. </summary>
        public event AlphaChangeHandler AlphaChangeStarted;
        /// <summary> Occurs when alpha changed. </summary>
        public event AlphaChangeHandler AlphaChangeDelta;
        /// <summary> Occurs when the alpha changed is complete. </summary>
        public event AlphaChangeHandler AlphaChangeCompleted;


        /// <summary> Gets picker's type name. </summary>
        public string Type => "Alpha";
        /// <summary> Gets picker self. </summary>
        public Control Self => this;

        
        #region Color


        /// <summary> Gets or sets picker's color. </summary>
        public Color Color
        {
            get => Color.FromArgb(this.Alpha, 255, 255, 255);
            set => this.Alpha = value.A;
        }


        /// <summary> Gets or sets picker's alpha. </summary>
        public byte Alpha
        {
            get => this.alpha;
            set
            {
                this.Change(value);
                this.alpha = value;
            }
        }
        private byte alpha = 255;


        #endregion


        #region DependencyProperty 


        /// <summary> Get or set the button style. </summary>
        public Style ButtonStyle
        {
            get => (Style)base.GetValue(ButtonStyleProperty);
            set => base.SetValue(ButtonStyleProperty, value);
        }
        /// <summary> Identifies the <see cref = "NumberPicker.ButtonStyle" /> dependency property. </summary>
        public static readonly DependencyProperty ButtonStyleProperty = DependencyProperty.Register(nameof(ButtonStyle), typeof(Style), typeof(NumberPicker), new PropertyMetadata(null));

        /// <summary> Get or set the flyout style. </summary>
        public Style FlyoutPresenterStyle
        {
            get => (Style)base.GetValue(FlyoutPresenterStyleProperty);
            set => base.SetValue(FlyoutPresenterStyleProperty, value);
        }
        /// <summary> Identifies the <see cref = "NumberPicker.FlyoutPresenterStyle" /> dependency property. </summary>
        public static readonly DependencyProperty FlyoutPresenterStyleProperty = DependencyProperty.Register(nameof(FlyoutPresenterStyle), typeof(Style), typeof(AlphaPicker), new PropertyMetadata(null));


        /// <summary> Get or set the flyout placement. </summary>
        public FlyoutPlacementMode Placement
        {
            get => (FlyoutPlacementMode)base.GetValue(PlacementProperty);
            set => base.SetValue(FlyoutPresenterStyleProperty, value);
        }
        /// <summary> Identifies the <see cref = "NumberPicker.Placement" /> dependency property. </summary>
        public static readonly DependencyProperty PlacementProperty = DependencyProperty.Register(nameof(Placement), typeof(FlyoutPlacementMode), typeof(AlphaPicker), new PropertyMetadata(FlyoutPlacementMode.Bottom));
    
        
        /// <summary>  Gets or sets a brush that describes the border fill of the control. </summary>
        public SolidColorBrush Stroke
        {
            get => (SolidColorBrush)base.GetValue(StrokeProperty);
            set => base.SetValue(StrokeProperty, value);
        }

        /// <summary> Identifies the <see cref = "AlphaPicker.Stroke" /> dependency property. </summary>
        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(nameof(Stroke), typeof(SolidColorBrush), typeof(AlphaPicker), new PropertyMetadata(new SolidColorBrush(Windows.UI.Colors.Gray)));


        #endregion


        float _canvasWidth;
        float _canvasHeight;
        CanvasBitmap _bitmap;

        CanvasLinearGradientBrush _brush;
        Vector2 _startPoint;
        Vector2 _endPoint;


        //@Construct
        /// <summary>
        /// Construct a AlphaPicker.
        /// </summary>
        public AlphaPicker()
        {
            this.InitializeComponent();

            //Slider
            this.ASlider.ValueChangeStarted += (s, value) =>
            {
                byte alpha = (byte)value;
                this.alpha = alpha;
                this.AlphaChangeStarted?.Invoke(this, alpha);//Delegate
            };
            this.ASlider.ValueChangeDelta += (s, value) =>
            {
                byte alpha = (byte)value;
                this.alpha = alpha;
                this.AlphaChangeDelta?.Invoke(this, alpha);//Delegate
            };
            this.ASlider.ValueChangeCompleted += (s, value) =>
            {
                byte alpha = (byte)value;
                this.alpha = alpha;
                this.AlphaChangeCompleted?.Invoke(this, alpha);//Delegate
            };

            //Picker
            this.APicker.ValueChanged += (s, value) =>
            {
                byte alpha = (byte)value;
                this.alpha = alpha;
                this.AlphaChanged?.Invoke(this, alpha);//Delegate
            };

            //Canvas
            this.CanvasControl.SizeChanged += (s, e) =>
            {
                if (e.NewSize == e.PreviousSize) return;
                this._canvasWidth = (float)e.NewSize.Width;
                this._canvasHeight = (float)e.NewSize.Height;

                this._startPoint = new Vector2(0, this._canvasHeight / 2);
                this._endPoint = new Vector2(this._canvasWidth, this._canvasHeight / 2);

                if (this._brush != null)
                {
                    this._brush.StartPoint = this._startPoint;
                    this._brush.EndPoint = this._endPoint;
                }
            };
            this.CanvasControl.CreateResources += (sender, args) =>
            {
                this._bitmap = GreyWhiteMeshHelpher.GetGreyWhiteMesh(sender);
                this._brush = GreyWhiteMeshHelpher.GetLinearGradientBrush(sender, this._startPoint, this._endPoint);
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                args.DrawingSession.DrawImage(GreyWhiteMeshHelpher.GetBorderExtendMesh(this._canvasHeight / 3, this._bitmap));
                args.DrawingSession.FillRectangle(0, 0, this._canvasWidth, this._canvasHeight, this._brush);
            };
        }

        #region Change


        private byte Change(byte value, bool? sliderOrPicker = null)
        {
            byte A = value;

            if (sliderOrPicker != false) this.ASlider.Value = A;
            if (sliderOrPicker != true) this.APicker.Value = A;
            
            this.CanvasControl.Invalidate();
            return A;
        }


        #endregion

    }
}
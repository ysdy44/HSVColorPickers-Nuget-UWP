using System;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace HSVColorPickers
{
    /// <summary>
    /// Color palette picker.
    /// </summary>
    public partial class PalettePicker : UserControl, IColorPicker, IHSVPicker
    {

        #region Helper


        private class PaletteSquare
        {
            public Vector2 Center = new Vector2(50, 50);
            public float Width = 100;
            public float Height = 100;
            public float HalfWidth => this.Width / 2;
            public float HalfHeight => this.Height / 2;
            public float StrokePadding = 12;
        }


        #endregion


        //@Delegate
        /// <summary> Occurs when the color value changed. </summary>
        public event ColorChangeHandler ColorChanged;
        /// <summary> Occurs when the color change starts. </summary>
        public event ColorChangeHandler ColorChangeStarted;
        /// <summary> Occurs when color change. </summary>
        public event ColorChangeHandler ColorChangeDelta;
        /// <summary> Occurs when the color change is complete. </summary>
        public event ColorChangeHandler ColorChangeCompleted;
        /// <summary> Occurs when the hsv value changed. </summary>
        public event HSVChangeHandler HSVChanged;
        /// <summary> Occurs when the hsv change starts. </summary>
        public event HSVChangeHandler HSVChangeStarted;
        /// <summary> Occurs when hsv change. </summary>
        public event HSVChangeHandler HSVChangeDelta;
        /// <summary> Occurs when the hsv change is complete. </summary>
        public event HSVChangeHandler HSVChangeCompleted;


        /// <summary> Gets picker's type name. </summary>
        public string Type { get; set; } = "Palette";
        /// <summary> Gets picker self. </summary>
        public UserControl Self => this;



        #region Color


        /// <summary> Gets or sets picker's color. </summary>
        public Color Color
        {
            get => HSV.HSVtoRGB(this.HSV);
            set => this.HSV = HSV.RGBtoHSV(value);
        }


        /// <summary> Gets or sets picker's hsv. </summary>
        public HSV HSV
        {
            get => this.hsv;
            set
            {
                this.Action(value);
                this.hsv = value;

                this.CanvasControl.Invalidate();
            }
        }
        private HSV hsv = new HSV(255, 0, 100, 100);


        private HSV _HSV
        {
            get => this.hsv;
            set
            {
                this.ColorChanged?.Invoke(this, HSV.HSVtoRGB(value.A, value.H, value.S, value.V));//Delegate
                this.HSVChanged?.Invoke(this, value);//Delegate

                this.hsv = value;
            }
        }
        private HSV _HSVStarted
        {
            set
            {
                this.ColorChangeStarted?.Invoke(this, HSV.HSVtoRGB(value.A, value.H, value.S, value.V));//Delegate
                this.HSVChangeStarted?.Invoke(this, value);//Delegate

                this.hsv = value;
            }
        }
        private HSV _HSVDelta
        {
            set
            {
                this.ColorChangeDelta?.Invoke(this, HSV.HSVtoRGB(value.A, value.H, value.S, value.V));//Delegate
                this.HSVChangeDelta?.Invoke(this, value);//Delegate

                this.hsv = value;
            }
        }
        private HSV _HSVCompleted
        {
            set
            {
                this.ColorChangeCompleted?.Invoke(this, HSV.HSVtoRGB(value.A, value.H, value.S, value.V));//Delegate
                this.HSVChangeCompleted?.Invoke(this, value);//Delegate

                this.hsv = value;
            }
        }


        #endregion


        #region DependencyProperty


        /// <summary>  Gets or sets a brush that describes the border fill of the control. </summary>
        public SolidColorBrush Stroke
        {
            get { return (SolidColorBrush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        /// <summary> Identifies the <see cref = "WheelPicker.Stroke" /> dependency property. </summary>
        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(nameof(Stroke), typeof(SolidColorBrush), typeof(WheelPicker), new PropertyMetadata(new SolidColorBrush(Windows.UI.Colors.Gray)));


        #endregion

        bool IsPalette = false;
        Vector2 Vector;
        Action<HSV> Action;
        PaletteSquare Square = new PaletteSquare();


        //@Construct
        /// <summary>
        /// Construct a PalettePicker.
        /// </summary>
        /// <param name="paletteBase"> The source base. </param>
        public PalettePicker(PaletteBase paletteBase)
        {
            this.InitializeComponent();
            this.Type = paletteBase.Type;

            //Picker
            this.Slider.Minimum = paletteBase.Minimum;
            this.Slider.Maximum = paletteBase.Maximum;

            this.Slider.Value = paletteBase.GetValue(this.hsv);
            this.LinearGradientBrush.GradientStops = paletteBase.GetSliderBrush(this.hsv);

            this.Slider.ValueChangeStarted += (sender, value) => this.HSV = this._HSVStarted = paletteBase.GetHSV(this.hsv, (float)value);
            this.Slider.ValueChangeDelta += (sender, value) => this.HSV = this._HSVDelta = paletteBase.GetHSV(this.hsv, (float)value);
            this.Slider.ValueChangeCompleted += (sender, value) => this.HSV = this._HSVCompleted = paletteBase.GetHSV(this.hsv, (float)value);


            //Action
            this.Action = (HSV hsv) =>
            {
                this.Slider.Value = paletteBase.GetValue(hsv);
                this.LinearGradientBrush.GradientStops = paletteBase.GetSliderBrush(hsv);
            };

            //Canvas
            this.CanvasControl.SizeChanged += (sender, e) =>
            {
                this.Square.Center = e.NewSize.ToVector2() / 2;

                this.Square.Width = (float)e.NewSize.Width - this.Square.StrokePadding * 2;
                this.Square.Height = (float)e.NewSize.Height - this.Square.StrokePadding * 2;
            };
            this.CanvasControl.Draw += (sender, args) => paletteBase.Draw(this.CanvasControl, args.DrawingSession, this.hsv, this.Square.Center, this.Square.HalfWidth, this.Square.HalfHeight, this.Stroke);



            //Manipulation
            this.CanvasControl.ManipulationMode = ManipulationModes.All;
            this.CanvasControl.ManipulationStarted += (sender, e) =>
            {
                this.Vector = e.Position.ToVector2() - this.Square.Center;

                this.IsPalette = Math.Abs(Vector.X) < this.Square.Width && Math.Abs(this.Vector.Y) < this.Square.Height;

                if (this.IsPalette) this.HSV = this._HSVStarted = paletteBase.Delta(this.hsv, this.Vector, this.Square.HalfWidth, this.Square.HalfHeight);
            };
            this.CanvasControl.ManipulationDelta += (sender, e) =>
            {
                this.Vector += e.Delta.Translation.ToVector2();

                if (this.IsPalette) this.HSV = this._HSVDelta = paletteBase.Delta(this.hsv, this.Vector, this.Square.HalfWidth, this.Square.HalfHeight);
            };
            this.CanvasControl.ManipulationCompleted += (sender, e) =>
            {
                if (this.IsPalette) this.HSV = this._HSVCompleted = paletteBase.Delta(this.hsv, this.Vector, this.Square.HalfWidth, this.Square.HalfHeight);
                this.IsPalette = false;
            };


            this.CanvasControl.Invalidate();
        }
    }
}
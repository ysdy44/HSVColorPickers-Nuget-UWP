using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
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
    public abstract partial class PalettePicker : UserControl, IColorPicker, IHSVPicker
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
        public event ColorChangedHandler ColorChanged;
        /// <summary> Occurs when the color changed starts. </summary>
        public event ColorChangedHandler ColorChangedStarted;
        /// <summary> Occurs when color changed. </summary>
        public event ColorChangedHandler ColorChangedDelta;
        /// <summary> Occurs when the color changed is complete. </summary>
        public event ColorChangedHandler ColorChangedCompleted;
        /// <summary> Occurs when the hsv value changed. </summary>
        public event HSVChangedHandler HSVChanged;
        /// <summary> Occurs when the hsv changed starts. </summary>
        public event HSVChangedHandler HSVChangeStarted;
        /// <summary> Occurs when hsv changed. </summary>
        public event HSVChangedHandler HSVChangeDelta;
        /// <summary> Occurs when the hsv changed is complete. </summary>
        public event HSVChangedHandler HSVChangeCompleted;


        //@Abstract
        /// <summary> Gets picker's type name. </summary>
        public abstract string Type { get; set; }
        /// <summary> Unit </summary>
        public abstract string Unit { get; set; }
        /// <summary> Minimum </summary>
        public abstract double Minimum { get; set; }
        /// <summary> Maximum </summary>
        public abstract double Maximum { get; set; }


        /// <summary>
        /// Change the current value to get the HSV
        /// </summary>
        /// <param name="hsv"> HSV</param>
        /// <param name="value"> The source value. </param>
        /// <returns> HSV </returns>
        public abstract HSV GetHSV(HSV hsv, float value);
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
        /// <param name="stroke"> The stroke brush. </param>
        public abstract void Draw(CanvasControl sender, CanvasDrawingSession ds, HSV hsv, Vector2 center, float squareHalfWidth, float squareHalfHeight, SolidColorBrush stroke);
        /// <summary>
        /// Occurs when dragging on a palette.
        /// </summary>
        /// <param name="hsv"> HSV </param>
        /// <param name="position"> Position </param>
        /// <param name="squareHalfWidth"> Palette square half width. </param>
        /// <param name="squareHalfHeight"> Palette square half height. </param>
        /// <returns> HSV </returns>
        public abstract HSV Delta(HSV hsv, Vector2 position, float squareHalfWidth, float squareHalfHeight);

        /// <summary> Gets picker self. </summary>
        public Control Self => this;



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
                this.Slider.Value = this.GetValue(value);
                this.LinearGradientBrush.GradientStops = this.GetSliderBrush(value);
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
                this.ColorChangedStarted?.Invoke(this, HSV.HSVtoRGB(value.A, value.H, value.S, value.V));//Delegate
                this.HSVChangeStarted?.Invoke(this, value);//Delegate

                this.hsv = value;
            }
        }
        private HSV _HSVDelta
        {
            set
            {
                this.ColorChangedDelta?.Invoke(this, HSV.HSVtoRGB(value.A, value.H, value.S, value.V));//Delegate
                this.HSVChangeDelta?.Invoke(this, value);//Delegate

                this.hsv = value;
            }
        }
        private HSV _HSVCompleted
        {
            set
            {
                this.ColorChangedCompleted?.Invoke(this, HSV.HSVtoRGB(value.A, value.H, value.S, value.V));//Delegate
                this.HSVChangeCompleted?.Invoke(this, value);//Delegate

                this.hsv = value;
            }
        }


        #endregion


        #region DependencyProperty


        /// <summary>  Gets or sets a brush that describes the border fill of the control. </summary>
        public SolidColorBrush Stroke
        {
            get => (SolidColorBrush)base.GetValue(StrokeProperty);
            set => base.SetValue(StrokeProperty, value);
        }
        /// <summary> Identifies the <see cref = "WheelPicker.Stroke" /> dependency property. </summary>
        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(nameof(Stroke), typeof(SolidColorBrush), typeof(WheelPicker), new PropertyMetadata(new SolidColorBrush(Windows.UI.Colors.Gray)));


        #endregion


        bool IsPalette = false;
        Vector2 Vector;
        readonly PaletteSquare Square = new PaletteSquare();


        //@Construct
        /// <summary>
        /// Construct a PalettePicker.
        /// </summary>
        public PalettePicker()
        {
            this.InitializeComponent();

            //Picker
            this.Slider.Minimum = this.Minimum;
            this.Slider.Maximum = this.Maximum;

            this.Slider.Value = this.GetValue(this.hsv);
            this.LinearGradientBrush.GradientStops = this.GetSliderBrush(this.hsv);

            this.Slider.ValueChangeStarted += (sender, value) => this.HSV = this._HSVStarted = this.GetHSV(this.hsv, (float)value);
            this.Slider.ValueChangeDelta += (sender, value) => this.HSV = this._HSVDelta = this.GetHSV(this.hsv, (float)value);
            this.Slider.ValueChangeCompleted += (sender, value) => this.HSV = this._HSVCompleted = this.GetHSV(this.hsv, (float)value);


            //Canvas
            this.CanvasControl.SizeChanged += (sender, e) =>
            {
                this.Square.Center = e.NewSize.ToVector2() / 2;

                this.Square.Width = (float)e.NewSize.Width - this.Square.StrokePadding * 2;
                this.Square.Height = (float)e.NewSize.Height - this.Square.StrokePadding * 2;
            };
            this.CanvasControl.Draw += (sender, args) => this.Draw(this.CanvasControl, args.DrawingSession, this.hsv, this.Square.Center, this.Square.HalfWidth, this.Square.HalfHeight, this.Stroke);


            //Pointer
            this.CanvasControl.PointerPressed += (s, e) =>
            {
                base.CapturePointer(e.Pointer);
            };
            this.CanvasControl.PointerReleased += (s, e) =>
            {
                base.ReleasePointerCapture(e.Pointer);
            };


            //Manipulation
            this.CanvasControl.ManipulationMode = ManipulationModes.All;
            this.CanvasControl.ManipulationStarted += (sender, e) =>
            {
                this.Vector = e.Position.ToVector2() - this.Square.Center;

                this.IsPalette = Math.Abs(Vector.X) < this.Square.Width && Math.Abs(this.Vector.Y) < this.Square.Height;

                if (this.IsPalette) this.HSV = this._HSVStarted = this.Delta(this.hsv, this.Vector, this.Square.HalfWidth, this.Square.HalfHeight);
            };
            this.CanvasControl.ManipulationDelta += (sender, e) =>
            {
                this.Vector += e.Delta.Translation.ToVector2();

                if (this.IsPalette) this.HSV = this._HSVDelta = this.Delta(this.hsv, this.Vector, this.Square.HalfWidth, this.Square.HalfHeight);
            };
            this.CanvasControl.ManipulationCompleted += (sender, e) =>
            {
                if (this.IsPalette) this.HSV = this._HSVCompleted = this.Delta(this.hsv, this.Vector, this.Square.HalfWidth, this.Square.HalfHeight);
                this.IsPalette = false;
            };


            this.CanvasControl.Invalidate();
        }
    }
}
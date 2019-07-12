using System;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace HSVColorPickers
{
    /// <summary>
    /// Color palette picker.
    /// </summary>
    public partial class PalettePicker : UserControl, IColorPicker, IHSVPicker
    {
        private class PaletteSquare
        {
            public Vector2 Center = new Vector2(50, 50);
            public float Width = 100;
            public float Height = 100;
            public float HalfWidth => this.Width / 2;
            public float HalfHeight => this.Height / 2;
            public float StrokePadding = 12;
        }

        //@Delegate
        /// <summary> Occurs when the color value changes. </summary>
        public event ColorChangeHandler ColorChange = null;
        /// <summary> Occurs when the hsv value changes. </summary>
        public event HSVChangeHandler HSVChange = null;


        /// <summary> Gets picker's type name. </summary>
        public string Type { get; set; } = "Palette";
        /// <summary> Gets picker self. </summary>
        public UserControl Self => this;

        /// <summary> Gets or Sets picker's color. </summary>
        public Color Color
        {
            get => HSV.HSVtoRGB(this.HSV);
            set => this.HSV = HSV.RGBtoHSV(value);
        }

        #region DependencyProperty


        /// <summary> Gets or Sets picker's hsv. </summary>
        public HSV HSV
        {
            get => this.hsl;
            set
            {
                this.Action(value);
                this.hsl = value;

                this.CanvasControl.Invalidate();
            }
        }

        private HSV hsl = new HSV { A = 255, H = 0, S = 1, V = 1 };
        private HSV _HSL
        {
            get => this.hsl;
            set
            {
                this.ColorChange?.Invoke(this, HSV.HSVtoRGB(value.A, value.H, value.S, value.V));//Delegate
                this.HSVChange?.Invoke(this, value);//Delegate

                this.hsl = value;
            }
        }


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

            this.Slider.Value = paletteBase.GetValue(this.hsl);
            this.LinearGradientBrush.GradientStops = paletteBase.GetSliderBrush(this.hsl);

            this.Slider.ValueChangeDelta += (sender, value) => this.HSV = this._HSL = paletteBase.GetHSL(this.hsl, (float)value);

            //Action
            this.Action = (HSV hsl) =>
            {
                this.Slider.Value = paletteBase.GetValue(hsl);
                this.LinearGradientBrush.GradientStops = paletteBase.GetSliderBrush(hsl);
            };

            //Canvas
            this.CanvasControl.SizeChanged += (sender, e) =>
            {
                this.Square.Center = e.NewSize.ToVector2() / 2;

                this.Square.Width = (float)e.NewSize.Width - this.Square.StrokePadding * 2;
                this.Square.Height = (float)e.NewSize.Height - this.Square.StrokePadding * 2;
            };
            this.CanvasControl.Draw += (sender, args) => paletteBase.Draw(this.CanvasControl, args.DrawingSession, this.hsl, this.Square.Center, this.Square.HalfWidth, this.Square.HalfHeight);



            //Manipulation
            this.CanvasControl.ManipulationMode = ManipulationModes.All;
            this.CanvasControl.ManipulationStarted += (sender, e) =>
            {
                this.Vector = e.Position.ToVector2() - this.Square.Center;

                this.IsPalette = Math.Abs(Vector.X) < this.Square.Width && Math.Abs(this.Vector.Y) < this.Square.Height;

                if (this.IsPalette) this.HSV = this._HSL = paletteBase.Delta(this.hsl, this.Vector, this.Square.HalfWidth, this.Square.HalfHeight);
            };
            this.CanvasControl.ManipulationDelta += (sender, e) =>
            {
                this.Vector += e.Delta.Translation.ToVector2();

                if (this.IsPalette) this.HSV = this._HSL = paletteBase.Delta(this.hsl, this.Vector, this.Square.HalfWidth, this.Square.HalfHeight);
            };
            this.CanvasControl.ManipulationCompleted += (sender, e) => this.IsPalette = false;



            this.CanvasControl.Invalidate();
        }


        //@Static
        /// <summary>
        ///  Create form PaletteHue.
        /// </summary>
        /// <returns> PalettePicker </returns>
        public static PalettePicker CreateFormHue() => new PalettePicker(new PaletteHue());
        /// <summary>
        ///  Create form PaletteSaturation.
        /// </summary>
        /// <returns> PalettePicker </returns>
        public static PalettePicker CreateFormSaturation() => new PalettePicker(new PaletteSaturation());
        /// <summary>
        ///  Create form PaletteValue.
        /// </summary>
        /// <returns> PalettePicker </returns>
        public static PalettePicker CreateFormValue() => new PalettePicker(new PaletteValue());
    }
}
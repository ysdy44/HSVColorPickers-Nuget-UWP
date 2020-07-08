using System.Collections.Generic;
using System;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace HSVColorPickers
{
    /// <summary>
    /// Color picker (ง •̀_•́)ง
    /// </summary>
    public sealed partial class ColorPicker : UserControl, IColorPicker
    {
        //@Delegate
        /// <summary> Occurs when the color value changed. </summary>
        public event ColorChangeHandler ColorChanged;
        /// <summary> Occurs when the color change starts. </summary>
        public event ColorChangeHandler ColorChangeStarted;
        /// <summary> Occurs when color change. </summary>
        public event ColorChangeHandler ColorChangeDelta;
        /// <summary> Occurs when the color change is complete. </summary>
        public event ColorChangeHandler ColorChangeCompleted;


        //@Group
        private EventHandler<int> Group;
        private EventHandler<Color> ChangeColor;

        /// <summary> Gets picker's type name. </summary>
        public string Type => "Color";
        /// <summary> Gets picker self. </summary>
        public Control Self => this;
        /// <summary> Gets hex picker. </summary>
        public TextBox HexPicker => this._HexPicker;


        private IEnumerable<IColorPicker> ColorPickers()
        {
            yield return this.SwatchesPicker;
            yield return this.WheelPicker;
            yield return this.RGBPicker;
            yield return this.HSVPicker;

            yield return this.PaletteHuePicker;
            yield return this.PaletteSaturationPicker;
            yield return this.PaletteValuePicker;

            yield return this.CirclePicker;
        }


        #region DependencyProperty


        /// <summary> Get or set index of the current picker. </summary>
        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set { SetValue(IndexProperty, value); }
        }
        /// <summary> Identifies the <see cref = "ColorPicker.Index" /> dependency property. </summary>
        public static readonly DependencyProperty IndexProperty = DependencyProperty.Register(nameof(Index), typeof(int), typeof(ColorPicker), new PropertyMetadata(0, (sender, e) =>
        {
            ColorPicker con = (ColorPicker)sender;

            if (e.NewValue is int value)
            {
                con.Group?.Invoke(con, value);
            }
        }));


        /// <summary> Gets or sets picker's color. </summary>
        public Color Color
        {
            get => Color.FromArgb(this.Alpha, this.R, this.G, this.B);
            set
            {
                if (value.A == this.Alpha)
                {
                    if (value.R == this.R) if (value.G == this.G) if (value.B == this.B) return;
                }
                else this.Alpha = value.A;


                Color color = Color.FromArgb(255, value.R, value.G, value.B);
                this.ChangeColor?.Invoke(this, color);//Delegate
                this._HexPicker.Color = color;

                this.R = value.R;
                this.G = value.G;
                this.B = value.B;
            }
        }

        private Color _Color
        {
            set
            {
                if (value.A == this.Alpha) if (value.R == this.R) if (value.G == this.G) if (value.B == this.B) return;

                this.R = value.R;
                this.G = value.G;
                this.B = value.B;

                Color color = Color.FromArgb(this.Alpha, value.R, value.G, value.B);
                this.ColorChanged?.Invoke(this, color);//Delegate
                this._HexPicker.Color = color;
            }
        }
        private Color _ColorStarted
        {
            set
            {
                //if (value.A == this.Alpha) if (value.R == this.R) if (value.G == this.G) if (value.B == this.B) return;

                this.R = value.R;
                this.G = value.G;
                this.B = value.B;

                Color color = Color.FromArgb(this.Alpha, value.R, value.G, value.B);
                this.ColorChangeStarted?.Invoke(this, color);//Delegate
            }
        }
        private Color _ColorDelta
        {
            set
            {
                if (value.A == this.Alpha) if (value.R == this.R) if (value.G == this.G) if (value.B == this.B) return;

                this.R = value.R;
                this.G = value.G;
                this.B = value.B;

                Color color = Color.FromArgb(this.Alpha, value.R, value.G, value.B);
                this.ColorChangeDelta?.Invoke(this, color);//Delegate
            }
        }
        private Color _ColorCompleted
        {
            set
            {
                //if (value.A == this.Alpha) if (value.R == this.R) if (value.G == this.G) if (value.B == this.B) return;

                this.R = value.R;
                this.G = value.G;
                this.B = value.B;

                Color color = Color.FromArgb(this.Alpha, value.R, value.G, value.B);
                this.ColorChangeCompleted?.Invoke(this, color);//Delegate
                this._HexPicker.Color = color;
            }
        }



        /// <summary> Gets or sets picker's alpha. </summary>
        public byte Alpha
        {
            get => this.AlphaPicker.Alpha;
            set => this.AlphaPicker.Alpha = value;
        }

        private byte _Alpha
        {
            set => this.ColorChanged?.Invoke(this, Color.FromArgb(value, this.R, this.G, this.B)); //Delegate
        }
        private byte _AlphaStarted
        {
            set => this.ColorChangeStarted?.Invoke(this, Color.FromArgb(value, this.R, this.G, this.B)); //Delegate
        }
        private byte _AlphaDelta
        {
            set => this.ColorChangeDelta?.Invoke(this, Color.FromArgb(value, this.R, this.G, this.B)); //Delegate
        }
        private byte _AlphaCompleted
        {
            set => this.ColorChangeCompleted?.Invoke(this, Color.FromArgb(value, this.R, this.G, this.B)); //Delegate
        }

        /// <summary> Gets or sets picker's red. </summary>
        private byte R = 255;
        /// <summary> Gets or sets picker's green. </summary>
        private byte G = 255;
        /// <summary> Gets or sets picker's blue. </summary>
        private byte B = 255;


        #endregion


        #region DependencyProperty


        /// <summary> Get or set the text style. </summary>
        public Style TextStyle
        {
            get { return (Style)GetValue(TextStyleProperty); }
            set { SetValue(TextStyleProperty, value); }
        }
        /// <summary> Identifies the <see cref = "ColorPicker.ButtonStyle" /> dependency property. </summary>
        public static readonly DependencyProperty TextStyleProperty = DependencyProperty.Register(nameof(TextStyle), typeof(Style), typeof(ColorPicker), new PropertyMetadata(null));


        /// <summary> Get or set the button style. </summary>
        public Style ButtonStyle
        {
            get { return (Style)GetValue(ButtonStyleProperty); }
            set { SetValue(ButtonStyleProperty, value); }
        }
        /// <summary> Identifies the <see cref = "ColorPicker.ButtonStyle" /> dependency property. </summary>
        public static readonly DependencyProperty ButtonStyleProperty = DependencyProperty.Register(nameof(ButtonStyle), typeof(Style), typeof(ColorPicker), new PropertyMetadata(null));


        /// <summary> Get or set the flyout style. </summary>
        public Style FlyoutPresenterStyle
        {
            get { return (Style)GetValue(FlyoutPresenterStyleProperty); }
            set { SetValue(FlyoutPresenterStyleProperty, value); }
        }
        /// <summary> Identifies the <see cref = "ColorPicker.FlyoutPresenterStyle" /> dependency property. </summary>
        public static readonly DependencyProperty FlyoutPresenterStyleProperty = DependencyProperty.Register(nameof(FlyoutPresenterStyle), typeof(Style), typeof(ColorPicker), new PropertyMetadata(null));


        /// <summary> Get or set the flyout placement. </summary>
        public FlyoutPlacementMode Placement
        {
            get { return (FlyoutPlacementMode)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }
        /// <summary> Identifies the <see cref = "ColorPicker.Placement" /> dependency property. </summary>
        public static readonly DependencyProperty PlacementProperty = DependencyProperty.Register(nameof(Placement), typeof(FlyoutPlacementMode), typeof(ColorPicker), new PropertyMetadata(FlyoutPlacementMode.Bottom));


        /// <summary>  Gets or sets a brush that describes the border fill of the control. </summary>
        public SolidColorBrush Stroke
        {
            get { return (SolidColorBrush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }
        /// <summary> Identifies the <see cref = "ColorPicker.Stroke" /> dependency property. </summary>
        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(nameof(Stroke), typeof(SolidColorBrush), typeof(ColorPicker), new PropertyMetadata(new SolidColorBrush(Windows.UI.Colors.Gray)));


        #endregion


        //@Construct
        /// <summary>
        /// Construct a ColorPicker.
        /// </summary>
        public ColorPicker()
        {
            this.InitializeComponent();

            //Picker
            IList<IColorPicker> colorPickers = this.ColorPickers().ToList();
         this.Loaded += (s, e) => this.ConstructColorPickers(colorPickers);
            this.ComboBox.ItemsSource = from colorPicker in colorPickers select colorPicker.Type;

            //Alpha
            this.Alpha = 255;
            this.AlphaPicker.AlphaChanged += (s, value) => this._Alpha = value;
            this.AlphaPicker.AlphaChangeStarted += (s, value) => this._Alpha = value;
            this.AlphaPicker.AlphaChangeDelta += (s, value) => this._Alpha = value;
            this.AlphaPicker.AlphaChangeCompleted += (s, value) => this._Alpha = value;

            //Hex
            this._HexPicker.Color = this.Color;
            this._HexPicker.ColorChanged += (s, color) =>
            {
                this._Color = color;
                this.ChangeColor?.Invoke(this, color);
            };
            //Straw
            this._StrawPicker.Color = this.Color;
            this._StrawPicker.ColorChanged += (s, color) =>
            {
                this._Color = color;
                this.ChangeColor?.Invoke(this, color);
            };
        }

    }

    /// <summary>
    /// Color picker (ง •̀_•́)ง
    /// </summary>
    public sealed partial class ColorPicker : UserControl
    {
        //IColorPicker
        private void ConstructColorPickers(IList<IColorPicker> colorPickers)
        {
            int index = 0;

            //Pickers
            foreach (IColorPicker colorPicker in colorPickers)
            {
                this.ConstructGroup(colorPicker, index);
                index++;
            }
        }

        //Group
        private void ConstructGroup(IColorPicker colorPicker, int index)
        {
            void group(int groupIndex)
            {
                if (groupIndex == index)
                {
                    colorPicker.Self.Visibility = Visibility.Visible;

                    colorPicker.Color = this.Color;
                }
                else
                {
                    colorPicker.Self.Visibility = Visibility.Collapsed;
                }
            }

            //NoneButton
            group(this.Index);

            //Buttons
            colorPicker.ColorChanged += (s, value) => this._Color = value;
            colorPicker.ColorChangeStarted += (s, value) => this._ColorStarted = value;
            colorPicker.ColorChangeDelta += (s, value) => this._ColorDelta = value;
            colorPicker.ColorChangeCompleted += (s, value) => this._ColorCompleted = value;

            //Change
            this.Group += (s, e) => group(e);
            this.ChangeColor += (s, color) =>
            {
                if (this.Index == index)
                {
                    colorPicker.Color = color;
                }
            };
        }

    }
}
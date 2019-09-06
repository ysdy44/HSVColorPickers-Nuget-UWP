using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace HSVColorPickers
{
    /// <summary>
    /// Color picker (ง •̀_•́)ง
    /// </summary>
    public sealed partial class ColorPicker : UserControl, IColorPicker, IAlphaPicker
    {
        //@Delegate
        /// <summary> Occurs when the color value changes. </summary>
        public event ColorChangeHandler ColorChange;
        /// <summary> Occurs when the alpha value changes. </summary>
        public event AlphaChangeHandler AlphaChange;


        /// <summary> Gets picker's type name. </summary>
        public string Type => "Color";
        /// <summary> Gets picker self. </summary>
        public UserControl Self => this;


        private IEnumerable<IColorPicker> Pickers()
        {
            yield return this.SwatchesPicker;
            yield return this.WheelPicker;
            yield return this.RGBPicker;
            yield return this.HSVPicker;

            yield return this.PaletteHuePicker;
            yield return this.PaletteSaturationPicker;
            yield return this.PaletteValuePicker;
        }


        #region Color


        /// <summary> Gets or Sets picker's color. </summary>
        public Color Color
        {
            get => Color.FromArgb(this.Alpha, this.SolidColorBrushName.Color.R, this.SolidColorBrushName.Color.G, this.SolidColorBrushName.Color.B);
            set
            {
                if (value.A != this.Alpha) this.Alpha = value.A;

                if (value.A == this.Alpha && value.R == this.SolidColorBrushName.Color.R && value.G == this.SolidColorBrushName.Color.G && value.B == this.SolidColorBrushName.Color.B)
                    return;

                Color color = Color.FromArgb(255, value.R, value.G, value.B);
                this.SetColorWithCurrentPicker(color);
                this.SolidColorBrushName.Color = color;
            }
        }

        private Color _Color
        {
            get => this.SolidColorBrushName.Color;
            set
            {
                if (value.A == this.Alpha && value.R == this.SolidColorBrushName.Color.R && value.G == this.SolidColorBrushName.Color.G && value.B == this.SolidColorBrushName.Color.B)
                    return;

                this.SolidColorBrushName.Color = Color.FromArgb(255, value.R, value.G, value.B);
                this.ColorChange?.Invoke(this, Color.FromArgb(this.Alpha, value.R, value.G, value.B));//Delegate
            }
        }
         


        /// <summary> Gets or Sets picker's alpha. </summary>
        public byte Alpha
        {
            get => this.AlphaPicker.Alpha;
            set => this.AlphaPicker.Alpha = value;
        }

        private byte _Alpha
        {
            get => this.AlphaPicker.Alpha;
            set
            {
                this.AlphaPicker.Alpha = value;
                this.AlphaChange?.Invoke(this, value);//Delegate
                this.ColorChange?.Invoke(this, Color.FromArgb(value, this.SolidColorBrushName.Color.R, this.SolidColorBrushName.Color.G, this.SolidColorBrushName.Color.B)); //Delegate
            }
        }


        #endregion


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
                con.SetVisibilityWithCurrentPicker(value);
            }
        }));


        /// <summary> Get or set the flyout style. </summary>
        public Style FlyoutPresenterStyle
        {
            get { return (Style)GetValue(FlyoutPresenterStyleProperty); }
            set { SetValue(FlyoutPresenterStyleProperty, value); }
        }
        /// <summary> Identifies the <see cref = "NumberPicker.FlyoutPresenterStyle" /> dependency property. </summary>
        public static readonly DependencyProperty FlyoutPresenterStyleProperty = DependencyProperty.Register(nameof(FlyoutPresenterStyle), typeof(Style), typeof(ColorPicker), new PropertyMetadata(null));


        /// <summary> Get or set the flyout placement. </summary>
        public FlyoutPlacementMode Placement
        {
            get { return (FlyoutPlacementMode)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }
        /// <summary> Identifies the <see cref = "NumberPicker.Placement" /> dependency property. </summary>
        public static readonly DependencyProperty PlacementProperty = DependencyProperty.Register(nameof(Placement), typeof(FlyoutPlacementMode), typeof(ColorPicker), new PropertyMetadata(FlyoutPlacementMode.Bottom));


        #endregion


        /// <summary> Get or set up display <see cref="HSVColorPickers.HexPicker"/> or <see cref="HSVColorPickers.StrawPicker"/>. </summary>
        public bool HexOrStraw
        {
            get => hexOrStraw;
            set
            {
                if (value)
                {
                    this.HexPicker.Visibility = Visibility.Visible;
                    this.StrawPicker.Visibility = Visibility.Collapsed;

                    this.HexPicker.Color = this.Color;
                }
                else
                {
                    this.HexPicker.Visibility = Visibility.Collapsed;
                    this.StrawPicker.Visibility = Visibility.Visible;
                }
                hexOrStraw = value;
            }
        }
        private bool hexOrStraw;


        //@Construct
        /// <summary>
        /// Construct a ColorPicker.
        /// </summary>
        public ColorPicker()
        {
            this.InitializeComponent();
            this.Loaded += (s, e) => this.SetVisibilityWithCurrentPicker(this.Index);

            //Picker
            this.ComboBox.ItemsSource = from picker in this.Pickers() select picker.Type;

            //Alpha
            this.Alpha = 255;
            this.AlphaPicker.AlphaChange += (s, value) => this._Alpha = value;

            //HexOrStraw
            this.HexOrStraw = false;
            this.HexOrStrawButton.Tapped += (s, e) => this.HexOrStraw = !this.HexOrStraw;
            //Hex
            this.HexPicker.Color = this.Color;
            this.HexPicker.ColorChange += (s, color) =>
            {
                this._Color = color;
                this.SetColorWithCurrentPicker(color);
            };
            //Straw
            this.StrawPicker.Color = this.Color;
            this.StrawPicker.ColorChange += (s, color) =>
            {
                this._Color = color;
                this.SetColorWithCurrentPicker(color);
            };

            //Pickers
            foreach (IColorPicker picker in this.Pickers())
            {
                picker.ColorChange += (s, value) =>
                {
                    this._Color = value;

                    if (this.HexOrStraw)
                        this.HexPicker.Color = value;
                    else
                        this.StrawPicker.Color = value;
                };
            }
        }



        private void SetColorWithCurrentPicker(Color color)
        {
            foreach (IColorPicker picker in this.Pickers())
            {
                if (picker.Self.Visibility == Visibility.Visible)
                {
                    picker.Color = color;
                }
            }
        }

        private void SetVisibilityWithCurrentPicker(int index)
        {
            foreach (IColorPicker picker in this.Pickers())
            {
                bool isSelf = index == picker.Self.TabIndex;
                if (isSelf)
                {
                    picker.Self.Visibility = Visibility.Visible;
                    picker.Color = this.SolidColorBrushName.Color;
                }
                else
                {
                    picker.Self.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
using System.Collections.Generic;
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
    public sealed partial class ColorPicker : UserControl, IColorPicker, IAlphaPicker
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
        /// <summary> Occurs when alpha change. </summary>
        public event AlphaChangeHandler AlphaChanged;
        /// <summary> Occurs when the alpha change starts. </summary>
        public event AlphaChangeHandler AlphaChangeStarted;
        /// <summary> Occurs when alpha change. </summary>
        public event AlphaChangeHandler AlphaChangeDelta;
        /// <summary> Occurs when the alpha change is complete. </summary>
        public event AlphaChangeHandler AlphaChangeCompleted;


        /// <summary> Gets picker's type name. </summary>
        public string Type => "Color";
        /// <summary> Gets picker self. </summary>
        public UserControl Self => this;


        private IEnumerable<IColorPicker> _pickers()
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


        #region Color


        /// <summary> Gets or sets picker's color. </summary>
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
                this.ColorChanged?.Invoke(this, Color.FromArgb(this.Alpha, value.R, value.G, value.B));//Delegate
            }
        }
        private Color _ColorStarted
        {
            get => this.SolidColorBrushName.Color;
            set
            {
                //if (value.A == this.Alpha && value.R == this.SolidColorBrushName.Color.R && value.G == this.SolidColorBrushName.Color.G && value.B == this.SolidColorBrushName.Color.B)
                    //return;

                this.SolidColorBrushName.Color = Color.FromArgb(255, value.R, value.G, value.B);
                this.ColorChangeStarted?.Invoke(this, Color.FromArgb(this.Alpha, value.R, value.G, value.B));//Delegate
            }
        }
        private Color _ColorDelta
        {
            get => this.SolidColorBrushName.Color;
            set
            {
                if (value.A == this.Alpha && value.R == this.SolidColorBrushName.Color.R && value.G == this.SolidColorBrushName.Color.G && value.B == this.SolidColorBrushName.Color.B)
                    return;

                this.SolidColorBrushName.Color = Color.FromArgb(255, value.R, value.G, value.B);
                this.ColorChangeDelta?.Invoke(this, Color.FromArgb(this.Alpha, value.R, value.G, value.B));//Delegate
            }
        }
        private Color _ColorCompleted
        {
            get => this.SolidColorBrushName.Color;
            set
            {
                //if (value.A == this.Alpha && value.R == this.SolidColorBrushName.Color.R && value.G == this.SolidColorBrushName.Color.G && value.B == this.SolidColorBrushName.Color.B)
                    //return;

                this.SolidColorBrushName.Color = Color.FromArgb(255, value.R, value.G, value.B);
                this.ColorChangeCompleted?.Invoke(this, Color.FromArgb(this.Alpha, value.R, value.G, value.B));//Delegate
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
            get => this.AlphaPicker.Alpha;
            set
            {
                this.AlphaPicker.Alpha = value;
                this.AlphaChanged?.Invoke(this, value);//Delegate
                this.ColorChanged?.Invoke(this, Color.FromArgb(value, this.SolidColorBrushName.Color.R, this.SolidColorBrushName.Color.G, this.SolidColorBrushName.Color.B)); //Delegate
            }
        }
        private byte _AlphaStarted
        {
            get => this.AlphaPicker.Alpha;
            set
            {
                this.AlphaPicker.Alpha = value;
                this.AlphaChangeStarted?.Invoke(this, value);//Delegate
                this.ColorChangeStarted?.Invoke(this, Color.FromArgb(value, this.SolidColorBrushName.Color.R, this.SolidColorBrushName.Color.G, this.SolidColorBrushName.Color.B)); //Delegate
            }
        }
        private byte _AlphaDelta
        {
            get => this.AlphaPicker.Alpha;
            set
            {
                this.AlphaPicker.Alpha = value;
                this.AlphaChangeDelta?.Invoke(this, value);//Delegate
                this.ColorChangeDelta?.Invoke(this, Color.FromArgb(value, this.SolidColorBrushName.Color.R, this.SolidColorBrushName.Color.G, this.SolidColorBrushName.Color.B)); //Delegate
            }
        }
        private byte _AlphaCompleted
        {
            get => this.AlphaPicker.Alpha;
            set
            {
                this.AlphaPicker.Alpha = value;
                this.AlphaChangeCompleted?.Invoke(this, value);//Delegate
                this.ColorChangeCompleted?.Invoke(this, Color.FromArgb(value, this.SolidColorBrushName.Color.R, this.SolidColorBrushName.Color.G, this.SolidColorBrushName.Color.B)); //Delegate
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
            if (con._isLoad == false) return;

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


        /// <summary>  Gets or sets a brush that describes the border fill of the control. </summary>
        public SolidColorBrush Stroke
        {
            get { return (SolidColorBrush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        /// <summary> Identifies the <see cref = "ColorPicker.Stroke" /> dependency property. </summary>
        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(nameof(Stroke), typeof(SolidColorBrush), typeof(ColorPicker), new PropertyMetadata(new SolidColorBrush(Windows.UI.Colors.Gray)));


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

        bool _isLoad;
        //@Construct
        /// <summary>
        /// Construct a ColorPicker.
        /// </summary>
        public ColorPicker()
        {
            this.InitializeComponent();
            this.Loaded += (s, e) => 
            {
                this._isLoad = true;
                this.SetVisibilityWithCurrentPicker(this.Index);
            };

            //Picker
            this.ComboBox.ItemsSource = from picker in this._pickers() select picker.Type;

            //Alpha
            this.Alpha = 255;
            this.AlphaPicker.AlphaChanged += (s, value) => this._Alpha = value;

            //HexOrStraw
            this.HexOrStraw = false;
            this.HexOrStrawButton.Click += (s, e) => this.HexOrStraw = !this.HexOrStraw;
            //Hex
            this.HexPicker.Color = this.Color;
            this.HexPicker.ColorChanged += (s, color) =>
            {
                this._Color = color;
                this.SetColorWithCurrentPicker(color);
            };
            //Straw
            this.StrawPicker.Color = this.Color;
            this.StrawPicker.ColorChanged += (s, color) =>
            {
                this._Color = color;
                this.SetColorWithCurrentPicker(color);
            };

            
            //Pickers
            foreach (IColorPicker picker in this._pickers())
            {
                picker.ColorChanged += (s, value) =>
                {
                    this._Color = value;

                    if (this.HexOrStraw) this.HexPicker.Color = value;
                    else this.StrawPicker.Color = value;
                };
                picker.ColorChangeStarted += (s, value) =>
                {
                    this._ColorStarted = value;

                    if (this.HexOrStraw) this.HexPicker.Color = value;
                    else this.StrawPicker.Color = value;
                };
                picker.ColorChangeDelta += (s, value) =>
                {
                    this._ColorDelta = value;

                    if (this.HexOrStraw) this.HexPicker.Color = value;
                    else this.StrawPicker.Color = value;
                };
                picker.ColorChangeCompleted += (s, value) =>
                {
                    this._ColorCompleted = value;

                    if (this.HexOrStraw) this.HexPicker.Color = value;
                    else this.StrawPicker.Color = value;
                };
            }
        }



        private void SetColorWithCurrentPicker(Color color)
        {
            foreach (IColorPicker picker in this._pickers())
            {
                if (picker.Self.Visibility == Visibility.Visible)
                {
                    picker.Color = color;
                }
            }
        }

        private void SetVisibilityWithCurrentPicker(int index)
        {
            foreach (IColorPicker picker in this._pickers())
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
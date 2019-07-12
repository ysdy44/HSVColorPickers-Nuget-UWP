using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Linq;

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

        /// <summary> All pickers. </summary>
        public IColorPicker[] Pickers = new IColorPicker[]
        {
            new SwatchesPicker(),
            new WheelPicker(),
            new RGBPicker(),
            new HSVPicker(),

            PalettePicker.CreateFormHue(),
            PalettePicker.CreateFormSaturation(),
            PalettePicker.CreateFormValue(),
        };


        #region Color


        /// <summary> Gets or Sets picker's color. </summary>
        public Color Color
        {
            get => Color.FromArgb(this.Alpha, this._color.R, this._color.G, this._color.B);
            set
            {
                if (value.A != this.Alpha) this.Alpha = value.A;

                if (value.A == this.Alpha && value.R == this._color.R && value.G == this._color.G && value.B == this._color.B)
                    return;

                Color color = Color.FromArgb(255, value.R, value.G, value.B);
                this.Pickers[this.Index].Color = color;

                this._color = color;
            }
        }

        private Color _Color
        {
            get => this._color;
            set
            {
                if (value.A == this.Alpha && value.R == this._color.R && value.G == this._color.G && value.B == this._color.B)
                    return;

                this._color = Color.FromArgb(255, value.R, value.G, value.B);
                this.ColorChange?.Invoke(this, Color.FromArgb(this.Alpha, value.R, value.G, value.B));//Delegate
            }
        }

        private Color _color
        {
            get => this.SolidColorBrushName.Color;
            set => this.SolidColorBrushName.Color = value;
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
                this.ColorChange?.Invoke(this, Color.FromArgb(value, this._color.R, this._color.G, this._color.B)); //Delegate
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
                if (e.OldValue is int oldValue)
                {
                    con.IndexChanged(value, oldValue);
                }
                else
                {
                    con.IndexChanged(value);
                }
            }
        }));

        private void IndexChanged(int value)
        {
            IColorPicker newPicker = this.Pickers[value];

            UserControl control = newPicker.Self;
            this.ContentBorder.Child = control;

            newPicker.Color = this._Color;
            newPicker.ColorChange += this.Picker_ColorChange;
        }

        private void IndexChanged(int newValue, int oldvalue)
        {
            IColorPicker newPicker = this.Pickers[newValue];

            if (newValue != oldvalue)
            {
                IColorPicker oldPicker = this.Pickers[oldvalue];
                oldPicker.ColorChange -= this.Picker_ColorChange;
            }

            UserControl control = newPicker.Self;
            this.ContentBorder.Child = control;

            newPicker.Color = this._Color;
            newPicker.ColorChange += this.Picker_ColorChange;
        }


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
            
            //Picker
            if (this.Index == 0) this.IndexChanged(this.Index);
            this.ComboBox.ItemsSource = from item in this.Pickers select item.Type;

            this.Loaded += (s2, e2) =>
            {
                this.ComboBox.SelectedIndex = this.Index;
                this.ComboBox.SelectionChanged += (s, e) => this.Index = this.ComboBox.SelectedIndex;
            };

            //Alpha
            this.Alpha = 255;
            this.AlphaPicker.AlphaChange += (s, value) => this._Alpha = value;

            //HexOrStraw
            this.HexOrStraw = false;
            this.HexOrStrawButton.Tapped += (s, e) => this.HexOrStraw = !this.HexOrStraw;
            //Hex
            this.HexPicker.Color = this.Color;
            this.HexPicker.ColorChange += this.Picker_ColorChange2;
            //Straw
            this.StrawPicker.Color = this.Color;
            this.StrawPicker.ColorChange += this.Picker_ColorChange2;
        }

        private void Picker_ColorChange(object sender, Color value)
        {
            this._Color = value;
            this.HexPicker.Color = value;
        }
        private void Picker_ColorChange2(object sender, Color value)
        {
            this._Color = value;
            this.Pickers[this.Index].Color = value;
        }
    }
}
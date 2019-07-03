using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HSVColorPickers
{
    /// <summary>
    /// Interface of <see cref="Picker"/>.
    /// </summary>
    public interface IPicker
    {
        event ColorChangeHandler ColorChange;
        Color Color { get; set; }
    }

    /// <summary>
    /// Represents a basic color picker.
    /// </summary>
    public class Picker
    {
        /// <summary> <see cref="Picker">'s name. </summary>
        public string Name;
        /// <summary> <see cref="Picker">'s control. </summary>
        public IPicker Control;

        //@Construct
        /// <summary>
        /// Construct a <see cref="Picker">.
        /// </summary>
        /// <param name="name"> name </param>
        /// <param name="control"> IPicker </param>
        public Picker(string name, IPicker control)
        {
            this.Name = name;
            this.Control = control;
        }
    }

    /// <summary>
    /// Color picker (ง •̀_•́)ง
    /// </summary>
    public sealed partial class ColorPicker : UserControl
    {
        //@Delegate
        /// <summary> Occurs when the color value changes. </summary>
        public event ColorChangeHandler ColorChange;


        #region Picker

        /// <summary> All pickers. </summary>
        public Picker[] Pickers = new Picker[]
        {
            new Picker( "Swatches",new SwatchesPicker()),
            new Picker( "Wheel",new WheelPicker()),
            new Picker( "RGB",new RGBPicker()),
            new Picker( "HSV",new HSVPicker()),
            new Picker( "Palette Hue",PalettePicker.CreateFormHue()),
            new Picker( "Palette Saturation",PalettePicker.CreateFormSaturation()),
            new Picker( "Palette Value",PalettePicker.CreateFormValue()),
        };

        /// <summary> Get or set index of the current picker. </summary>
        public int Index
        {
            get => this.index;
            set
            {
                IPicker newControl = this.Pickers[value].Control;

                if (value != this.index)
                {
                    IPicker oldControl = this.Pickers[this.index].Control;
                    oldControl.ColorChange -= this.Picker_ColorChange;
                }

                this.ContentControl.Content = newControl;
                newControl.Color = this._Color;

                newControl.ColorChange += this.Picker_ColorChange;

                this.index = value;
            }
        }
        private int index;


        #endregion


        #region Color


        /// <summary> Get or set current color. </summary>
        public Color Color
        {
            get => Color.FromArgb(this.Alpha, this._color.R, this._color.G, this._color.B);
            set
            {
                if (value.A != this.Alpha) this.Alpha = value.A;

                if (value.A == this.Alpha && value.R == this._color.R && value.G == this._color.G && value.B == this._color.B)
                    return;

                Color color = Color.FromArgb(255, value.R, value.G, value.B);
                this.Pickers[this.Index].Control.Color = color;

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



        /// <summary> Get or set current color aphla. </summary>
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
                this.ColorChange?.Invoke(this, Color.FromArgb(value, this._color.R, this._color.G, this._color.B)); //Delegate
            }
        }


        #endregion


        /// <summary> Get or set up display <see cref="HSVColorPickers.HexPicker"> or <see cref="HSVColorPickers.StrawPicker">. </summary>
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
        public ColorPicker()
        {
            this.InitializeComponent();

            //Picker
            this.Index = 0;
            this.ComboBox.SelectedIndex = this.Index;
            this.ComboBox.SelectionChanged += (s, e) => this.Index = this.ComboBox.SelectedIndex;

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
            this.Pickers[this.Index].Control.Color = value;
        }
    }
}
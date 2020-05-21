using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HSVColorPickers.TestApp
{
    public sealed partial class MainPage : Page
    {
        private IEnumerable<IColorPicker> Pickers()
        {
            yield return this.ColorPicker;

            yield return this.SwatchesPicker;
            yield return this.WheelPicker;
            yield return this.RGBPicker;
            yield return this.HSVPicker;

            yield return this.PaletteHuePicker;
            yield return this.PaletteSaturationPicker;
            yield return this.PaletteValuePicker;

            yield return this.CirclePicker;
        }

        private Color _Color
        {
            get => this.SolidColorBrush.Color;
            set => this.SolidColorBrush.Color = value;
        }

        #region DependencyProperty

        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set { SetValue(IndexProperty, value); }
        }
        public static readonly DependencyProperty IndexProperty = DependencyProperty.Register(nameof(Index), typeof(int), typeof(MainPage), new PropertyMetadata(0, (sender, e) =>
        {
            MainPage con = (MainPage)sender;

            if (e.NewValue is int value)
            {
                con.SetVisibilityWithCurrentPicker(value);
            }
        }));
        
        #endregion

        //@Construct
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += (s, e) => this.SetVisibilityWithCurrentPicker(this.Index);

            this.ListBox.ItemsSource = from picker in this.Pickers() select picker.Type;

            //Pickers
            foreach (IColorPicker picker in this.Pickers())
            {
                picker.ColorChanged += (s, value) =>
                {
                    this._Color = value;
                };
                picker.ColorChangeStarted += (s, value) =>
                {
                    this._Color = value;
                    this.TextBlock.Text = "ColorChangeStarted";
                };
                picker.ColorChangeDelta += (s, value) =>
                {
                    this._Color = value;
                    this.TextBlock.Text = "ColorChangeDelta";
                };
                picker.ColorChangeCompleted += (s, value) =>
                {
                    this._Color = value;
                    this.TextBlock.Text = "ColorChangeCompleted";
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
                    picker.Color = this._Color;
                }
                else
                {
                    picker.Self.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
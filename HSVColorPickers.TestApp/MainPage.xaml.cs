using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI;

namespace HSVColorPickers.TestApp
{
    public sealed partial class MainPage : Page
    {

        public IColorPicker[] Pickers = new IColorPicker[]
        {
             new ColorPicker()
             {
                 Index = 1,
                 Background = new SolidColorBrush(Colors.Transparent)
             },

             new RGBPicker(),
             new HSVPicker(),
             new WheelPicker(),

             new SwatchesPicker(),
             new HexPicker(),
             new AlphaPicker(),

             PalettePicker.CreateFormHue(),
             PalettePicker.CreateFormSaturation(),
             PalettePicker.CreateFormValue(),
        };

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
                con.IndexChanged(value);
            }
        }));

        private void IndexChanged(int value)
        {
            IColorPicker picker = this.Pickers[value];

            UserControl control = picker.Self;
            this.Border.Child = control;

            picker.Color = this.SolidColorBrush.Color;
            picker.ColorChange += (s, color) => this.SolidColorBrush.Color = color;
        }

        #endregion

        //@Construct
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += (s, e) => this.IndexChanged(0);
        }
    }
}

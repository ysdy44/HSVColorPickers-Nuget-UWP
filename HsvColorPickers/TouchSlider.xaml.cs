using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace HSVColorPickers
{
    /// <summary>
    /// Touch slider, It has three events : Started, Delta and Completed.
    /// </summary>
    public sealed partial class TouchSlider : TouchSliderBase
    {

        //@Content
        /// <summary> Get the RootGrid. </summary>
        public override Grid RootGrid => this._RootGrid;
        /// <summary> Get the left GridLength. </summary>
        public override ColumnDefinition LeftGridLength => this._LeftGridLength;
        /// <summary> Get the center GridLength. </summary>
        public override ColumnDefinition CenterGridLength => this._CenterGridLength;
        /// <summary> Get the right GridLength. </summary>
        public override ColumnDefinition RightGridLength => this._RightGridLength;
        
        /// <summary> Get or set a UIElement to provide a control background. </summary>
        public UIElement SliderBackground { get => this.Border.Child; set => this.Border.Child = value; }
        /// <summary> Get or set a brush to provide a control background. </summary>
        public Brush SliderBrush { get => this.UserControl.Background; set => this.UserControl.Background = value; }

        
        //@Construct
        /// <summary>
        /// Construct a TouchSlider.
        /// </summary>
        public TouchSlider()
        {
            this.InitializeComponent();
            base.InitializeComponent();
        }
    }
}
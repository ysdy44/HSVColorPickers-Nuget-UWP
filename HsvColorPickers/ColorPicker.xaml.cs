using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
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
        /// <summary> Occurs when the color changed starts. </summary>
        public event ColorChangeHandler ColorChangedStarted;
        /// <summary> Occurs when color changed. </summary>
        public event ColorChangeHandler ColorChangedDelta;
        /// <summary> Occurs when the color changed is complete. </summary>
        public event ColorChangeHandler ColorChangedCompleted;

        /// <summary> Occurs when the eyedropper closed. </summary>
        public event EventHandler<object> EyedropperClosed
        {
            remove => this.Eyedropper.Closed -= value;
            add => this.Eyedropper.Closed += value;
        }
        /// <summary> Occurs when the eyedropper opened. </summary>
        public event EventHandler<object> EyedropperOpened
        {
            remove => this.Eyedropper.Opened -= value;
            add => this.Eyedropper.Opened += value;
        }

        private EventHandler<Color> ChangeColor;


        /// <summary> Gets picker's type name. </summary>
        public string Type => "Color";
        /// <summary> Gets picker self. </summary>
        public Control Self => this;
        /// <summary> Gets hex picker. </summary>
        public TextBox HexPicker => this.HexPickerCore;

        private readonly Eyedropper Eyedropper = new Eyedropper();
        private readonly IEnumerable<IColorPicker> ColorPickers;
        private readonly IEnumerable<GridViewItem> Headers;


        #region DependencyProperty


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
                this.HexPickerCore.Color = color;

                this.R = value.R;
                this.G = value.G;
                this.B = value.B;
            }
        }

        private void OnColorChanged(object sender, Color value)
        {
            if (value.A == this.Alpha) if (value.R == this.R) if (value.G == this.G) if (value.B == this.B) return;

            this.R = value.R;
            this.G = value.G;
            this.B = value.B;

            Color color = Color.FromArgb(this.Alpha, value.R, value.G, value.B);
            this.ColorChanged?.Invoke(this, color);//Delegate
            this.HexPickerCore.Color = color;
        }
        private void OnColorChangedStarted(object sender, Color value)
        {
            //if (value.A == this.Alpha) if (value.R == this.R) if (value.G == this.G) if (value.B == this.B) return;

            this.R = value.R;
            this.G = value.G;
            this.B = value.B;

            Color color = Color.FromArgb(this.Alpha, value.R, value.G, value.B);
            this.ColorChangedStarted?.Invoke(this, color);//Delegate
        }
        private void OnColorChangedDelta(object sender, Color value)
        {
            //if (value.A == this.Alpha) if (value.R == this.R) if (value.G == this.G) if (value.B == this.B) return;

            this.R = value.R;
            this.G = value.G;
            this.B = value.B;

            Color color = Color.FromArgb(this.Alpha, value.R, value.G, value.B);
            this.ColorChangedDelta?.Invoke(this, color);//Delegate
        }
        private void OnColorChangedCompleted(object sender, Color value)
        {
            //if (value.A == this.Alpha) if (value.R == this.R) if (value.G == this.G) if (value.B == this.B) return;

            this.R = value.R;
            this.G = value.G;
            this.B = value.B;

            Color color = Color.FromArgb(this.Alpha, value.R, value.G, value.B);
            this.ColorChangedCompleted?.Invoke(this, color);//Delegate
            this.HexPickerCore.Color = color;
        }



        /// <summary> Gets or sets picker's alpha. </summary>
        public byte Alpha
        {
            get => this.AlphaPicker.Alpha;
            set => this.AlphaPicker.Alpha = value;
        }

        private void OnAlphaChanged(object sender, byte value) => this.ColorChanged?.Invoke(this, Color.FromArgb(value, this.R, this.G, this.B)); //Delegate  
        private void OnAlphaChangedStarted(object sender, byte value) => this.ColorChangedStarted?.Invoke(this, Color.FromArgb(value, this.R, this.G, this.B)); //Delegate
        private void OnAlphaChangedDelta(object sender, byte value) => this.ColorChangedDelta?.Invoke(this, Color.FromArgb(value, this.R, this.G, this.B)); //Delegate
        private void OnAlphaChangedCompleted(object sender, byte value) => this.ColorChangedCompleted?.Invoke(this, Color.FromArgb(value, this.R, this.G, this.B)); //Delegate


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
            get => (Style)base.GetValue(TextStyleProperty);
            set => base.SetValue(TextStyleProperty, value);
        }
        /// <summary> Identifies the <see cref = "ColorPicker.ButtonStyle" /> dependency property. </summary>
        public static readonly DependencyProperty TextStyleProperty = DependencyProperty.Register(nameof(TextStyle), typeof(Style), typeof(ColorPicker), new PropertyMetadata(null));


        /// <summary> Get or set the button style. </summary>
        public Style ButtonStyle
        {
            get => (Style)base.GetValue(ButtonStyleProperty);
            set => base.SetValue(TextStyleProperty, value);
        }
        /// <summary> Identifies the <see cref = "ColorPicker.ButtonStyle" /> dependency property. </summary>
        public static readonly DependencyProperty ButtonStyleProperty = DependencyProperty.Register(nameof(ButtonStyle), typeof(Style), typeof(ColorPicker), new PropertyMetadata(null));


        /// <summary> Get or set the flyout style. </summary>
        public Style FlyoutPresenterStyle
        {
            get => (Style)base.GetValue(FlyoutPresenterStyleProperty);
            set => base.SetValue(FlyoutPresenterStyleProperty, value);
        }
        /// <summary> Identifies the <see cref = "ColorPicker.FlyoutPresenterStyle" /> dependency property. </summary>
        public static readonly DependencyProperty FlyoutPresenterStyleProperty = DependencyProperty.Register(nameof(FlyoutPresenterStyle), typeof(Style), typeof(ColorPicker), new PropertyMetadata(null));


        /// <summary> Get or set the flyout placement. </summary>
        public FlyoutPlacementMode Placement
        {
            get => (FlyoutPlacementMode)base.GetValue(PlacementProperty);
            set => base.SetValue(FlyoutPresenterStyleProperty, value);
        }
        /// <summary> Identifies the <see cref = "ColorPicker.Placement" /> dependency property. </summary>
        public static readonly DependencyProperty PlacementProperty = DependencyProperty.Register(nameof(Placement), typeof(FlyoutPlacementMode), typeof(ColorPicker), new PropertyMetadata(FlyoutPlacementMode.Bottom));


        /// <summary>  Gets or sets a brush that describes the border fill of the control. </summary>
        public SolidColorBrush Stroke
        {
            get => (SolidColorBrush)base.GetValue(StrokeProperty);
            set => base.SetValue(StrokeProperty, value);
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
            this.ColorPickers = from child in this.BodyGrid.Children where child is IColorPicker select child as IColorPicker;
            this.Headers = from child in this.SegmentedGrid.Children where child is GridViewItem select child as GridViewItem;
            this.ConstructColorPickers();
            this.Loaded += this.ColorPicker_Loaded;

            //Alpha
            this.Alpha = 255;
            this.AlphaPicker.AlphaChanged += this.OnAlphaChanged;
            this.AlphaPicker.AlphaChangeStarted += this.OnAlphaChangedStarted;
            this.AlphaPicker.AlphaChangeDelta += this.OnAlphaChangedDelta;
            this.AlphaPicker.AlphaChangeCompleted += this.OnAlphaChangedCompleted;

            //Hex
            this.HexPickerCore.Color = this.Color;
            this.HexPickerCore.ColorChanged += (s, color) =>
            {
                this.OnColorChanged(this, color);
                this.ChangeColor?.Invoke(this, color);
            };

            //Eyedropper
            this.EyedropperButton.Click += async (s, e) =>
            {
                Color color = await this.Eyedropper.OpenAsync(this.EyedropperButton);
                this.OnColorChanged(this, color);
                this.ChangeColor?.Invoke(this, color);
            };
            this.EyedropperClosed += (s, e) =>
            {
                this.EyedropperButton.IsEnabled = true;
                Window.Current.CoreWindow.KeyDown -= CoreWindow_KeyDown;
            };
            this.EyedropperOpened += (s, e) =>
            {
                this.EyedropperButton.IsEnabled = false;
                Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            };
        }


        //IColorPicker
        private void ConstructColorPickers()
        {
            foreach (IColorPicker colorPicker2 in this.ColorPickers)
            {
                this.ChangeColor += (s, color) =>
                {
                    if (colorPicker2.Self.Visibility == Visibility.Visible)
                    {
                        colorPicker2.Color = color;
                    }
                };
            }

            foreach (GridViewItem header in this.Headers)
            {
                header.Tapped += (s, e) => this.ItemClick(header);
            }
        }

        private void ColorPicker_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= this.ColorPicker_Loaded;
            this.ItemClick(this.Circle);
        }

        private void ItemClick(GridViewItem header)
        {
            string type = header.Name;

            foreach (GridViewItem item2 in this.Headers)
            {
                item2.IsSelected = item2.Name == type;
            }

            foreach (IColorPicker colorPicker in this.ColorPickers)
            {
                if (colorPicker.Type == type)
                {
                    colorPicker.ColorChanged += this.OnColorChanged;
                    colorPicker.ColorChangedStarted += this.OnColorChangedStarted;
                    colorPicker.ColorChangedDelta += this.OnColorChangedDelta;
                    colorPicker.ColorChangedCompleted += this.OnColorChangedCompleted;

                    colorPicker.Self.Visibility = Visibility.Visible;
                    colorPicker.Color = this.Color;
                }
                else
                {
                    colorPicker.ColorChanged -= this.OnColorChanged;
                    colorPicker.ColorChangedStarted -= this.OnColorChangedStarted;
                    colorPicker.ColorChangedDelta -= this.OnColorChangedDelta;
                    colorPicker.ColorChangedCompleted -= this.OnColorChangedCompleted;

                    colorPicker.Self.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case VirtualKey.Left:
                    {
                        Vector2 position = this.Eyedropper.Postion;
                        position.X -= 2;
                        this.Eyedropper.Postion = position;
                    }
                    break;
                case VirtualKey.Up:
                    {
                        Vector2 position = this.Eyedropper.Postion;
                        position.Y -= 2;
                        this.Eyedropper.Postion = position;
                    }
                    break;
                case VirtualKey.Right:
                    {
                        Vector2 position = this.Eyedropper.Postion;
                        position.X += 2;
                        this.Eyedropper.Postion = position;
                    }
                    break;
                case VirtualKey.Down:
                    {
                        Vector2 position = this.Eyedropper.Postion;
                        position.Y += 2;
                        this.Eyedropper.Postion = position;
                    }
                    break;

                case VirtualKey.Enter:
                    {
                        this.Eyedropper.Close();
                    }
                    break;
                case VirtualKey.Escape:
                    {
                        this.Eyedropper.Close();
                        Color color = this.Eyedropper.Color;
                        this.OnColorChanged(this, color);
                        this.ChangeColor?.Invoke(this, color);
                    }
                    break;

                default:
                    break;
            }
        }

    }
}
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace HSVColorPickers
{
    /// <summary>
    /// Provide a numeric keypad to enter numbers.
    /// </summary>
    public sealed partial class NumberPicker : UserControl
    {
        //@Delegate
        /// <summary> Occurs when the value changed. </summary>
        public event ValueChangeHandler ValueChanged = null;


        #region DependencyProperty


        /// <summary> Get or set the current value for a number picker. </summary>
        public int Value
        {
            get
            {
                int value = (int)GetValue(ValueProperty);

                if (this.IsFlyoutOpened == false)
                {
                    if (value < this.Minimum) return this.Minimum;
                    if (value > this.Maximum) return this.Maximum;
                }

                return value;
            }
            set
            {
                if (this.IsFlyoutOpened == false)
                {
                    if (value < this.Minimum) value = this.Minimum;
                    if (value > this.Maximum) value = this.Maximum;
                }

                SetValue(ValueProperty, value);
            }
        }
        /// <summary> Identifies the <see cref = "NumberPicker.Value" /> dependency property. </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(int), typeof(NumberPicker), new PropertyMetadata(0));


        /// <summary> Get or set the minimum desirable Value for range elements. </summary>
        public int Minimum
        {
            get => (int)base.GetValue(MinimumProperty);
            set => base.SetValue(MinimumProperty, value);
        }
        /// <summary> Identifies the <see cref = "NumberPicker.Minimum" /> dependency property. </summary>
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(nameof(Minimum), typeof(int), typeof(NumberPicker), new PropertyMetadata(0));


        /// <summary> Get or set the maximum desirable Value for range elements. </summary>
        public int Maximum
        {
            get => (int)base.GetValue(MaximumProperty);
            set => base.SetValue(MaximumProperty, value);
        }
        /// <summary> Identifies the <see cref = "NumberPicker.Maximum" /> dependency property. </summary>
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(nameof(Maximum), typeof(int), typeof(NumberPicker), new PropertyMetadata(100));


        /// <summary> Get or set the string Unit for range elements. </summary>
        public string Unit
        {
            get => (string)base.GetValue(UnitProperty);
            set => base.SetValue(UnitProperty, value);
        }
        /// <summary> Identifies the <see cref = "NumberPicker.Unit" /> dependency property. </summary>
        public static readonly DependencyProperty UnitProperty = DependencyProperty.Register(nameof(Unit), typeof(string), typeof(NumberPicker), new PropertyMetadata(null));


        /// <summary> Get or set the button style. </summary>
        public Style ButtonStyle
        {
            get => (Style)base.GetValue(ButtonStyleProperty);
            set => base.SetValue(ButtonStyleProperty, value);
        }
        /// <summary> Identifies the <see cref = "NumberPicker.ButtonStyleProperty" /> dependency property. </summary>
        public static readonly DependencyProperty ButtonStyleProperty = DependencyProperty.Register(nameof(ButtonStyleProperty), typeof(Style), typeof(NumberPicker), new PropertyMetadata(null));


        /// <summary> Get or set the flyout style. </summary>
        public Style FlyoutPresenterStyle
        {
            get => (Style)base.GetValue(FlyoutPresenterStyleProperty);
            set => base.SetValue(FlyoutPresenterStyleProperty, value);
        }
        /// <summary> Identifies the <see cref = "NumberPicker.FlyoutPresenterStyle" /> dependency property. </summary>
        public static readonly DependencyProperty FlyoutPresenterStyleProperty = DependencyProperty.Register(nameof(FlyoutPresenterStyle), typeof(Style), typeof(NumberPicker), new PropertyMetadata(null));


        /// <summary> Get or set the flyout placement. </summary>
        public FlyoutPlacementMode Placement
        {
            get => (FlyoutPlacementMode)base.GetValue(PlacementProperty);
            set => base.SetValue(FlyoutPresenterStyleProperty, value);
        }
        /// <summary> Identifies the <see cref = "NumberPicker.Placement" /> dependency property. </summary>
        public static readonly DependencyProperty PlacementProperty = DependencyProperty.Register(nameof(Placement), typeof(FlyoutPlacementMode), typeof(NumberPicker), new PropertyMetadata(FlyoutPlacementMode.Bottom));


        #endregion


        /// <summary> The Flyout is opened? </summary>
        bool IsFlyoutOpened;
        /// <summary> Is this button the first one to be clicked? </summary>
        bool IsFirstClickedButton;
        /// <summary> Indicates the positive and negative of <see cref = "CacheValue" />. </summary>
        bool IsNegative;
        /// <summary> Temporary values. </summary>
        int CacheValue;


        //@Construct
        /// <summary>
        /// Construct a NumberPicker.
        /// </summary>
        public NumberPicker()
        {
            this.InitializeComponent();
            this.Button.Click += (s, e) =>
            {
                //FirstClicked: Reset the boolean
                this.IsFirstClickedButton = true;

                this.IsNegative = (this.Value < 0);
                this.CacheValue = 0;
            };

            //Flyout
            this.Flyout.Opened += (s, e) => this.IsFlyoutOpened = true;
            this.Flyout.Closed += (s, e) =>
            {
                if (this.IsFlyoutOpened)
                {
                    // The Value will be reset, when the user clicks on the blank and then Flyout closed.
                    this.SetCacheValue(this.Value);

                    this.IsFlyoutOpened = false;
                }
            };

            //Number
            this.Zero.Click += (s, e) => this.SetCacheValue(this.CacheValue * 10);
            this.One.Click += (s, e) => this.SetCacheValue(this.CacheValue * 10 + 1);
            this.Two.Click += (s, e) => this.SetCacheValue(this.CacheValue * 10 + 2);
            this.Three.Click += (s, e) => this.SetCacheValue(this.CacheValue * 10 + 3);
            this.Four.Click += (s, e) => this.SetCacheValue(this.CacheValue * 10 + 4);
            this.Five.Click += (s, e) => this.SetCacheValue(this.CacheValue * 10 + 5);
            this.Six.Click += (s, e) => this.SetCacheValue(this.CacheValue * 10 + 6);
            this.Seven.Click += (s, e) => this.SetCacheValue(this.CacheValue * 10 + 7);
            this.Eight.Click += (s, e) => this.SetCacheValue(this.CacheValue * 10 + 8);
            this.Nine.Click += (s, e) => this.SetCacheValue(this.CacheValue * 10 + 9);
            this.Decimal.Click += (s, e) => this.SetCacheValue(0);

            //Back, Negative
            this.Back.Click += (s, e) =>
            {
                //FirstClicked
                if (this.IsFirstClickedButton) this.CacheValue = Math.Abs(this.Value);

                this.SetCacheValue(this.CacheValue / 10);
            };
            this.Negative.Click += (s, e) =>
            {
                this.IsNegative = !this.IsNegative;

                //FirstClicked
                if (this.IsFirstClickedButton) this.CacheValue = Math.Abs(this.Value);

                this.SetCacheValue(this.CacheValue);
            };

            //OK, Cancel
            this.Cancel.Click += (s, e) => this.Flyout.Hide();
            this.OK.Click += (s, e) =>
            {
                this.IsFlyoutOpened = false;
                this.Flyout.Hide();

                //FirstClicked
                if (this.IsFirstClickedButton) return;

                if (this.IsNegative)
                    this.Value = -this.CacheValue;
                else
                    this.Value = this.CacheValue;

                this.ValueChanged?.Invoke(this, this.Value); //Delegate
            };
        }

        private void SetCacheValue(int cacheValue)
        {
            this.CacheValue = cacheValue;

            if (this.IsNegative)
                this.Value = -Math.Abs(cacheValue);
            else
                this.Value = Math.Abs(cacheValue);

            //FirstClicked
            this.IsFirstClickedButton = false;
        }
    }
}
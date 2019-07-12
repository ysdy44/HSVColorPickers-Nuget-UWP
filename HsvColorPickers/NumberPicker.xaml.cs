using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HSVColorPickers
{
    /// <summary>
    /// Provide a numeric keypad to enter numbers.
    /// </summary>
    public sealed partial class NumberPicker : UserControl
    {
        //@Delegate
        /// <summary> Occurs when the value changes. </summary>
        public event ValueChangeHandler ValueChange = null;


        #region DependencyProperty


        /// <summary> Get or set the current value for a number picker. </summary>
        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        /// <summary> Identifies the <see cref = "NumberPicker.Value" /> dependency property. </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(int), typeof(NumberPicker), new PropertyMetadata(0, new PropertyChangedCallback((sender, e) =>
        {
            NumberPicker con = (NumberPicker)sender;

            if (e.NewValue is int value)
            {
                if (value > con.Maximum) value = con.Maximum;
                if (value < con.Minimum) value = con.Minimum;

                con.Button.Content = con.GetContent(value);
            }
        })));


        /// <summary> Get or set the minimum desirable Value for range elements. </summary>
        public int Minimum
        {
            get { return (int)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }
        /// <summary> Identifies the <see cref = "NumberPicker.Minimum" /> dependency property. </summary>
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(nameof(Minimum), typeof(int), typeof(NumberPicker), new PropertyMetadata(0));


        /// <summary> Get or set the maximum desirable Value for range elements. </summary>
        public int Maximum
        {
            get { return (int)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }
        /// <summary> Identifies the <see cref = "NumberPicker.Maximum" /> dependency property. </summary>
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(nameof(Maximum), typeof(int), typeof(NumberPicker), new PropertyMetadata(100));


        /// <summary> Get or set the string Unit for range elements. </summary>
        public string Unit
        {
            get { return (string)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }
        /// <summary> Identifies the <see cref = "NumberPicker.Unit" /> dependency property. </summary>
        public static readonly DependencyProperty UnitProperty = DependencyProperty.Register(nameof(Unit), typeof(string), typeof(NumberPicker), new PropertyMetadata(string.Empty, new PropertyChangedCallback((sender, e) =>
        {
            NumberPicker con = (NumberPicker)sender;

            if (e.NewValue is string value)
            {
                con.GetContent(value);
            }
        })));


        #endregion


        /// <summary> The Flyout is Flyouted? </summary>
        bool IsFlyoutClosed;
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
            this.Loaded += (s, e) => this.Button.Content = this.Value.ToString() + " " + this.Unit;
            this.Button.Click += (s, e) =>
            {
                //FirstClicked: Reset the boolean
                this.IsFirstClickedButton = true;

                this.IsNegative = (this.Value < 0);
                this.CacheValue = 0;
            };

            //Flyout
            this.Flyout.Opened += (s, e) => this.IsFlyoutClosed = true;
            this.Flyout.Closed += (s, e) =>
            {
                if (this.IsFlyoutClosed)
                {
                    // The Value will be reset, when the user clicks on the blank and then Flyout closed.
                    this.SetCacheValue(this.Value);

                    this.IsFlyoutClosed = false;
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
                this.IsFlyoutClosed = false;
                this.Flyout.Hide();

                //FirstClicked
                if (this.IsFirstClickedButton) return;

                if (this.IsNegative)
                    this.Value = -this.CacheValue;
                else
                    this.Value = this.CacheValue;

                this.ValueChange?.Invoke(this, this.Value); //Delegate
            };
        }

        private string GetContent(int value) => value.ToString() + " " + this.Unit;
        private string GetContent(bool isNegative, int value) => (isNegative ? "-" : "") + Math.Abs(value).ToString() + " " + this.Unit;
        private string GetContent(string unit) => (this.IsNegative ? "-" : "") + this.Value.ToString() + " " + unit;

        private void SetCacheValue(int cacheValue)
        {
            this.CacheValue = cacheValue;
            this.Button.Content = this.GetContent(this.IsNegative, this.CacheValue);

            //FirstClicked
            this.IsFirstClickedButton = false;
        }
    }
}
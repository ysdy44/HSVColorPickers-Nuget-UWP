using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HSVColorPickers
{
    public sealed partial class NumberPicker : UserControl
    {

        //Delegate
        public delegate void ValueChangeHandler(object sender, int value);
        public event ValueChangeHandler ValueChange = null;


        #region DependencyProperty


        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(int), typeof(NumberPicker), new PropertyMetadata(0, new PropertyChangedCallback((sender, e) =>
        {
            NumberPicker con = (NumberPicker)sender;

            if (e.NewValue is int value)
            {
                int limite = con.GeLimitedtValue(value);
                con.SetValue(limite);
            }
        })));


        public int Minimum
        {
            get { return (int)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(nameof(Minimum), typeof(int), typeof(NumberPicker), new PropertyMetadata(0));


        public int Maximum
        {
            get { return (int)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(nameof(Maximum), typeof(int), typeof(NumberPicker), new PropertyMetadata(100));


        public string Unit
        {
            get { return (string)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }
        public static readonly DependencyProperty UnitProperty = DependencyProperty.Register(nameof(Unit), typeof(string), typeof(NumberPicker), new PropertyMetadata(string.Empty, new PropertyChangedCallback((sender, e) =>
        {
            NumberPicker con = (NumberPicker)sender;

            if (e.NewValue is string value)
            {
                con.SetValue(value);
            }
        })));


        #endregion

        //Flyout
        private bool IsFlyouted;
        private bool IsNegative;

        private int cacheValue;
        private int CacheValue
        {
            get => this.cacheValue;
            set
            {
                this.SetValue(this.IsNegative, value);

                this.cacheValue = value;
            }
        }


        public NumberPicker()
        {
            this.InitializeComponent();
            this.Loaded += (s, e) => this.Button.Content = this.Value.ToString() + " " + this.Unit;
            this.Button.Click += (s, e) =>
            {
                this.cacheValue = Math.Abs(this.Value);
                this.IsNegative = (this.Value < 0);
            };

            //Flyout
            this.Flyout.Opened += (s, e) =>
            {
                this.cacheValue = 0;
                this.IsFlyouted = true;
            };
            this.Flyout.Closed += (s, e) =>
            {
                if (this.IsFlyouted)
                {
                    this.CacheValue = this.Value;
                    this.IsFlyouted = false;
                }
            };

            //Number
            this.Zero.Click += (s, e) => this.CacheValue = this.cacheValue * 10;
            this.One.Click += (s, e) => this.CacheValue = this.cacheValue * 10 + 1;
            this.Two.Click += (s, e) => this.CacheValue = this.cacheValue * 10 + 2;
            this.Three.Click += (s, e) => this.CacheValue = this.cacheValue * 10 + 3;
            this.Four.Click += (s, e) => this.CacheValue = this.cacheValue * 10 + 4;
            this.Five.Click += (s, e) => this.CacheValue = this.cacheValue * 10 + 5;
            this.Six.Click += (s, e) => this.CacheValue = this.cacheValue * 10 + 6;
            this.Seven.Click += (s, e) => this.CacheValue = this.cacheValue * 10 + 7;
            this.Eight.Click += (s, e) => this.CacheValue = this.cacheValue * 10 + 8;
            this.Nine.Click += (s, e) => this.CacheValue = this.cacheValue * 10 + 9;

            //Back, Negative
            this.Back.Click += (s, e) => this.CacheValue = this.cacheValue / 10;
            this.Negative.Click += (s, e) => this.IsNegative = !this.IsNegative;
            this.Decimal.Click += (s, e) => this.CacheValue = 0;

            //OK, Cancel
            this.Cancel.Click += (s, e) => this.Flyout.Hide();
            this.OK.Click += (s, e) =>
            {
                this.IsFlyouted = false;
                this.Flyout.Hide();

                int value = this.GeLimitedtValue(this.cacheValue);
                this.Value = value;
                this.ValueChange?.Invoke(this, value);
            };
        }

        private void SetValue(int value) => this.Button.Content = value.ToString() + " " + this.Unit;
        private void SetValue(bool isNegative, int value) => this.Button.Content = (isNegative ? "-" : "") + Math.Abs(value).ToString() + " " + this.Unit;
        private void SetValue(string unit) => this.Button.Content = (this.IsNegative ? "-" : "") + this.Value.ToString() + " " + unit;

        private int GeLimitedtValue(int num)
        {
            num = Math.Abs(num);
            num = this.IsNegative ? -num : num;

            if (num > this.Maximum) return this.Maximum;
            if (num < this.Minimum) return this.Minimum;

            return num;
        }
    }
}

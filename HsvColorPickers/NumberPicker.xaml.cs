using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HsvColorPickers
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
                con.Button.Content = value.ToString() + " " + con.Unit;
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
                con.Button.Content = (con.IsNegative ? "-" : "") + con.Value.ToString() + " " + value;
            }
        })));


        #endregion

        #region Property


        private int OldValue { get; set; }

        private int newValue;
        private int NewValue
        {
            get => this.newValue;
            set
            {
                this.Button.Content = (this.IsNegative ? "-" : "") + value.ToString() + " " + this.Unit;
                this.newValue = value;
            }
        }

        private bool isNegative;
        private bool IsNegative
        {
            get => this.isNegative;
            set
            {
                this.Button.Content = (value ? "-" : "") + this.Value.ToString() + " " + this.Unit;
                this.isNegative = value;
            }
        }


        #endregion


        public NumberPicker()
        {
            this.InitializeComponent();
            this.Loaded += (s, e) => this.Button.Content = this.Value.ToString() + " " + this.Unit;

            //Number
            this.Zero.Click += (s, e) => this.NewValue = this.NewValue * 10;
            this.One.Click += (s, e) => this.NewValue = this.NewValue * 10 + 1;
            this.Two.Click += (s, e) => this.NewValue = this.NewValue * 10 + 2;
            this.Three.Click += (s, e) => this.NewValue = this.NewValue * 10 + 3;
            this.Four.Click += (s, e) => this.NewValue = this.NewValue * 10 + 4;
            this.Five.Click += (s, e) => this.NewValue = this.NewValue * 10 + 5;
            this.Six.Click += (s, e) => this.NewValue = this.NewValue * 10 + 6;
            this.Seven.Click += (s, e) => this.NewValue = this.NewValue * 10 + 7;
            this.Eight.Click += (s, e) => this.NewValue = this.NewValue * 10 + 8;
            this.Nine.Click += (s, e) => this.NewValue = this.NewValue * 10 + 9;

            //Back, Negative
            this.Back.Click += (s, e) => this.NewValue = this.NewValue / 10;
            this.Negative.Click += (s, e) => this.IsNegative = !this.IsNegative;
            this.Decimal.Click += (s, e) => this.NewValue = 0;

            //OK, Cancel
            this.OK.Click += (s, e) =>
            {
                this.Value = this.GetValue(this.NewValue);
                this.ValueChange?.Invoke(this, this.Value);
            };
            this.Cancel.Click += (s, e) =>
            {
                this.Value = this.GetValue(this.OldValue);
                this.ValueChange?.Invoke(this, this.Value);
            };

            this.Button.Click += (s, e) =>
            {
                //cache value
                if (this.Value >= 0)
                {
                    this.NewValue = this.Value;
                    this.OldValue = this.Value;
                    this.IsNegative = false;
                }
                else
                {
                    this.NewValue = -this.Value;
                    this.OldValue = -this.Value;
                    this.IsNegative = true;
                }
            };
        }

        private int GetValue(int num)
        {
            this.Flyout.Hide();

            num = Math.Abs(num);
            int value = this.IsNegative ? -num : num;
            if (value > this.Maximum) return this.Maximum;
            if (value < this.Minimum) return this.Minimum;

            return value;
        }

    }
}

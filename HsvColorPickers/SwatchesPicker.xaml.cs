using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace HSVColorPickers
{
    /// <summary>
    /// Swatches picker
    /// </summary>
    public sealed partial class SwatchesPicker : UserControl, IColorPicker
    {
        /// <summary> 
        /// It contains 16 colors. 
        /// </summary>
        private sealed class Swatches
        {
            public Color Color;
            public Color[] Colors;

            public Swatches(Color color, bool isGray = false, int count = 16)
            {
                this.Color = color;
                this.Colors = isGray ? this.GetGrayColors(count) : this.GetColorfulColors(color, count);
            }

            private Color[] GetGrayColors(int count)
            {
                Color[] colors = new Color[count];

                double span = 255 / count;

                for (int i = 0; i < count; i++)
                {
                    byte c = (byte)(255 - i * span);
                    colors[i] = Color.FromArgb(255, c, c, c);
                }

                return colors;
            }

            private Color[] GetColorfulColors(Color color, int count)
            {
                Color[] colors = new Color[count];

                float h = HSV.RGBtoHSV(color).H;
                float span = 100 / count;

                for (int i = 0; i < count; i++)
                {
                    float l = 100 - i * span;
                    float s = i % 4 * 20 + 20;
                    colors[i] = HSV.HSVtoRGB(255, h, s, l);
                }

                return colors;
            }
        }

        /// <summary>
        /// Size. 
        /// </summary>
        private sealed class RainbowSize
        {
            public readonly float Span = 4;
            public readonly float thiscikneee = 1;

            //ItemBackground
            public float ItemBackgroundX => this.Span;
            public float ItemBackgroundY => this.Span;
            public float ItemBackgroundWidth;
            public float ItemBackgroundHeight;
            //Item
            public float ItemX(int index) => this.Span + this.thiscikneee + index * this.ItemWidth;
            public float ItemY => this.Span + this.thiscikneee;
            public float ItemWidth;
            public float ItemHeight;

            //Current
            public float CurrentX(int index) => this.thiscikneee + index * this.ItemWidth;
            public float CurrentY => this.thiscikneee;
            public float CurrentWidth;
            public float CurrentHeight;
            // CurrentBackground
            public float CurrentBackgroundX(int i) => i * this.ItemWidth;
            public float CurrentBackgroundY = 0;
            public float CurrentBackgroundWidth;
            public float CurrentBackgroundHeight;

            public int Index(float x) => (int)((x - this.Span - this.thiscikneee) / this.ItemWidth);

            public void Change(float width, float height, int count)
            {
                this.ItemBackgroundWidth = width - this.Span - this.Span;
                this.ItemBackgroundHeight = height - this.Span - this.Span;

                this.ItemWidth = (this.ItemBackgroundWidth - this.thiscikneee - this.thiscikneee) / count;
                this.ItemHeight = (this.ItemBackgroundHeight - this.thiscikneee - this.thiscikneee);

                this.CurrentWidth = this.ItemWidth + this.Span + this.Span;
                this.CurrentHeight = height - this.thiscikneee - this.thiscikneee;

                this.CurrentBackgroundWidth = this.CurrentWidth + this.thiscikneee + this.thiscikneee;
                this.CurrentBackgroundHeight = height;
            }
        }


        //@Delegate
        /// <summary> Occurs when the color value changes. </summary>
        public event ColorChangeHandler ColorChange = null;


        /// <summary> Gets picker's type name. </summary>
        public string Type => "Swatches";
        /// <summary> Gets picker self. </summary>
        public UserControl Self => this;

        /// <summary> Gets or Sets picker's color. </summary>
        public Color Color { get; set; } = Color.FromArgb(255, 255, 255, 255);


        double CurrentX;
        RainbowSize Size = new RainbowSize();// Size

        // Set 16 GridViewItem's Color
        SolidColorBrush[] Brushs;

        int Count;// Rainbows count
        /// <summary> Get or set index of the current swatches. </summary>
        public int Index
        {
            get => this.index;
            set
            {
                value %= this.Count;
                value += this.Count;
                value %= this.Count;

                if (value != this.index)
                {
                    Color[] colors = this.Collection[value].Colors;
                    this.SetBrushs(colors);
                }

                this.index = value;
            }
        }
        private int index;
        Swatches[] Collection = new Swatches[]// Rainbows
        {
            new Swatches(Color.FromArgb(255,0,0,0),true),
            new Swatches(Color.FromArgb(255,192,0,0)),
            new Swatches(Color.FromArgb(255,255,0,0)),
            new Swatches(Color.FromArgb(255,254,68,1)),
            new Swatches(Color.FromArgb(255,255,192,0)),
            new Swatches(Color.FromArgb(255,255,255,0)),
            new Swatches(Color.FromArgb(255,146,208,80)),
            new Swatches(Color.FromArgb(255,86,197,1)),
            new Swatches(Color.FromArgb(255,0,176,80)),
            new Swatches(Color.FromArgb(255,6,192,197)),
            new Swatches(Color.FromArgb(255,0,176,240)),
            new Swatches(Color.FromArgb(255,0,112,192)),
            new Swatches(Color.FromArgb(255,0,32,96)),
            new Swatches(Color.FromArgb(255,112,48,160)),
            new Swatches(Color.FromArgb(255,255,64,196)),
            new Swatches(Color.FromArgb(255,254,14,111)),
        };


        //@Construct
        /// <summary>
        /// Construct a SwatchesPicker.
        /// </summary>
        public SwatchesPicker()
        {
            this.InitializeComponent();
            this.Count = this.Collection.Count();
            this.Index = 0;

            //Brush
            Color[] colors = this.Collection[this.Index].Colors;
            this.Brushs = this.GetBrushs(colors);
            this.GridView.ItemsSource = this.GetItems(this.Brushs);

            //Manipulation
            this.CanvasControl.ManipulationMode = ManipulationModes.TranslateX;
            this.CanvasControl.ManipulationStarted += (s, e) => this.CurrentX = e.Position.X;
            this.CanvasControl.ManipulationDelta += (s, e) =>
            {
                this.CurrentX += e.Delta.Translation.X;
                this.Index = this.Size.Index((float)this.CurrentX);
                this.CanvasControl.Invalidate();
            };

            //Draw
            this.CanvasControl.SizeChanged += (s, e) => this.Size.Change((float)e.NewSize.Width, (float)e.NewSize.Height, this.Count);
            this.CanvasControl.Draw += (sender, args) =>
            {
                args.DrawingSession.FillRectangle(this.Size.ItemBackgroundX, this.Size.ItemBackgroundY, this.Size.ItemBackgroundWidth, this.Size.ItemBackgroundHeight, Windows.UI.Colors.Gray);
                for (int i = 0; i < this.Count; i++)
                {
                    args.DrawingSession.FillRectangle(this.Size.ItemX(i), this.Size.ItemY, this.Size.ItemWidth, this.Size.ItemHeight, Collection[i].Color);
                }

                Swatches current = this.Collection[this.Index];
                args.DrawingSession.FillRectangle(this.Size.CurrentBackgroundX(this.Index), this.Size.CurrentBackgroundY, this.Size.CurrentBackgroundWidth, this.Size.CurrentBackgroundHeight, Windows.UI.Colors.Gray);
                args.DrawingSession.FillRectangle(this.Size.CurrentX(this.Index), this.Size.CurrentY, this.Size.CurrentWidth, this.Size.CurrentHeight, current.Color);
            };

            //Wheel
            this.CanvasControl.PointerWheelChanged += (s, e) =>
            {
                if (e.GetCurrentPoint(this.CanvasControl).Properties.MouseWheelDelta > 0) this.Index++;
                else this.Index--;
                this.CanvasControl.Invalidate();
            };
        }


        //Get
        private SolidColorBrush[] GetBrushs(Color[] colors)
        {
            IEnumerable<SolidColorBrush> brushs = from item in colors select new SolidColorBrush(item);
            return brushs.ToArray();
        }
        //Set
        private void SetBrushs(Color[] colors)
        {
            for (int i = 0; i < this.Count; i++)
            {
                this.Brushs[i].Color = colors[i];
            } 
        }

        //Items
        private IEnumerable<Rectangle> GetItems(SolidColorBrush[] brushs)
        {
            IEnumerable<Rectangle> items = from item in brushs select this.BuildRectangle(item);
            return items;
        }
        private Rectangle BuildRectangle(SolidColorBrush brush)
        {
            Rectangle rectangle = new Rectangle
            {
                Fill= brush,
                Width = 44,
                Height = 44,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            rectangle.Tapped += (s, e) =>
            {
                Color color = brush.Color;

                this.Color = color;
                this.ColorChange?.Invoke(this, color);//Delegate
            };

            return rectangle;
        }
    }
}
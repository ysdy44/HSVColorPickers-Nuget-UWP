using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace HSVColorPickers
{
    public class Square
    {
        public Vector2 Center = new Vector2(50, 50);
        public float Width = 100;
        public float Height = 100;
        public float HalfWidth => this.Width / 2;
        public float HalfHeight => this.Height / 2;
        public float StrokePadding = 12;
    }

    public partial class PalettePicker : UserControl, IPicker
    {
        //Delegate
        public event ColorChangeHandler ColorChange = null;
        public Color GetColor() => HSV.HSVtoRGB(this.HSV);
        public void SetColor(Color value) => this.HSV = HSV.RGBtoHSV(value);


        #region DependencyProperty


        private HSV hsl = new HSV { A = 255, H = 0, S = 1, V = 1 };
        private HSV _HSL
        {
            get => this.hsl;
            set
            {
                this.ColorChange?.Invoke(this, HSV.HSVtoRGB(value.A, value.H, value.S, value.V));

                this.hsl = value;
            }
        }
        public HSV HSV
        {
            get => this.hsl;
            set
            {
                this.Action(value);
                this.hsl = value;

                this.CanvasControl.Invalidate();
            }
        }
        public Color Color
        {
            get => this.GetColor();
            set => this.SetColor(value);
        }


        #endregion



        bool IsPalette = false;
        Vector2 Vector;
        Action<HSV> Action;
        Square Square = new Square();
                
        public PalettePicker(PaletteBase paletteBase)
        {
            this.InitializeComponent();

            //Picker
            this.Slider.Minimum = paletteBase.Minimum;
            this.Slider.Maximum = paletteBase.Maximum;

            this.Slider.Value = paletteBase.GetValue(this.hsl);
            this.LinearGradientBrush.GradientStops = paletteBase.GetSliderBrush(this.hsl);

            this.Slider.ValueChangeDelta += (sender, value) => this.HSV = this._HSL = paletteBase.GetHSL(this.hsl, value);

            //Action
            this.Action = (HSV hsl) =>
            {
                this.Slider.Value = paletteBase.GetValue(hsl);
                this.LinearGradientBrush.GradientStops = paletteBase.GetSliderBrush(hsl);
            };

            //Canvas
            this.CanvasControl.SizeChanged += (sender, e) =>
            {
                this.Square.Center = e.NewSize.ToVector2() / 2;

                this.Square.Width = (float)e.NewSize.Width - this.Square.StrokePadding * 2;
                this.Square.Height = (float)e.NewSize.Height - this.Square.StrokePadding * 2;
            };
            this.CanvasControl.Draw += (sender, args) => paletteBase.Draw(this.CanvasControl, args.DrawingSession, this.hsl, this.Square.Center, this.Square.HalfWidth, this.Square.HalfHeight);



            //Manipulation
            this.CanvasControl.ManipulationMode = ManipulationModes.All;
            this.CanvasControl.ManipulationStarted += (sender, e) =>
            {
                this.Vector = e.Position.ToVector2() - this.Square.Center;

                this.IsPalette = Math.Abs(Vector.X) < this.Square.Width && Math.Abs(this.Vector.Y) < this.Square.Height;

                if (this.IsPalette) this.HSV = this._HSL = paletteBase.Delta(this.hsl, this.Vector, this.Square.HalfWidth, this.Square.HalfHeight);
            };
            this.CanvasControl.ManipulationDelta += (sender, e) =>
            {
                this.Vector += e.Delta.Translation.ToVector2();

                if (this.IsPalette) this.HSV = this._HSL = paletteBase.Delta(this.hsl, this.Vector, this.Square.HalfWidth, this.Square.HalfHeight);
            };
            this.CanvasControl.ManipulationCompleted += (sender, e) => this.IsPalette = false;



            this.CanvasControl.Invalidate();
        }


        //@static
        public static PalettePicker CreateFormHue() => new PalettePicker(new PaletteHue());
        public static PalettePicker CreateFormSaturation() => new PalettePicker(new PaletteSaturation());
        public static PalettePicker CreateFormValue() => new PalettePicker(new PaletteValue());
    }



    /// <summary> Palette Base </summary>
    public abstract class PaletteBase
    {
        public string Name;
        public string Unit;
        public double Minimum;
        public double Maximum;

        public abstract HSV GetHSL(HSV HSV, double value);
        public abstract double GetValue(HSV HSV);

        public abstract GradientStopCollection GetSliderBrush(HSV HSV);

        public abstract void Draw(CanvasControl CanvasControl, CanvasDrawingSession ds, HSV HSV, Vector2 Center, float SquareHalfWidth, float SquareHalfHeight);
        public abstract HSV Delta(HSV HSV, Vector2 v, float SquareHalfWidth, float SquareHalfHeight);
    }

    /// <summary> Palette Hue</summary>
    public class PaletteHue : PaletteBase
    {
        public PaletteHue()
        {
            this.Name = "Hue";
            this.Unit = "º";
            this.Minimum = 0;
            this.Maximum = 360;
        }

        public override HSV GetHSL(HSV HSV, double value) => new HSV(HSV.A, value, HSV.S, HSV.V);
        public override double GetValue(HSV HSV) => HSV.H;

        public override GradientStopCollection GetSliderBrush(HSV HSV)
        {
            byte A = HSV.A;
            double H = HSV.H;
            double S = HSV.S;
            double L = HSV.V;

            return new GradientStopCollection()
            {
                new GradientStop()
                {
                    Offset = 0,
                    Color = HSV.HSVtoRGB(A, 0, S, L)
                },
                new GradientStop()
                {
                    Offset = 0.16666667,
                    Color = HSV.HSVtoRGB(A, 60, S, L)
                },
                 new GradientStop()
                {
                    Offset = 0.33333333 ,
                    Color = HSV.HSVtoRGB(A, 120, S, L)
                },
                new GradientStop()
                {
                    Offset = 0.5 ,
                    Color = HSV.HSVtoRGB(A, 180, S, L)
                },
                new GradientStop()
                {
                    Offset = 0.66666667 ,
                    Color = HSV.HSVtoRGB(A, 240, S, L)
                },
                new GradientStop()
                {
                    Offset = 0.83333333 ,
                    Color = HSV.HSVtoRGB(A, 300, S, L)
                },
                new GradientStop()
                {
                    Offset = 1 ,
                    Color = HSV.HSVtoRGB(A, 0, S, L)
                },
            };
        }

        public override void Draw(CanvasControl CanvasControl, CanvasDrawingSession ds, HSV HSV, Vector2 Center, float SquareHalfWidth, float SquareHalfHeight)
        {
            //Palette
            Rect rect = new Rect(Center.X - SquareHalfWidth, Center.Y - SquareHalfHeight, SquareHalfWidth * 2, SquareHalfHeight * 2);
            ds.FillRoundedRectangle(rect, 4, 4, new CanvasLinearGradientBrush(CanvasControl, Windows.UI.Colors.White, HSV.HSVtoRGB(HSV.H)) { StartPoint = new Vector2(Center.X - SquareHalfWidth, Center.Y), EndPoint = new Vector2(Center.X + SquareHalfWidth, Center.Y) });
            ds.FillRoundedRectangle(rect, 4, 4, new CanvasLinearGradientBrush(CanvasControl, Windows.UI.Colors.Transparent, Windows.UI.Colors.Black) { StartPoint = new Vector2(Center.X, Center.Y - SquareHalfHeight), EndPoint = new Vector2(Center.X, Center.Y + SquareHalfHeight) });
            ds.DrawRoundedRectangle(rect, 4, 4, Windows.UI.Colors.Gray);

            //Thumb 
            float px = ((float)HSV.S - 50) * SquareHalfWidth / 50 + Center.X;
            float py = (50 - (float)HSV.V) * SquareHalfHeight / 50 + Center.Y;
            ds.DrawCircle(px, py, 9, Windows.UI.Colors.Black, 5);
            ds.DrawCircle(px, py, 9, Windows.UI.Colors.White, 3);
        }
        public override HSV Delta(HSV HSV, Vector2 v, float SquareHalfWidth, float SquareHalfHeight)
        {
            double S = 50 + v.X * 50 / SquareHalfWidth;
            double L = 50 - v.Y * 50 / SquareHalfHeight;

            return new HSV(HSV.A, HSV.H, S, L);
        }
    }

    /// <summary> Palette Saturation </summary>
    public class PaletteSaturation : PaletteBase
    {
        public CanvasGradientStop[] BackgroundStops = new CanvasGradientStop[]
        {
            new CanvasGradientStop { Position = 0.0f, Color =  Windows.UI.Colors.Red },
            new CanvasGradientStop { Position = 0.16666667f, Color =Windows.UI.Colors.Yellow},
            new CanvasGradientStop { Position = 0.33333333f, Color = Color.FromArgb(255,0,255,0) },
            new CanvasGradientStop { Position = 0.5f, Color = Windows.UI.Colors.Cyan },
            new CanvasGradientStop { Position = 0.66666667f, Color = Windows.UI.Colors.Blue},
            new CanvasGradientStop { Position = 0.83333333f, Color =  Windows.UI.Colors.Magenta },
            new CanvasGradientStop { Position = 1.0f, Color =  Windows.UI.Colors.Red },
        };
        public CanvasGradientStop[] ForegroundStops = new CanvasGradientStop[]
        {
            new CanvasGradientStop { Position = 0.0f, Color = Windows.UI.Colors.Transparent },
            new CanvasGradientStop { Position = 1.0f, Color = Windows.UI.Colors.Black }
        };

        public PaletteSaturation()
        {
            this.Name = "Saturation";
            this.Unit = "%";
            this.Minimum = 0;
            this.Maximum = 100;
        }

        public override HSV GetHSL(HSV HSV, double value) => new HSV(HSV.A, HSV.H, value, HSV.V);
        public override double GetValue(HSV HSV) => HSV.S;
        public override GradientStopCollection GetSliderBrush(HSV HSV)
        {
            byte A = HSV.A;
            double H = HSV.H;
            double S = HSV.S;
            double L = HSV.V;

            return new GradientStopCollection()
            {
                new GradientStop()
                {
                    Offset = 0,
                    Color = HSV.HSVtoRGB(A, H, 0.0d, L)
                },
               new GradientStop()
                {
                    Offset = 1,
                    Color =HSV.HSVtoRGB(A, H, 100.0d, L)
                },
            };
        }

        public override void Draw(CanvasControl CanvasControl, CanvasDrawingSession ds, HSV HSV, Vector2 Center, float SquareHalfWidth, float SquareHalfHeight)
        {
            //Palette
            Rect rect = new Rect(Center.X - SquareHalfWidth, Center.Y - SquareHalfHeight, SquareHalfWidth * 2, SquareHalfHeight * 2);
            using (CanvasLinearGradientBrush rainbow = new CanvasLinearGradientBrush(CanvasControl, this.BackgroundStops))
            {
                rainbow.StartPoint = new Vector2(Center.X - SquareHalfWidth, Center.Y);
                rainbow.EndPoint = new Vector2(Center.X + SquareHalfWidth, Center.Y);
                ds.FillRoundedRectangle(rect, 4, 4, rainbow);
            }
            using (CanvasLinearGradientBrush brush = new CanvasLinearGradientBrush(CanvasControl, this.ForegroundStops))
            {
                brush.StartPoint = new Vector2(Center.X, Center.Y - SquareHalfHeight);
                brush.EndPoint = new Vector2(Center.X, Center.Y + SquareHalfHeight);
                ds.FillRoundedRectangle(rect, 4, 4, brush);
            }
            ds.DrawRoundedRectangle(rect, 4, 4, Windows.UI.Colors.Gray);

            //Thumb 
            float px = ((float)HSV.H - 180) * SquareHalfWidth / 180 + Center.X;
            float py = ((float)(50 - HSV.V)) * SquareHalfHeight / 50 + Center.Y;
            ds.DrawCircle(px, py, 9, Windows.UI.Colors.Black, 5);
            ds.DrawCircle(px, py, 9, Windows.UI.Colors.White, 3);
        }
        public override HSV Delta(HSV HSV, Vector2 v, float SquareHalfWidth, float SquareHalfHeight)
        {
            double H = v.X * 180 / SquareHalfWidth + 180;
            double L = 50 - v.Y * 50 / SquareHalfHeight;
            return new HSV(HSV.A, H, HSV.S, L);
        }
    }

    /// <summary> Palette Value </summary>
    public class PaletteValue : PaletteBase
    {
        public CanvasGradientStop[] BackgroundStops = new CanvasGradientStop[]
        {
            new CanvasGradientStop { Position = 0.0f, Color =  Windows.UI.Colors.Red },
            new CanvasGradientStop { Position = 0.16666667f, Color =Windows.UI.Colors.Yellow},
            new CanvasGradientStop { Position = 0.33333333f, Color = Color.FromArgb(255,0,255,0) },
            new CanvasGradientStop { Position = 0.5f, Color = Windows.UI.Colors.Cyan },
            new CanvasGradientStop { Position = 0.66666667f, Color = Windows.UI.Colors.Blue},
            new CanvasGradientStop { Position = 0.83333333f, Color =  Windows.UI.Colors.Magenta },
            new CanvasGradientStop { Position = 1.0f, Color =  Windows.UI.Colors.Red },
        };
        public CanvasGradientStop[] ForegroundStops = new CanvasGradientStop[]
        {
            new CanvasGradientStop { Position = 0.0f, Color = Color.FromArgb(0,128,128,128) },
            new CanvasGradientStop { Position = 1.0f, Color = Windows.UI.Colors.White }
        };

        public PaletteValue()
        {
            this.Name = "Value";
            this.Unit = "%";
            this.Minimum = 0;
            this.Maximum = 100;
        }

        public override HSV GetHSL(HSV HSV, double value) => new HSV(HSV.A, HSV.H, HSV.S, value);
        public override double GetValue(HSV HSV) => HSV.V;

        public override GradientStopCollection GetSliderBrush(HSV HSV)
        {
            byte A = HSV.A;
            double H = HSV.H;
            double S = HSV.S;
            double L = HSV.V;

            return new GradientStopCollection()
            {
                new GradientStop()
                {
                    Offset = 0.0f ,
                    Color = HSV.HSVtoRGB(A, H, S, 0)
                },
                new GradientStop()
                {
                    Offset = 1.0f,
                    Color = HSV.HSVtoRGB(A, H, S, 100)
                },
           };
        }

        public override void Draw(CanvasControl CanvasControl, CanvasDrawingSession ds, HSV HSV, Vector2 Center, float SquareHalfWidth, float SquareHalfHeight)
        {
            //Palette
            Rect rect = new Rect(Center.X - SquareHalfWidth, Center.Y - SquareHalfHeight, SquareHalfWidth * 2, SquareHalfHeight * 2);
            using (CanvasLinearGradientBrush rainbow = new CanvasLinearGradientBrush(CanvasControl, this.BackgroundStops))
            {
                rainbow.StartPoint = new Vector2(Center.X - SquareHalfWidth, Center.Y);
                rainbow.EndPoint = new Vector2(Center.X + SquareHalfWidth, Center.Y);
                ds.FillRoundedRectangle(rect, 4, 4, rainbow);
            }
            using (CanvasLinearGradientBrush brush = new CanvasLinearGradientBrush(CanvasControl, this.ForegroundStops))
            {
                brush.StartPoint = new Vector2(Center.X, Center.Y - SquareHalfHeight);
                brush.EndPoint = new Vector2(Center.X, Center.Y + SquareHalfHeight);
                ds.FillRoundedRectangle(rect, 4, 4, brush);
            }
            ds.DrawRoundedRectangle(rect, 4, 4, Windows.UI.Colors.Gray);

            //Thumb 
            float px = ((float)HSV.H - 180) * SquareHalfWidth / 180 + Center.X;
            float py = (50 - (float)HSV.S) * SquareHalfHeight / 50 + Center.Y;
            ds.DrawCircle(px, py, 9, Windows.UI.Colors.Black, 5);
            ds.DrawCircle(px, py, 9, Windows.UI.Colors.White, 3);
        }
        public override HSV Delta(HSV HSV, Vector2 v, float SquareHalfWidth, float SquareHalfHeight)
        {
            double H = v.X * 180 / SquareHalfWidth + 180;
            double S = 50 - v.Y * 50 / SquareHalfHeight;
            return new HSV(HSV.A, H, S, HSV.V);
        }
    }

}

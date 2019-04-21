using System;
using Windows.UI;

namespace HsvColorPickers
{
    //Delegate
    public delegate void AlphaChangeHandler(object sender, byte value);
    public delegate void ColorChangeHandler(object sender, Color value);

    /// <summary> Color form HSV </summary>
    public struct HSV
    {

        /// <summary> Alpha </summary>
        public byte A;

        /// <summary> Hue </summary>
        public double H
        {
            get => this.h;
            set
            {
                if (value < 0) this.h = 0;
                else if (value > 360) this.h = 360;
                else this.h = value;
            }
        }
        private double h;

        /// <summary> Saturation </summary>
        public double S
        {
            get => this.s;
            set
            {
                if (value < 0) this.s = 0;
                else if (value > 100) this.s = 100;
                else this.s = value;
            }
        }
        private double s;

        /// <summary> Value </summary>
        public double V
        {
            get => this.v;
            set
            {
                if (value < 0) this.v = 0;
                else if (value > 100) this.v = 100;
                else this.v = value;
            }
        }
        private double v;



        public HSV(byte a, double h, double s, double v) { this.A = a; this.h = h; this.s = v; this.v = v; this.H = H; this.S = s; this.V = v; }



        /// <summary> RGB to HSV </summary>
        /// <param name="h"> Hue </param>
        /// <returns> Color </returns>
        public static Color HSVtoRGB(double h)
        {
            double hh = h / 60;
            byte xhh = (byte)((1 - Math.Abs(hh % 2 - 1)) * 255);

            if (hh < 1) return Color.FromArgb(255, 255, xhh, 0);
            else if (hh < 2) return Color.FromArgb(255, xhh, 255, 0);
            else if (hh < 3) return Color.FromArgb(255, 0, 255, xhh);
            else if (hh < 4) return Color.FromArgb(255, 0, xhh, 255);
            else if (hh < 5) return Color.FromArgb(255, xhh, 0, 255);
            else return Color.FromArgb(255, 255, 0, xhh);
        }

        /// <summary> RGB to HSV </summary>
        /// <param name="hsl"> HSV </param>
        /// <returns> Color </returns>
        public static Color HSVtoRGB(HSV hsl) => HSV.HSVtoRGB(hsl.A, hsl.H, hsl.S, hsl.V);

        /// <summary> RGB to HSV </summary>
        /// <param name="a"> Alpha </param>
        /// <param name="h"> Hue </param>
        /// <param name="s"> Saturation </param>
        /// <param name="v"> Value </param>
        /// <returns> Color </returns>
        public static Color HSVtoRGB(byte a, double h, double s, double v)
        {
            if (h == 360) h = 0;

            if (s == 0)
            {
                byte ll = (byte)(v / 100 * 255);
                return Color.FromArgb(a, ll, ll, ll);
            }

            double S = s / 100;
            double V = v / 100;

            int H1 = (int)(h * 1.0 / 60);
            double F = h / 60 - H1;
            double P = V * (1.0 - S);
            double Q = V * (1.0 - F * S);
            double T = V * (1.0 - (1.0 - F) * S);

            double R = 0, G = 0, B = 0;
            switch (H1)
            {
                case 0: R = V; G = T; B = P; break;
                case 1: R = Q; G = V; B = P; break;
                case 2: R = P; G = V; B = T; break;
                case 3: R = P; G = Q; B = V; break;
                case 4: R = T; G = P; B = V; break;
                case 5: R = V; G = P; B = Q; break;
            }

            R = R * 255;
            while (R > 255) R -= 255;
            while (R < 0) R += 255;

            G = G * 255;
            while (G > 255) G -= 255;
            while (G < 0) G += 255;

            B = B * 255;
            while (B > 255) B -= 255;
            while (B < 0) B += 255;

            return Color.FromArgb(a, (byte)R, (byte)G, (byte)B);
        }



        /// <summary> RGB to HSV </summary>
        /// <param name="color"> Color </param>
        /// <returns> HSV </returns>
        public static HSV RGBtoHSV(Color color) => HSV.RGBtoHSV(color.A, color.R, color.G, color.B);

        /// <summary> RGB to HSV </summary>
        /// <param name="a"> Alpha </param>
        /// <param name="r"> Red </param>
        /// <param name="g"> Green </param>
        /// <param name="b"> Blue </param>
        /// <returns> HSV </returns>
        public static HSV RGBtoHSV(byte a, byte r, byte g, byte b)
        {
            double R = r * 1.0 / 255;
            double G = g * 1.0 / 255;
            double B = b * 1.0 / 255;

            double min = Math.Min(Math.Min(R, G), B);
            double max = Math.Max(Math.Max(R, G), B);

            double H = 0, S, V;

            if (max == min) { H = 0; }

            else if (max == R && G > B) H = 60 * (G - B) * 1.0 / (max - min) + 0;
            else if (max == R && G < B) H = 60 * (G - B) * 1.0 / (max - min) + 360;
            else if (max == G) H = H = 60 * (B - R) * 1.0 / (max - min) + 120;
            else if (max == B) H = H = 60 * (R - G) * 1.0 / (max - min) + 240;

            if (max == 0) S = 0;
            else S = (max - min) * 1.0 / max;

            V = max;

            return new HSV(a, H, (S * 100), V * 100);
        }
    }
}

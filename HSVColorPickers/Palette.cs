using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;

namespace HSVColorPickers
{
    /// <summary>
    /// Extract theme colors from bitmap.
    /// </summary>
    public static class Palette
    {

        /// <summary> The default color. </summary>
        public static readonly Color FallBackColor = Color.FromArgb(255, 177, 101, 105);

        //Size of the cache thumbnail
        private static readonly int width = 8;
        private static readonly int height = 8;
        private static readonly CanvasDevice device = new CanvasDevice();



        /// <summary>
        /// Palette color from Url.
        /// </summary>
        /// <param name="uri"> Web or file path. </param>
        /// <returns> Color </returns>
        public static async Task<Color> GetColorFormImage(Uri uri)
        {
            try
            {
                CanvasBitmap bimap = await CanvasBitmap.LoadAsync(device, uri);

                return GetPaletteFormBitmap(bimap);
            }
            catch (Exception)
            {
                return FallBackColor;
            }
        }

        /// <summary>
        /// Palette color from CanvasBitmap.
        /// </summary>
        /// <param name="bimap"> Win2D CanvasBitmap. </param>
        /// <returns> Color </returns>
        private static Color GetPaletteFormBitmap(CanvasBitmap bimap)
        {
            try
            {
                //scale 
                double scaleX = width / bimap.Size.Width;
                double scaleY = height / bimap.Size.Height;
                Vector2 v = new Vector2((float)scaleX, (float)scaleY);
                Matrix3x2 m = Matrix3x2.CreateScale(v);

                //draw
                using (CanvasRenderTarget target = new CanvasRenderTarget(device, width, height, bimap.Dpi))
                {
                    using (CanvasDrawingSession ds = target.CreateDrawingSession())
                    {
                        Transform2DEffect effect = new Transform2DEffect
                        {
                            Source = bimap,
                            TransformMatrix = m
                        };
                        ds.DrawImage(effect);
                    }

                    //Palette
                    Color[] colors = target.GetPixelColors();
                    return GetPaletteFromColors(colors);
                }
            }
            catch (Exception)
            {
                return FallBackColor;
            }
        }

        /// <summary>
        /// Palette color from colors
        /// </summary>
        /// <param name="colors"> Color array. </param>
        /// <returns> Color </returns>
        public static Color GetPaletteFromColors(Color[] colors)
        {
            try
            {
                //saturation
                double sumS = 0;

                //value
                double sumV = 0;
                double sumHue = 0;

                double maxV = 0;
                double maxS = 0;
                double maxH = 0;
                double count = 0;
                foreach (var color in colors)
                {
                    HSV hsv = HSV.RGBtoHSV(color);

                    if (hsv.H == 0)continue;
                    
                    maxS = hsv.S > maxS ? hsv.S : maxS;
                    maxV = hsv.V > maxV ? hsv.V : maxV;
                    maxH = hsv.H > maxH ? hsv.H : maxH;
                    sumHue += hsv.H;
                    sumS += hsv.S;
                    sumV += hsv.V;
                    count++;
                }


                double avgH = sumHue / count;
                double avgV = sumV / count;
                double avgS = sumS / count;
                double maxAvgV = maxV / 2;
                double maxAvgS = maxS / 2;
                double maxAvgH = maxH / 2;

                double h = Math.Max(maxAvgV, avgV);
                double s = Math.Min(maxAvgS, avgS);
                double hue = Math.Min(maxAvgH, avgH);

                double R = 0;
                double G = 0;
                double B = 0;
                count = 0;


                foreach (var color in colors)
                {
                    HSV hsv = HSV.RGBtoHSV(color);

                    if (hsv.H == 0) continue;

                    if (hsv.H >= hue + 10 && hsv.V >= h && hsv.S >= s)
                    {
                        R += color.R;
                        G += color.G;
                        B += color.B;
                        count++;
                    }
                }

                double r = R / count;
                double g = G / count;
                double b = B / count;

                //   if (r + g + b > 96 && r + g + b < 672)
                return Color.FromArgb(255, (byte)r, (byte)g, (byte)b);
            }
            catch (Exception)
            {
                return FallBackColor;
            }
        }

        /// <summary>
        /// Get CanvasBitmap from url.
        /// </summary>
        /// <param name="uri">web or file path</param>
        /// <returns></returns>
        public static async Task<CanvasBitmap> GetBitmapFormImage(Uri uri)
        {
            try
            {
                return await CanvasBitmap.LoadAsync(device, uri);
            }
            catch (Exception)
            {
                return null;
            }
        }


        /// <summary>
        /// Save the file via a Uri.
        /// </summary>
        /// <param name="url"> Web or file path. </param>
        /// <param name="foldername"> Folder name in PicturesLibrary. </param>
        /// <returns> Saved successfully returns **true**. </returns>
        public static async Task<bool> Save(string url, string foldername)
        {
            try
            {
                string[] Splits = url.Split('/');
                string name = Splits.Last();

                CanvasBitmap bitmap = await Palette.GetBitmapFormImage(new Uri(url));
                CanvasBitmapFileFormat format = Palette.GetFormat(name);

                StorageFolder folder = await KnownFolders.PicturesLibrary.CreateFolderAsync(foldername, CreationCollisionOption.OpenIfExists);
                StorageFile file = await folder.CreateFileAsync(name);

                using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    await bitmap.SaveAsync(fileStream, format);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Save the file via a Uri.
        /// </summary>
        /// <param name="url"> Web or file path. </param>
        /// <param name="folder"> Destination folder. </param>
        /// <returns> Saved successfully returns **true**. </returns>
        public static async Task<bool> Save(string url, StorageFolder folder)
        {
            try
            {
                string[] Splits = url.Split('/');
                string name = Splits.Last();

                CanvasBitmap bitmap = await Palette.GetBitmapFormImage(new Uri(url));
                CanvasBitmapFileFormat format = Palette.GetFormat(name);

                StorageFile file = await folder.CreateFileAsync(name);

                using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    await bitmap.SaveAsync(fileStream, format);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// CanvasBitmap FileFormat.
        /// </summary>
        /// <param name="name"> "BMP", "GIF", "JPEG", "JPEGXR", "PNG", "TIFF". </param>
        /// <returns> Format </returns>
        public static CanvasBitmapFileFormat GetFormat(string name)
        {
            string[] Splits = name.Split('.');
            string format = Splits.Last();
            string Upper = format.ToUpper();

            switch (Upper)
            {
                case "BMP": return CanvasBitmapFileFormat.Bmp;
                case "GIF": return CanvasBitmapFileFormat.Gif;
                case "JPEG": return CanvasBitmapFileFormat.Jpeg;
                case "JPEGXR": return CanvasBitmapFileFormat.JpegXR;
                case "PNG": return CanvasBitmapFileFormat.Png;
                case "TIFF": return CanvasBitmapFileFormat.Tiff;
                default: return CanvasBitmapFileFormat.Jpeg;
            }
        }

    }
}
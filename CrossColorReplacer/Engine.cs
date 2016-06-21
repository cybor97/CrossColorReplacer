using System;
using System.Drawing;

namespace CrossColorReplacer
{
    public class Engine
    {
        public static Bitmap SourceBitmap, TargetBitmap;
        public static bool Completed;
        public static Color SourceColor, TargetColor;
        public static void ReColor(int aSensivity, int rSensivity, int gSensivity, int bSensivity)
        {
            Completed = false;
            int w, h;
            lock (SourceBitmap)
            {
                w = SourceBitmap.Width;
                h = SourceBitmap.Height;
            }
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    Color currentPixel;
                    lock (SourceBitmap)
                        currentPixel = SourceBitmap.GetPixel(x, y);
                    int a = currentPixel.A, r = currentPixel.R, g = currentPixel.G, b = currentPixel.B;
                    bool aLimit = Math.Abs(a - SourceColor.A) <= aSensivity,
                        rLimit = Math.Abs(r - SourceColor.R) <= rSensivity,
                        gLimit = Math.Abs(g - SourceColor.G) <= gSensivity,
                        bLimit = Math.Abs(b - SourceColor.B) <= bSensivity;
                    if (aLimit && rLimit && gLimit && bLimit)
                    {
                        if (aLimit || aSensivity > 150)
                            a = TargetColor.A;
                        if (rLimit || rSensivity > 150)
                            r = TargetColor.R;
                        if (gLimit || gSensivity > 150)
                            g = TargetColor.G;
                        if (bLimit || bSensivity > 150)
                            b = TargetColor.B;
                    }
                    lock (TargetBitmap)
                        TargetBitmap.SetPixel(x, y, Color.FromArgb(a, r, g, b));
                }
            Completed = true;
        }
    }
}

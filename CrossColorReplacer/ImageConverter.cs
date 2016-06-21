using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CrossColorReplacer
{
    public static class BitmapConverter
    {
        public static byte[] ToByteArray(this Icon icon)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                icon.ToBitmap().Save(ms, ImageFormat.Png);
                return ms.GetBuffer();
            }
        }
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        extern static bool DestroyIcon(IntPtr handle);
        public static Icon ToIcon(this byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                Icon tmp = Icon.FromHandle(new Bitmap(ms).GetHicon());
                Icon Result = new Icon(tmp, tmp.Size);
                DestroyIcon(tmp.Handle);
                return Result;
            }
        }
        public static byte[] ToByteArray(this Bitmap image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Png);
                return ms.GetBuffer();
            }
        }
        public static Bitmap ToBitmap(this byte[] ByteArray)
        {
            return (Bitmap)Image.FromStream(new MemoryStream(ByteArray), true, true); ;
        }

        public static System.Drawing.Bitmap ToBitmap(this BitmapSource srs)
        {
            int width = srs.PixelWidth;
            int height = srs.PixelHeight;
            int stride = width * ((srs.Format.BitsPerPixel + 7) / 8);
            IntPtr ptr = IntPtr.Zero;
            try
            {
                ptr = Marshal.AllocHGlobal(height * stride);
                srs.CopyPixels(new Int32Rect(0, 0, width, height), ptr, height * stride, stride);
                using (var btm = new Bitmap(width, height, stride, System.Drawing.Imaging.PixelFormat.Format32bppArgb, ptr))
                    return new Bitmap(btm);
            }
            finally
            {
                if (ptr != IntPtr.Zero) Marshal.FreeHGlobal(ptr);
            }
        }
        public static BitmapSource ToBitmapSource(this Bitmap bitmap)
        {
            if (bitmap != null)
                using (MemoryStream stream = new MemoryStream())
                {
                    bitmap.Save(stream, ImageFormat.Png);
                    stream.Position = 0;
                    BitmapImage result = new BitmapImage();
                    result.BeginInit();
                    // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
                    // Force the bitmap to load right now so we can dispose the stream.
                    result.CacheOption = BitmapCacheOption.OnLoad;
                    result.StreamSource = stream;
                    result.EndInit();
                    result.Freeze();
                    return result;
                }
            else return null;
        }
        public static ImageSource ToImageSource(this Icon icon)
        {
            return (ImageSource)icon.ToBitmap().ToBitmapSource();
        }

    }
}

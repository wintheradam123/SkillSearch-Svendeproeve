using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Shared.Helpers
{
    public abstract class ImageHelper
    {
        public static byte[]? ResizeAndCompressImage(Stream? img, int maxSize, float aspectRatioX = 1, float aspectRatioY = 1)
        {
            int newWidth;
            int newHeight;

            if (img == null)
                return null;

            try
            {
                if (img.Length == 0)
                    return null;

                Bitmap originalBitmap = new Bitmap(img);
                int originalWidth = originalBitmap.Width;
                int originalHeight = originalBitmap.Height;

                if (originalWidth > maxSize || originalHeight > maxSize)
                {
                    // preserve the aspect ratio  
                    float ratio = Math.Min(aspectRatioX, aspectRatioY);
                    newWidth = (int)(maxSize * ratio);
                    newHeight = (int)(maxSize * ratio);
                }
                else
                {
                    newWidth = originalWidth;
                    newHeight = originalHeight;
                }

                Bitmap tempBitmap = new Bitmap(originalBitmap, newWidth, newHeight);
                Graphics tempGraphics = Graphics.FromImage(tempBitmap);

                tempGraphics.SmoothingMode = SmoothingMode.AntiAlias;
                tempGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                tempGraphics.DrawImage(originalBitmap, 0, 0, newWidth, newHeight);

                ImageCodecInfo jpgEncoder = GetEncoderInfo("image/jpeg");
                Encoder encoder = Encoder.Quality;
                EncoderParameters encoderParameters = new EncoderParameters(1);
                EncoderParameter encoderParameter = new EncoderParameter(encoder, 75L);
                encoderParameters.Param[0] = encoderParameter;

                byte[]? outputBytes;
                using (MemoryStream outputStream = new MemoryStream())
                {
                    tempBitmap.Save(outputStream, jpgEncoder, encoderParameters);
                    outputBytes = outputStream.GetBuffer();
                }

                tempBitmap.Dispose();
                tempGraphics.Dispose();
                originalBitmap.Dispose();

                return outputBytes;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in ImageHelper.ResizeAndCompressImage(): " + ex.Message);
                return null;
            }
        }

        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageEncoders())
                if (codec.MimeType == mimeType)
                    return codec;

            return null;
        }
    }
}
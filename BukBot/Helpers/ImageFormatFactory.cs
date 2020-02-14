using System;
using System.Drawing.Imaging;

namespace BukBot.Helpers
{
    class ImageFormatFactory
    {
        public static ImageFormat GetImageFormat(string imageFormat) =>
            imageFormat.ToLower() switch
            {
                "png" => ImageFormat.Png,
                "jpeg" => ImageFormat.Jpeg,
                "jpg" => ImageFormat.Jpeg,
                "gif" => ImageFormat.Gif,
                "exif" => ImageFormat.Exif,
                "tiff" => ImageFormat.Tiff,
                "bmp" => ImageFormat.Bmp,
                "ico" => ImageFormat.Icon,
                "icon" => ImageFormat.Icon,
                _ => throw new Exception("Nieznany format mema")
            };
    }
}

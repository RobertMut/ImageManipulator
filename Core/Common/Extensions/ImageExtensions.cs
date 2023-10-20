using System.Drawing;
using System.Drawing.Imaging;

namespace ImageManipulator.Common.Extensions;

public static class ImageExtensions
{
    public static ImageFormat GetImageFormatFromExtension(this string path) =>
        (Path.GetExtension(path).ToLower()) switch
        {
            ".bmp" => ImageFormat.Bmp,
            ".jpg" => ImageFormat.Jpeg,
            ".jpeg" => ImageFormat.Jpeg,
            ".png" => ImageFormat.Png,
            ".gif" => ImageFormat.Gif,
            ".tiff" => ImageFormat.Tiff,
            ".wmf" => ImageFormat.Wmf,
            ".emf" => ImageFormat.Emf,
            ".ico" => ImageFormat.Icon,
            ".icon" => ImageFormat.Icon,
            _ => throw new InvalidOperationException("Unknown image format")
        };
}
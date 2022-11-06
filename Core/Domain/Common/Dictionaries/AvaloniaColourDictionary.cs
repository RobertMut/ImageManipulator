using Avalonia.Media;
using System.Collections.Generic;

namespace ImageManipulator.Domain.Common.Dictionaries
{
    public static class AvaloniaColourDictionary
    {
        public static Dictionary<string, Color> Colour = new Dictionary<string, Color>
        {
            {"red", Color.FromRgb(128,0,0) },
            {"green", Color.FromRgb(0, 255, 0) },
            {"blue", Color.FromRgb(0, 0, 255) },
            {"gray", Color.FromRgb(90, 90, 90) }
        };
    }
}
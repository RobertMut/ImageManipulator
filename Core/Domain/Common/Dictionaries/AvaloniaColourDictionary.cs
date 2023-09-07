using Avalonia.Media;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace ImageManipulator.Domain.Common.Dictionaries
{
    public static class AvaloniaColourDictionary
    {
        public static readonly ImmutableDictionary<string, Color> Colour = ImmutableDictionary.CreateRange(new[]
        {
            KeyValuePair.Create("red", Color.FromRgb(255,0,0)), 
            KeyValuePair.Create("green", Color.FromRgb(0, 255, 0)), 
            KeyValuePair.Create("blue", Color.FromRgb(0, 0, 255)), 
            KeyValuePair.Create("gray", Color.FromRgb(128, 128, 128)), 
        });
    }
}
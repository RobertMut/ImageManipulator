using Avalonia.Media;
using ReactiveUI;

namespace ImageManipulator.Domain.Models
{
    public class CanvasLineModel : ReactiveObject
    {
        private Color _color;
        private int _width;
        private int _height;
        private int _startX;
        private int _startY;

        public int Width { get => _width; private set => _width = value; }
        public int Height { get => _height; private set => _height = value; }
        public int StartX { get => _startX; private set => _startX = value; }
        public int StartY { get => _startY; private set => _startY = value; }
        public Color Color { get => _color; set => _color = value; }

        public CanvasLineModel(Color color, int width, int height, int startX, int startY)
        {
            Color = color;
            Width = width;
            Height = height;
            StartX = startX;
            StartY = startY;
        }
    }
}

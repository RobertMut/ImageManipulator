using Avalonia;
using Avalonia.Media;

namespace ImageManipulator.Domain.Models
{
    public class CanvasLineModel
    {
        public CanvasLineModel(Point startPoint, Point endPoint, Color color, int canvasTop, int canvasLeft, int zIndex)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
            Color = color;
            CanvasTop = canvasTop;
            CanvasLeft = canvasLeft;
            ZIndex = zIndex;
        }

        public Point StartPoint { get; }
        public Point EndPoint { get; }
        public Color Color { get; }
        public int CanvasTop { get; }
        public int CanvasLeft { get; }
        public int ZIndex { get; }
    }
}
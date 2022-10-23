using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using System;

namespace ImageManipulator.Domain.Common.Builders
{
    public class AvaloniaLineBuilder : IAvaloniaLineBuilder
    {
        private Line _line;

        public AvaloniaLineBuilder() => _line = new Line();

        public IAvaloniaLineBuilder SetWidth(double width)
        {
            _line.Width = width;
            return this;
        }

        public IAvaloniaLineBuilder SetHeight(double height)
        {
            _line.Height = height;
            return this;
        }

        public IAvaloniaLineBuilder SetStartPoint(Point point)
        {
            _line.StartPoint = point;
            return this;
        }

        public IAvaloniaLineBuilder SetEventPointerMoved(EventHandler<Avalonia.Input.PointerEventArgs> eventHandler)
        {
            _line.PointerMoved += eventHandler;
            return this;
        }

        public IAvaloniaLineBuilder SetColour(Color color)
        {
            _line.Fill = new SolidColorBrush(color);
            return this;
        }

        public Line Build() => _line;
        public static AvaloniaLineBuilder Create() => new AvaloniaLineBuilder();
    }

    public interface IAvaloniaLineBuilder
    {
        Line Build();
        IAvaloniaLineBuilder SetColour(Color color);
        IAvaloniaLineBuilder SetEventPointerMoved(EventHandler<PointerEventArgs> eventHandler);
        IAvaloniaLineBuilder SetHeight(double height);
        IAvaloniaLineBuilder SetStartPoint(Point point);
        IAvaloniaLineBuilder SetWidth(double width);
    }
}

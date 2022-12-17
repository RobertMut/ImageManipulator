namespace ImageManipulator.Application.Common.Models
{
    public class ColorDouble
    {
        private double _red;
        private double _blue;
        private double _green;

        public ColorDouble(double red, double blue, double green)
        {
            Red = red;
            Blue = blue;
            Green = green;
        }

        public double Red { get => _red; set => _red = value; }
        public double Blue { get => _blue; set => _blue = value; }
        public double Green { get => _green; set => _green = value; }
    }
}

namespace ImageManipulator.Application.Common.Models
{
    public class ColorDouble
    {
        public ColorDouble(double red, double blue, double green)
        {
            Red = red;
            Blue = blue;
            Green = green;
        }

        public double Red { get; set; }

        public double Blue { get; set; }

        public double Green { get; set; }
    }
}

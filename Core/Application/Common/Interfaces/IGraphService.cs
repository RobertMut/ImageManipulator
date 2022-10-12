using System.Collections.Generic;
using System.Drawing;

namespace ImageManipulator.Application.Common.Interfaces
{
    public interface IGraphService
    {
        Avalonia.Media.Imaging.Bitmap DrawGraphFromInput(Dictionary<Color, double[]> inputData,
            int width = 500,
            int height = 200,
            int horizontalMargins = 5,
            int verticalMargins = 5,
            float brushSize = 1.0f,
            double divideScale = 2,
            bool fillImage = true,
            string xLabel = "x",
            string yLabel = "y");
    }
}
using ImageManipulator.Domain.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

namespace ImageManipulator.Application.Common.Interfaces
{
    public interface IGraphService
    {
        public Bitmap DrawGraphFromInput(Dictionary<Avalonia.Media.Color, double[]> inputData,
                    int width = 500,
                    int height = 200,
                    int horizontalMargins = 5,
                    int verticalMargins = 5,
                    double brushSize = 1,
                    double divideScale = 2);
    }
}
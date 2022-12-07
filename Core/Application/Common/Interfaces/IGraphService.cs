using ImageManipulator.Domain.Models;
using System.Collections.Generic;

namespace ImageManipulator.Application.Common.Interfaces
{
    public interface IGraphService
    {
        public List<CanvasLineModel> DrawGraphFromInput(Dictionary<string, double[]> inputData,
                    int width = 500,
                    int height = 200,
                    int horizontalMargins = 5,
                    int verticalMargins = 5,
                    double brushSize = 1);
    }
}
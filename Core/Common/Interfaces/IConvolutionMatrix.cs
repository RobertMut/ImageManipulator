using ImageManipulator.Common.Enums;

namespace ImageManipulator.Common.Interfaces
{
    public interface IConvolutionMatrix
    {
        Dictionary<SoftenSharpenEnum, double[,]> SoftenSharpenMatrices { get; }
        Dictionary<SobelEnum, double[,]> SobelMatrices { get; }

        double[,] SoftenAverageWithWeight(double weight);
    }
}

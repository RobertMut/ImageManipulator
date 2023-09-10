using ImageManipulator.Common.Enums;

namespace ImageManipulator.Common.Interfaces
{
    public interface IConvolutionMatrix
    {
        Dictionary<SoftenSharpenType, double[,]> SoftenSharpenMatrices { get; }
        Dictionary<SobelType, double[,]> SobelMatrices { get; }

        double[,] SoftenAverageWithWeight(double weight);
    }
}

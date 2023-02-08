using ImageManipulator.Common.Enums;

namespace ImageManipulator.Common.Matrices;

public static class EdgeDetection
{
    public static Dictionary<EdgeDetectionEnum, double[,]> EdgeDetectionMatrices { get; } =
        new Dictionary<EdgeDetectionEnum, double[,]>()
        {
            {
                EdgeDetectionEnum.Laplace, new double[,]
                {
                    { -1, -1, -1, },
                    { -1, 8, -1, },
                    { -1, -1, -1, },
                }
            },
            {
                EdgeDetectionEnum.PrewittVertical, new double[,]
                {
                    { 1, 1, 1, },
                    { 0, 0, 0, },
                    { -1, -1, -1, },
                }
            },
            {
                EdgeDetectionEnum.PrewittHorizontal, new double[,]
                {
                    { -1, 0, 1, },
                    { -1, 0, 1, },
                    { -1, 0, 1, },
                }
            },
            {
                EdgeDetectionEnum.Canny, new double[,]
                {
                    { 1, 2, 1, },
                    { 2, 4, 2, },
                    { 1, 2, 1, },
                }
            },
        };
}
using ImageManipulator.Common.Enums;

namespace ImageManipulator.Common.Matrices;

public static class EdgeDetection
{
    public static Dictionary<EdgeDetectionType, double[,]> EdgeDetectionMatrices { get; } =
        new()
        {
            {
                EdgeDetectionType.Laplace, new double[,]
                {
                    { -1, -1, -1 },
                    { -1, 8, -1 },
                    { -1, -1, -1 }
                }
            },
            {
                EdgeDetectionType.PrewittVertical, new double[,]
                {
                    { 1, 1, 1 },
                    { 0, 0, 0 },
                    { -1, -1, -1 }
                }
            },
            {
                EdgeDetectionType.PrewittHorizontal, new double[,]
                {
                    { -1, 0, 1 },
                    { -1, 0, 1 },
                    { -1, 0, 1 }
                }
            },
            {
                EdgeDetectionType.Canny, new double[,]
                {
                    { 1, 2, 1 },
                    { 2, 4, 2 },
                    { 1, 2, 1 }
                }
            }
        };
}
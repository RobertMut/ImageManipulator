using ImageManipulator.Common.Enums;
using ImageManipulator.Common.Interfaces;

namespace ImageManipulator.Common.Matrices
{
    public class ConvolutionMatrices5x5 : IConvolutionMatrix
    {

        public Dictionary<SoftenSharpenType, double[,]> SoftenSharpenMatrices { get; } = new()
        {
            {SoftenSharpenType.SoftenAverage,  new double[,]
                {
                    { 1, 1, 1, 1, 1},
                    {1, 1, 1, 1, 1},
                    {1, 1, 1, 1, 1},
                    {1, 1, 1, 1, 1},
                    {1, 1, 1, 1, 1}
                }
            },
            {SoftenSharpenType.SoftenGauss,  new double[,]
                {
                    { 1, 4, 7, 4, 1 },
                    { 4, 16, 26, 16, 4 },
                    { 7, 26, 41, 26, 7 },
                    { 4, 16, 26, 16, 4 },
                    { 1, 4, 7, 4, 1 },
                }
            },

            {SoftenSharpenType.SharpenLaplace1,  new double[,]
                {
                    { 0, 0, -1, 0, 0 },
                    { 0, -1, -2, -1, 0},
                    { -1, -2, 17, -2, -1, },
                    { 0, -1, -2, -1, 0, },
                    { 0, 0, -1, 0, 0, }

                }
            },
            {SoftenSharpenType.SharpenLaplace2,  new double[,]
                {
                    { 0, 0, -1, 0, 0 },
                    { 0, -1, -2, -1, 0},
                    { -1, -2, 16, -2, -1, },
                    { 0, -1, -2, -1, 0, },
                    { 0, 0, -1, 0, 0, }
                }
            }
        };

        public Dictionary<SobelType, double[,]> SobelMatrices { get; } = new()
        {
            {SobelType.Sobel1,  new double[,]
                {
                    { 2, 2, 4, 2, 2  },
                    { 1, 1, 2, 1, 1  },
                    { 0, 0, 0, 0, 0},
                    { -1, -1, -2, -1, -1},
                    { -2, -2, -4, -2, -2},

                }
            },
            {SobelType.Sobel2,  new double[,]
                {
                    {-2, -1, 0, 1, 2},
                    {-2, -1, 0, 1, 2},
                    {-4, -2, 0, 2, 4},
                    {-2, -1, 0, 1, 2},
                    {-2, -1, 0, 1, 2},

                }
            },
            {SobelType.Sobel3,  new double[,]
                {
                    {0, 0, 1, 2,  4},
                    {0, 0, 1, 2,  2},
                    {-1, -1, 0, 1, 1 },
                    {-2, -2, -1, 0, 0},
                    {-4, -2, -1, 0, 0},

                }
            },
            {SobelType.Sobel4,  new double[,]
                {
                    {-4, -2, -1, 0, 0},
                    {-2, -2, -1, 0, 0},
                    {-1, -1, 0,  1, 1 },
                    {0,   0, 1,  2, 2 },
                    {0,   0, 1,  2, 4 },

                }
            },
            {SobelType.Sobel5,  new double[,]
                {
                    { -2, -2, -4, -2, -2  },
                    { -1, -1, -2, -1, -1  },
                    {0, 0, 0, 0, 0},
                    { 1, 1, 2, 1, 1},
                     { 2, 2, 4, 2, 2},
                }
            },
            {SobelType.Sobel6,  new double[,]
                {
                    {4, 2, 1, 0, 0},
                    {2, 2, 1, 0, 0},
                    {1, 1, 0,  -1, -1 },
                    {0, 0, -1, -2, -2 },
                    {0, 0, -1, -2, -4 },
                }
            },
            {SobelType.Sobel7,  new double[,]
                {
                    {2, 1, 0, -1, -2},
                    {2, 1, 0, -1, -2},
                    {4, 2, 0, -2, -4},
                    {2, 1, 0, -1, -2},
                    {2, 1, 0, -1, -2},
                }
            },
            {SobelType.Sobel8,  new double[,]
                {
                    {0, 0, -1, -2, -4},
                    {0, 0, -1, -2, -2},
                    {1, 1, 0, -1, -1},
                    {2, 2, 1, 0, 0},
                    {4, 2, 1, 0, 0},
                }
            }
        };

        public double[,] SoftenAverageWithWeight(double weight)
        {
            return new double[,]
            {
                { 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1 },
                { 1, 1, weight, 1, 1 },
                { 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1 },

            };
        }
    }
}

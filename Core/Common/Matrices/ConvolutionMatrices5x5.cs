using ImageManipulator.Common.Enums;
using ImageManipulator.Common.Interfaces;

namespace ImageManipulator.Common.Matrices
{
    public class ConvolutionMatrices5x5 : IConvolutionMatrix
    {

        public Dictionary<SoftenSharpenEnum, double[,]> SoftenSharpenMatrices { get; } = new()
        {
            {SoftenSharpenEnum.SoftenAverage,  new double[,]
                {
                    { 1, 1, 1, 1, 1},
                    {1, 1, 1, 1, 1},
                    {1, 1, 1, 1, 1},
                    {1, 1, 1, 1, 1},
                    {1, 1, 1, 1, 1}
                }
            },
            {SoftenSharpenEnum.SoftenGauss,  new double[,]
                {
                    { 1, 4, 7, 4, 1 },
                    { 4, 16, 26, 16, 4 },
                    { 7, 26, 41, 26, 7 },
                    { 4, 16, 26, 16, 4 },
                    { 1, 4, 7, 4, 1 },
                }
            },

            {SoftenSharpenEnum.SharpenLaplace1,  new double[,]
                {
                    { 0, 0, -1, 0, 0 },
                    { 0, -1, -2, -1, 0},
                    { -1, -2, 17, -2, -1, },
                    { 0, -1, -2, -1, 0, },
                    { 0, 0, -1, 0, 0, }

                }
            },
            {SoftenSharpenEnum.SharpenLaplace2,  new double[,]
                {
                    { 0, 0, -1, 0, 0 },
                    { 0, -1, -2, -1, 0},
                    { -1, -2, 16, -2, -1, },
                    { 0, -1, -2, -1, 0, },
                    { 0, 0, -1, 0, 0, }
                }
            }
        };

        public Dictionary<SobelEnum, double[,]> SobelMatrices { get; } = new()
        {
            {SobelEnum.Sobel1,  new double[,]
                {
                    { 2, 2, 4, 2, 2  },
                    { 1, 1, 2, 1, 1  },
                    { 0, 0, 0, 0, 0},
                    { -1, -1, -2, -1, -1},
                    { -2, -2, -4, -2, -2},

                }
            },
            {SobelEnum.Sobel2,  new double[,]
                {
                    {-2, -1, 0, 1, 2},
                    {-2, -1, 0, 1, 2},
                    {-4, -2, 0, 2, 4},
                    {-2, -1, 0, 1, 2},
                    {-2, -1, 0, 1, 2},

                }
            },
            {SobelEnum.Sobel3,  new double[,]
                {
                    {0, 0, 1, 2,  4},
                    {0, 0, 1, 2,  2},
                    {-1, -1, 0, 1, 1 },
                    {-2, -2, -1, 0, 0},
                    {-4, -2, -1, 0, 0},

                }
            },
            {SobelEnum.Sobel4,  new double[,]
                {
                    {-4, -2, -1, 0, 0},
                    {-2, -2, -1, 0, 0},
                    {-1, -1, 0,  1, 1 },
                    {0,   0, 1,  2, 2 },
                    {0,   0, 1,  2, 4 },

                }
            },
            {SobelEnum.Sobel5,  new double[,]
                {
                    { -2, -2, -4, -2, -2  },
                    { -1, -1, -2, -1, -1  },
                    {0, 0, 0, 0, 0},
                    { 1, 1, 2, 1, 1},
                     { 2, 2, 4, 2, 2},
                }
            },
            {SobelEnum.Sobel6,  new double[,]
                {
                    {4, 2, 1, 0, 0},
                    {2, 2, 1, 0, 0},
                    {1, 1, 0,  -1, -1 },
                    {0, 0, -1, -2, -2 },
                    {0, 0, -1, -2, -4 },
                }
            },
            {SobelEnum.Sobel7,  new double[,]
                {
                    {2, 1, 0, -1, -2},
                    {2, 1, 0, -1, -2},
                    {4, 2, 0, -2, -4},
                    {2, 1, 0, -1, -2},
                    {2, 1, 0, -1, -2},
                }
            },
            {SobelEnum.Sobel8,  new double[,]
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

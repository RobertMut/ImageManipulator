using ImageManipulator.Domain.Common.Enums;
using System.Collections.Generic;

namespace ImageManipulator.Domain.Common.Statics
{
    public static class ConvolutionMatrices5x5
    {

        public static Dictionary<SoftenSharpen5x5Enum, double[,]> SoftenSharpenMatrices = new Dictionary<SoftenSharpen5x5Enum, double[,]>()
        {
            {SoftenSharpen5x5Enum.SoftenAverage5x5,  new double[,]
                {
                    { 1, 1, 1, 1, 1},
                    {1, 1, 1, 1, 1},
                    {1, 1, 1, 1, 1},
                    {1, 1, 1, 1, 1},
                    {1, 1, 1, 1, 1}
                }
            },
            {SoftenSharpen5x5Enum.SoftenGauss5x5,  new double[,]
                {
                    { 1, 4, 7, 4, 1 },
                    { 4, 16, 26, 16, 4 },
                    { 7, 26, 41, 26, 7 },
                    { 4, 16, 26, 16, 4 },
                    { 1, 4, 7, 4, 1 },
                }
            },

            {SoftenSharpen5x5Enum.Sharpen5x5Laplace1,  new double[,]
                {
                    { 0, 0, -1, 0, 0 },
                    { 0, -1, -2, -1, 0},
                    { -1, -2, 17, -2, -1, },
                    { 0, -1, -2, -1, 0, },
                    { 0, 0, -1, 0, 0, }

                }
            },
            {SoftenSharpen5x5Enum.Sharpen5x5Laplace2,  new double[,]
                {
                    { 0, 0, -1, 0, 0 },
                    { 0, -1, -2, -1, 0},
                    { -1, -2, 16, -2, -1, },
                    { 0, -1, -2, -1, 0, },
                    { 0, 0, -1, 0, 0, }
                }
            }
        };

        public static Dictionary<Sobel5x5Enum, double[,]> SobelMatrices = new Dictionary<Sobel5x5Enum, double[,]>()
        {
            {Sobel5x5Enum.Sobel1,  new double[,]
                {
                    { 2, 2, 4, 2, 2  },
                    { 1, 1, 2, 1, 1  },
                    { 0, 0, 0, 0, 0},
                    { -1, -1, -2, -1, -1},
                    { -2, -2, -4, -2, -2},

                }
            },
            {Sobel5x5Enum.Sobel2,  new double[,]
                {
                    {-2, -1, 0, 1, 2},
                    {-2, -1, 0, 1, 2},
                    {-4, -2, 0, 2, 4},
                    {-2, -1, 0, 1, 2},
                    {-2, -1, 0, 1, 2},

                }
            },
            {Sobel5x5Enum.Sobel3,  new double[,]
                {
                    {0, 0, 1, 2,  4},
                    {0, 0, 1, 2,  2},
                    {-1, -1, 0, 1, 1 },
                    {-2, -2, -1, 0, 0},
                    {-4, -2, -1, 0, 0},

                }
            },
            {Sobel5x5Enum.Sobel4,  new double[,]
                {
                    {-4, -2, -1, 0, 0},
                    {-2, -2, -1, 0, 0},
                    {-1, -1, 0,  1, 1 },
                    {0,   0, 1,  2, 2 },
                    {0,   0, 1,  2, 4 },

                }
            },
            {Sobel5x5Enum.Sobel5,  new double[,]
                {
                    { -2, -2, -4, -2, -2  },
                    { -1, -1, -2, -1, -1  },
                    {0, 0, 0, 0, 0},
                    { 1, 1, 2, 1, 1},
                     { 2, 2, 4, 2, 2},
                }
            },
            {Sobel5x5Enum.Sobel6,  new double[,]
                {
                    {4, 2, 1, 0, 0},
                    {2, 2, 1, 0, 0},
                    {1, 1, 0,  -1, -1 },
                    {0, 0, -1, -2, -2 },
                    {0, 0, -1, -2, -4 },
                }
            },
            {Sobel5x5Enum.Sobel7,  new double[,]
                {
                    {2, 1, 0, -1, -2},
                    {2, 1, 0, -1, -2},
                    {4, 2, 0, -2, -4},
                    {2, 1, 0, -1, -2},
                    {2, 1, 0, -1, -2},
                }
            },
            {Sobel5x5Enum.Sobel8,  new double[,]
                {
                    {0, 0, -1, -2, -4},
                    {0, 0, -1, -2, -2},
                    {1, 1, 0, -1, -1},
                    {2, 2, 1, 0, 0},
                    {4, 2, 1, 0, 0},
                }
            }
        };

        public static double[,] SoftenAverage5x5WithWeight(double weight)
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

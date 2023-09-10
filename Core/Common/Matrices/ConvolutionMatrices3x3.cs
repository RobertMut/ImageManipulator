using ImageManipulator.Common.Enums;
using ImageManipulator.Common.Interfaces;

namespace ImageManipulator.Common.Matrices
{
    public class ConvolutionMatrices3x3 : IConvolutionMatrix
    {

        public Dictionary<SoftenSharpenType, double[,]> SoftenSharpenMatrices { get; } = new()
        {
            {SoftenSharpenType.SoftenAverage,  new double[,]
                {
                    { 1, 1, 1, },
                    { 1, 1, 1, },
                    { 1, 1, 1, }
                }
            },
            {SoftenSharpenType.SoftenGauss,  new double[,]
                {
                    { 1, 2, 1, },
                    { 2, 4, 2, },
                    { 1, 2, 1, },
                }
            },
            {SoftenSharpenType.SharpenLaplace1,  new double[,]
                {
                    { 0, -1, 0, },
                    { -1, 4, -1,},
                    { 0, -1, 0, },
                }
            },
            {SoftenSharpenType.SharpenLaplace2,  new double[,]
                {
                    { -1, -1, -1,},
                    { -1, 8, -1, },
                    { -1, -1, -1,},
                }
            },
            {SoftenSharpenType.SharpenLaplace3,  new double[,]
                {
                    { 1, -2, 1, },
                    { -2, 4, -2,},
                    { 1, -2, 1, },
                }
            }
        };

        public Dictionary<SobelType, double[,]> SobelMatrices { get; } = new()
        {
            {SobelType.Sobel1,  new double[,]
                {
                    { 1, 2, 1,   },
                    { 0, 0, 0,   },
                    { -1, -2, -1,},
                }
            },
            {SobelType.Sobel2,  new double[,]
                {
                    { -1, 0, 1, },
                    { -2, 0, 2, },
                    { -1, 0, 1, },
                }
            },
            {SobelType.Sobel3,  new double[,]
                {
                    { 0, 1, 2,  },
                    { -1, 0, 1, },
                    { -2, -1, 0,},
                }
            },
            {SobelType.Sobel4,  new double[,]
                {
                    { -2, -1, 0,},
                    { -1, 0, 1, },
                    { 0, 1, 2,  },
                }
            },
            {SobelType.Sobel5,  new double[,]
                {
                    { -1, -2, -1,},
                    { 0, 0, 0,   },
                    { 1, 2, 1,   },
                }
            },
            {SobelType.Sobel6,  new double[,]
                {
                    { 2, 1, 0,  },
                    { 1, 0, -1, },
                    { 0, -1, -2,},
                }
            },
            {SobelType.Sobel7,  new double[,]
                {
                    { 1, 0, -1, },
                    { 2, 0, -2, },
                    { 1, 0, -1, },
                }
            },
            {SobelType.Sobel8,  new double[,]
                {
                    { 0, -1, -2,},
                    { 1, 0, -1, },
                    { 2, 1, 0,  },
                }
            },
        };

        public double[,] SoftenAverageWithWeight(double weight)
        {
            return new double[,]
            {
                { 1, 1, 1, },
                { 1, weight, 1, },
                { 1, 1, 1, },
            };
        }
    }
}

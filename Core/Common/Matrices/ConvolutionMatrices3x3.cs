using ImageManipulator.Common.Enums;
using ImageManipulator.Common.Interfaces;

namespace ImageManipulator.Common.Matrices
{
    public class ConvolutionMatrices3x3 : IConvolutionMatrix
    {

        public Dictionary<SoftenSharpenEnum, double[,]> SoftenSharpenMatrices { get; } = new()
        {
            {SoftenSharpenEnum.SoftenAverage,  new double[,]
                {
                    { 1, 1, 1, },
                    { 1, 1, 1, },
                    { 1, 1, 1, }
                }
            },
            {SoftenSharpenEnum.SoftenGauss,  new double[,]
                {
                    { 1, 2, 1, },
                    { 2, 4, 2, },
                    { 1, 2, 1, },
                }
            },
            {SoftenSharpenEnum.SharpenLaplace1,  new double[,]
                {
                    { 0, -1, 0, },
                    { -1, 4, -1,},
                    { 0, -1, 0, },
                }
            },
            {SoftenSharpenEnum.SharpenLaplace2,  new double[,]
                {
                    { -1, -1, -1,},
                    { -1, 8, -1, },
                    { -1, -1, -1,},
                }
            },
            {SoftenSharpenEnum.SharpenLaplace3,  new double[,]
                {
                    { 1, -2, 1, },
                    { -2, 4, -2,},
                    { 1, -2, 1, },
                }
            }
        };

        public Dictionary<SobelEnum, double[,]> SobelMatrices { get; } = new()
        {
            {SobelEnum.Sobel1,  new double[,]
                {
                    { 1, 2, 1,   },
                    { 0, 0, 0,   },
                    { -1, -2, -1,},
                }
            },
            {SobelEnum.Sobel2,  new double[,]
                {
                    { -1, 0, 1, },
                    { -2, 0, 2, },
                    { -1, 0, 1, },
                }
            },
            {SobelEnum.Sobel3,  new double[,]
                {
                    { 0, 1, 2,  },
                    { -1, 0, 1, },
                    { -2, -1, 0,},
                }
            },
            {SobelEnum.Sobel4,  new double[,]
                {
                    { -2, -1, 0,},
                    { -1, 0, 1, },
                    { 0, 1, 2,  },
                }
            },
            {SobelEnum.Sobel5,  new double[,]
                {
                    { -1, -2, -1,},
                    { 0, 0, 0,   },
                    { 1, 2, 1,   },
                }
            },
            {SobelEnum.Sobel6,  new double[,]
                {
                    { 2, 1, 0,  },
                    { 1, 0, -1, },
                    { 0, -1, -2,},
                }
            },
            {SobelEnum.Sobel7,  new double[,]
                {
                    { 1, 0, -1, },
                    { 2, 0, -2, },
                    { 1, 0, -1, },
                }
            },
            {SobelEnum.Sobel8,  new double[,]
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

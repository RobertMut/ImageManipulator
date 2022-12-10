﻿using ImageManipulator.Domain.Common.Enums;
using System.Collections.Generic;

namespace ImageManipulator.Domain.Common.Statics
{
    public static class ConvolutionMatrices3x3
    {

        public static Dictionary<SoftenSharpen3x3Enum, double[,]> SoftenSharpenMatrices = new Dictionary<SoftenSharpen3x3Enum, double[,]>()
        {
            {SoftenSharpen3x3Enum.SoftenAverage3x3,  new double[,]
                {
                    { 1, 1, 1, },
                    { 1, 1, 1, },
                    { 1, 1, 1, }
                }
            },
            {SoftenSharpen3x3Enum.SoftenGauss3x3,  new double[,]
                {
                    { 1, 2, 1, },
                    { 2, 4, 2, },
                    { 1, 2, 1, },
                }
            },
            {SoftenSharpen3x3Enum.Sharpen3x3Laplace1,  new double[,]
                {
                    { 0, -1, 0, },
                    { -1, 4, -1,},
                    { 0, -1, 0, },
                }
            },
            {SoftenSharpen3x3Enum.Sharpen3x3Laplace2,  new double[,]
                {
                    { -1, -1, -1,},
                    { -1, 8, -1, },
                    { -1, -1, -1,},
                }
            },
            {SoftenSharpen3x3Enum.Sharpen3x3Laplace3,  new double[,]
                {
                    { 1, -2, 1, },
                    { -2, 4, -2,},
                    { 1, -2, 1, },
                }
            }
        };

        public static Dictionary<SobelEnum, double[,]> SobelMatrices = new Dictionary<SobelEnum, double[,]>()
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

        public static double[,] SoftenAverage3x3WithWeight(double weight)
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
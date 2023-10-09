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
                    { 1, 1, 1 },
                    { 1, 1, 1 },
                    { 1, 1, 1 }
                }
            },
            {SoftenSharpenType.SoftenGauss,  new double[,]
                {
                    { 1, 2, 1 },
                    { 2, 4, 2 },
                    { 1, 2, 1 }
                }
            },
            {SoftenSharpenType.Laplace1,  new double[,]
                {
                    { 0, -1, 0 },
                    { -1, 4, -1 },
                    { 0, -1, 0 }
                }
            },
            {SoftenSharpenType.Laplace2,  new double[,]
                {
                    { -1, -1, -1 },
                    { -1, 8, -1 },
                    { -1, -1, -1 }
                }
            },
            {SoftenSharpenType.Laplace3,  new double[,]
                {
                    { 1, -2, 1 },
                    { -2, 4, -2 },
                    { 1, -2, 1 }
                }
            }
        };

        public Dictionary<SobelType, double[,]> SobelMatrices { get; } = new()
        {
            {SobelType.North,  new double[,]
                {
                    { 1, 2, 1 },
                    { 0, 0, 0 },
                    { -1, -2, -1 }
                }
            },
            {SobelType.East,  new double[,]
                {
                    { -1, 0, 1 },
                    { -2, 0, 2 },
                    { -1, 0, 1 }
                }
            },
            {SobelType.NorthEast,  new double[,]
                {
                    { 0, 1, 2 },
                    { -1, 0, 1 },
                    { -2, -1, 0 }
                }
            },
            {SobelType.SouthEast,  new double[,]
                {
                    { -2, -1, 0 },
                    { -1, 0, 1 },
                    { 0, 1, 2 }
                }
            },
            {SobelType.South,  new double[,]
                {
                    { -1, -2, -1 },
                    { 0, 0, 0 },
                    { 1, 2, 1 }
                }
            },
            {SobelType.NorthWest,  new double[,]
                {
                    { 2, 1, 0 },
                    { 1, 0, -1 },
                    { 0, -1, -2 }
                }
            },
            {SobelType.West,  new double[,]
                {
                    { 1, 0, -1 },
                    { 2, 0, -2 },
                    { 1, 0, -1 }
                }
            },
            {SobelType.SouthWest,  new double[,]
                {
                    { 0, -1, -2 },
                    { 1, 0, -1 },
                    { 2, 1, 0 }
                }
            }
        };

        public double[,] SoftenAverageWithWeight(double weight)
        {
            return new[,]
            {
                { 1, 1, 1 },
                { 1, weight, 1 },
                { 1, 1, 1 }
            };
        }
    }
}

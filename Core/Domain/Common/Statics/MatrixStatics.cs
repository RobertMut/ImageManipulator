namespace ImageManipulator.Domain.Common.Statics
{
    public static class MatrixStatics
    {

        public static double[,] SoftenAverage3x3 = new double[,]
        {
            { 1, 1, 1, },
            { 1, 1, 1, },
            { 1, 1, 1, },
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


        public static double[,] SoftenGauss3x3 = new double[,]
        {
            { 1, 2, 1, },
            { 2, 4, 2, },
            { 1, 2, 1, },
        };


        public static double[,] Sharpen3x3Laplace1 = new double[,]
        {
            { 0, -1, 0, },
            { -1, 4, -1,},
            { 0, -1, 0, },
        };

        public static double[,] Sharpen3x3Laplace2 = new double[,]
        {
            { -1, -1, -1,},
            { -1, 8, -1, },
            { -1, -1, -1,},
        };

        public static double[,] Sharpen3x3Laplace3 = new double[,]
        {
            { 1, -2, 1, },
            { -2, 4, -2,},
            { 1, -2, 1, },
        };
    }
}

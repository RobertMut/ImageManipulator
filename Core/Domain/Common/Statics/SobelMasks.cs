namespace ImageManipulator.Domain.Common.Statics
{
    public static class SobelMasks
    {
        public static double[,] Sobel1 = new double[,]
        {
            { 1, 2, 1,    },
            { 0, 0, 0,    },
            { -1, -2, -1, },
        };

        public static double[,] Sobel2 = new double[,]
        {
            { -1, 0, 1, },
            { -2, 0, 2, },
            { -1, 0, 1, },
        };

        public static double[,] Sobel3 = new double[,]
        {
            { 0, 1, 2,   },
            { -1, 0, 1,  },
            { -2, -1, 0, },
        };

        public static double[,] Sobel4 = new double[,]
        {
            { -2, -1, 0, },
            { -1, 0, 1,  },
            { 0, 1, 2,   },
        };

        public static double[,] Sobel5 = new double[,]
        {
            { -1, -2, -1,},
            { 0, 0, 0,   },
            { 1, 2, 1,   },
        };

        public static double[,] Sobel6 = new double[,]
        {
            { 2, 1, 0,  },
            { 1, 0, -1, },
            { 0, -1, -2,},
        };

        public static double[,] Sobel7 = new double[,]
        {
            { 1, 0, -1, },
            { 2, 0, -2, },
            { 1, 0, -1, },
        };

        public static double[,] Sobel8 = new double[,]
        {
            { 0, -1, -2,},
            { 1, 0, -1, },
            { 2, 1, 0,  },
        };
    }
}

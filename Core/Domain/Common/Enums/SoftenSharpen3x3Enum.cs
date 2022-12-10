namespace ImageManipulator.Domain.Common.Enums
{
    public enum SoftenSharpen3x3Enum
    {
        SoftenAverage3x3 = 0,
        SoftenGauss3x3 = 1,
        SoftenAverage3x3WithWeight = 2,
        Sharpen3x3Laplace1 = 3,
        Sharpen3x3Laplace2 = 4,
        Sharpen3x3Laplace3 = 5
    }
}

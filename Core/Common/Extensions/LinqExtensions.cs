namespace ImageManipulator.Common.Extensions
{
    public static class LinqExtensions
    {
        /// <summary>
        /// Cumulative sum for sequence
        /// </summary>
        /// <remarks>
        /// Sadly, generic interfaces for numbers comes with .NET7 - IBinaryInteger, INumeric - <see cref="https://devblogs.microsoft.com/dotnet/dotnet-7-generic-math/"/>
        /// </remarks>
        /// <param name="sequence">IEnumerable double</param>
        /// <returns>IEnumerable double</returns>
        public static IEnumerable<double> CumulativeSum(this IEnumerable<double> sequence)
        {
            double sum = 0;

            foreach (var item in sequence)
            {
                sum += item;
                yield return sum;
            }
        }
    }
}

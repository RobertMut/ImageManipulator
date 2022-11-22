namespace ImageManipulator.Common.Extensions
{
    public static class LinqExtensions
    {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable)
            {
                action(item);

                yield return item;
            }
        }

        public static T ReplaceAndReturn<T>(this IList<T> list, int index, T item)
        {
            list.RemoveAt(index);
            list.Insert(index, item);

            return item;
        }

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

        /// <summary>
        /// If double item equals value returns true
        /// </summary>
        /// <param name="doubles">Double enumerable</param>
        /// <param name="value">Value to be masked</param>
        /// <returns>Bool enumerable</returns>
        public static IEnumerable<bool> MaskedEqual(this IEnumerable<double> doubles, double value)
        {
            foreach (var item in doubles)
            {
                yield return item == value;
            }
        }

        /// <summary>
        /// If boolean enumerable value corresponds to true in double enumerable then replace it by given value
        /// </summary>
        /// <param name="doubleEnumerable">double enumerable</param>
        /// <param name="boolEnumerable"> boolean enumerable used as mask</param>
        /// <param name="value"></param>
        /// <returns>Masking value</returns>
        /// <exception cref="Exception">When enumerables counts are different</exception>
        public static IEnumerable<double> Filled(this IEnumerable<double> doubleEnumerable, IEnumerable<bool> boolEnumerable, double value)
        {
            if (doubleEnumerable.Count() != boolEnumerable.Count())
            {
                throw new Exception($"Inconsinstent enumerables, given {doubleEnumerable.Count()} to booleans {boolEnumerable.Count()}");
            }

            double[] doubles = new double[doubleEnumerable.Count()];
            var doublesArray = doubleEnumerable.ToArray();
            var boolArray = boolEnumerable.ToArray();
            for (int i = 0; i < doubles.Length; i++)
            {
                doubles[i] = boolArray[i] == false ? doublesArray[i] : value;
            }

            return doubles;
        }
    }
}

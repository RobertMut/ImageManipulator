namespace ImageManipulator.Common.Extensions
{
    public static class ArrayExtensions
    {
        public static T[] Slice<T>(this T[] source, int index, int length)
        {
            T[] slice = new T[length];
            Array.Copy(source, index, slice, 0, length);
            return slice;
        }

        public static IEnumerable<T[]> GetSlices<T>(this T[] values, int length)
        {
            for (int i = 0; i < values.Length; i += length)
                yield return values.Slice(i, length);
        }
    }
}
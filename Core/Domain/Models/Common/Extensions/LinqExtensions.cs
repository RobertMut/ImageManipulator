using System;
using System.Collections.Generic;

namespace ImageManipulator.Common.Common.Extensions
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
    }
}

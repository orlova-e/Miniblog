using System;
using System.Collections.Generic;
using System.Linq;

namespace Services
{
    public static class LinqExtension
    {
        public static IEnumerable<TSource> ExceptBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            IEnumerable<TSource> source2)
            where TSource : class
        {
            IEnumerable<TKey> source2Keys = source2.Select(keySelector);
            foreach (TSource element in source)
            {
                if (!source2Keys.Contains(keySelector.Invoke(element)))
                {
                    yield return element;
                }
            }
        }

        public static IEnumerable<TSource> IntersectBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            IEnumerable<TSource> source2)
            where TSource : class
        {
            IEnumerable<TKey> source2Keys = source2.Select(keySelector);
            foreach (TSource element in source)
            {
                if (source2Keys.Contains(keySelector.Invoke(element)))
                {
                    yield return element;
                }
            }
        }

        public static IEnumerable<TSource> UnionBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            IEnumerable<TSource> source2)
            where TSource : class
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            IEnumerable<TSource> sources = source.Concat(source2);
            foreach (TSource element in sources)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
            where TSource : class
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}

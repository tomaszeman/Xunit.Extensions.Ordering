using System;
using System.Collections.Generic;
using System.Linq;

namespace Xunit.Extensions.Ordering
{
    /// <summary>
    /// Classes used to build a Topological Sort of tests based on their dependencies
    /// </summary>
    /// Adapted from https://www.codeproject.com/Articles/869059/Topological-sorting-in-Csharp
    public static class TestTopologicalSort
    {
        private static Func<T, IEnumerable<T>> RemapDependencies<T, TKey>(IEnumerable<T> source, Func<T, IEnumerable<TKey>> getDependencies, Func<T, TKey> getKey)
        {
            var map = source.ToDictionary(getKey);
            return item =>
            {
                var dependencies = getDependencies(item);
                return dependencies?.Select(key => map[key]);
            };
        }

        public static IList<T> TSort<T, TKey>(this IEnumerable<T> source, Func<T, IEnumerable<TKey>> getDependencies, Func<T, TKey> getKey)
        {
            var enumerable = source as T[] ?? source.ToArray();
            return TSort<T>(enumerable, RemapDependencies(enumerable, getDependencies, getKey));
        }

        public static IList<T> TSort<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> getDependencies)
        {
            var sorted = new List<T>();
            var visited = new Dictionary<T, bool>();

            foreach (var item in source)
            {
                Visit(item, getDependencies, sorted, visited);
            }

            return sorted;
        }

        public static void Visit<T>(T item, Func<T, IEnumerable<T>> getDependencies, List<T> sorted, Dictionary<T, bool> visited)
        {
            var alreadyVisited = visited.TryGetValue(item, out var inProcess);

            if (alreadyVisited)
            {
                if (inProcess)
                {
                    throw new ArgumentException("Cyclic dependency found.");
                }
            }
            else
            {
                visited[item] = true;

                var dependencies = getDependencies(item);
                if (dependencies != null)
                {
                    foreach (var dependency in dependencies)
                    {
                        Visit(dependency, getDependencies, sorted, visited);
                    }
                }

                visited[item] = false;
                sorted.Add(item);
            }
        }
    }
}


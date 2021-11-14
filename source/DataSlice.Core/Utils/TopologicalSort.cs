using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSlice.Core.Utils
{
    public static class TopologicalSort
    {
        public static IEnumerable<T> SortTopologically<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> dependencies, bool throwOnCycle = false)
        {
            var sorted = new List<T>();
            var visited = new HashSet<T>();

            foreach (var item in source)
            {
                if ((item as Table).Schema.Equals("OAUTH", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("");
                }
                Visit(item, visited, sorted, dependencies, throwOnCycle);
            }


            return sorted;
        }

        private static void Visit<T>(T item, HashSet<T> visited, List<T> sorted, Func<T, IEnumerable<T>> dependencies, bool throwOnCycle)
        {
            if (!visited.Contains(item))
            {
                visited.Add(item);

                foreach (var dep in dependencies(item))
                    Visit(dep, visited, sorted, dependencies, throwOnCycle);

                sorted.Add(item);
            }
            else
            {
                if (throwOnCycle && !sorted.Contains(item))
                    throw new Exception("Cyclic dependency found");
            }
        }
    }

    public class DependencyObject
    {
        public string Value { get; set; }

        public List<DependencyObject> Dependencies { get; set; }

        public DependencyObject()
        {
            Dependencies = new List<DependencyObject>();
        }
    }


}

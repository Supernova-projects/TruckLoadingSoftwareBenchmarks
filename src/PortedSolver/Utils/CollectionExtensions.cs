using System;
using System.Collections.Generic;
using System.Text;

namespace PortedSolver.Utils
{
    public static class CollectionExtensions
    {
        public static void SwapIndexes<TItem>(this IList<TItem> collection, int firstIndex, int secondIndex)
        {
            var temp = collection[firstIndex];
            collection[firstIndex] = collection[secondIndex];
            collection[secondIndex] = temp;
        }
    }
}

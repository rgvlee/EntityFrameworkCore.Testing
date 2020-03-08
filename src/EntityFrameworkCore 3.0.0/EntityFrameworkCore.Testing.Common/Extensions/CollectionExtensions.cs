using System.Collections.Generic;

namespace EntityFrameworkCore.Testing.Common.Extensions
{
    public static class CollectionExtensions
    {
        public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                yield return item;
            }
        }
    }
}
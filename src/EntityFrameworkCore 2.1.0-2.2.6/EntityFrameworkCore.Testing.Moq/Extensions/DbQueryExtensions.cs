using System.Collections.Generic;
using System.Linq;
using EntityFrameworkCore.Testing.Common.Helpers;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Testing.Moq.Extensions
{
    /// <summary>
    ///     Extensions for db queries.
    /// </summary>
    public static partial class DbQueryExtensions
    {
        /// <summary>
        ///     Adds an item to the end of the mocked db query source.
        /// </summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="mockedDbQuery">The mocked db query.</param>
        /// <param name="item">The item to be added to the end of the mocked db query source.</param>
        public static void AddToReadOnlySource<TQuery>(this DbQuery<TQuery> mockedDbQuery, TQuery item) where TQuery : class
        {
            EnsureArgument.IsNotNull(mockedDbQuery, nameof(mockedDbQuery));
            EnsureArgument.IsNotNull(item, nameof(item));

            var list = mockedDbQuery.ToList();
            list.Add(item);
            var queryable = list.AsQueryable();

            mockedDbQuery.SetSource(queryable);
        }

        /// <summary>
        ///     Adds the items of the specified sequence to the end of the mocked db query source.
        /// </summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="mockedDbQuery">The mocked db query.</param>
        /// <param name="items">The sequence whose items should be added to the end of the mocked db query source.</param>
        public static void AddRangeToReadOnlySource<TQuery>(this DbQuery<TQuery> mockedDbQuery, IEnumerable<TQuery> items) where TQuery : class
        {
            EnsureArgument.IsNotNull(mockedDbQuery, nameof(mockedDbQuery));
            EnsureArgument.IsNotEmpty(items, nameof(items));

            var list = mockedDbQuery.ToList();
            list.AddRange(items);
            var queryable = list.AsQueryable();

            mockedDbQuery.SetSource(queryable);
        }

        /// <summary>
        ///     Removes all items from the mocked db query source.
        /// </summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="mockedDbQuery">The mocked db query.</param>
        public static void ClearReadOnlySource<TQuery>(this DbQuery<TQuery> mockedDbQuery) where TQuery : class
        {
            EnsureArgument.IsNotNull(mockedDbQuery, nameof(mockedDbQuery));
            mockedDbQuery.SetSource(new List<TQuery>());
        }
    }
}
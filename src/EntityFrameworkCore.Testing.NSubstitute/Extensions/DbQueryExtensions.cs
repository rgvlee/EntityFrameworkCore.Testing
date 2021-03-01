using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using rgvlee.Core.Common.Helpers;

namespace EntityFrameworkCore.Testing.NSubstitute.Extensions
{
    /// <summary>
    ///     Extensions for db queries.
    /// </summary>
    public static partial class DbQueryExtensions
    {
        /// <summary>
        ///     Adds an item to the end of the mocked db query source.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="mockedDbQuery">The mocked db query.</param>
        /// <param name="item">The item to be added to the end of the mocked db query source.</param>
        public static void AddToReadOnlySource<TEntity>(this DbQuery<TEntity> mockedDbQuery, TEntity item) where TEntity : class
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
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="mockedDbQuery">The mocked db query.</param>
        /// <param name="items">The sequence whose items should be added to the end of the mocked db query source.</param>
        public static void AddRangeToReadOnlySource<TEntity>(this DbQuery<TEntity> mockedDbQuery, IEnumerable<TEntity> items) where TEntity : class
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
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="mockedDbQuery">The mocked db query.</param>
        public static void ClearReadOnlySource<TEntity>(this DbQuery<TEntity> mockedDbQuery) where TEntity : class
        {
            EnsureArgument.IsNotNull(mockedDbQuery, nameof(mockedDbQuery));
            mockedDbQuery.SetSource(new List<TEntity>());
        }
    }
}
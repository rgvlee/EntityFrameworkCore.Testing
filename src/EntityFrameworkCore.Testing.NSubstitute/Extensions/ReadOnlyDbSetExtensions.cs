using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using rgvlee.Core.Common.Helpers;

namespace EntityFrameworkCore.Testing.NSubstitute.Extensions
{
    /// <summary>
    ///     Extensions for read-only db sets.
    /// </summary>
    public static partial class ReadOnlyDbSetExtensions
    {
        /// <summary>
        ///     Adds an item to the end of the mocked readonly db set source.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="mockedReadOnlyDbSet">The mocked readonly db set.</param>
        /// <param name="item">The item to be added to the end of the mocked readonly db set source.</param>
        public static void AddToReadOnlySource<TEntity>(this DbQuery<TEntity> mockedReadOnlyDbSet, TEntity item) where TEntity : class
        {
            EnsureArgument.IsNotNull(mockedReadOnlyDbSet, nameof(mockedReadOnlyDbSet));
            ((DbSet<TEntity>) mockedReadOnlyDbSet).AddToReadOnlySource(item);
        }

        /// <summary>
        ///     Adds an item to the end of the mocked readonly db set source.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="mockedReadOnlyDbSet">The mocked readonly db set.</param>
        /// <param name="item">The item to be added to the end of the mocked readonly db set source.</param>
        public static void AddToReadOnlySource<TEntity>(this DbSet<TEntity> mockedReadOnlyDbSet, TEntity item) where TEntity : class
        {
            EnsureArgument.IsNotNull(mockedReadOnlyDbSet, nameof(mockedReadOnlyDbSet));
            EnsureArgument.IsNotNull(item, nameof(item));

            var list = mockedReadOnlyDbSet.ToList();
            list.Add(item);

            mockedReadOnlyDbSet.SetSource(list);
        }

        /// <summary>
        ///     Adds the items of the specified sequence to the end of the mocked readonly db set source.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="mockedReadOnlyDbSet">The mocked readonly db set.</param>
        /// <param name="items">The sequence whose items should be added to the end of the mocked readonly db set source.</param>
        public static void AddRangeToReadOnlySource<TEntity>(this DbQuery<TEntity> mockedReadOnlyDbSet, IEnumerable<TEntity> items) where TEntity : class
        {
            EnsureArgument.IsNotNull(mockedReadOnlyDbSet, nameof(mockedReadOnlyDbSet));
            ((DbSet<TEntity>) mockedReadOnlyDbSet).AddRangeToReadOnlySource(items);
        }

        /// <summary>
        ///     Adds the items of the specified sequence to the end of the mocked readonly db set source.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="mockedReadOnlyDbSet">The mocked readonly db set.</param>
        /// <param name="items">The sequence whose items should be added to the end of the mocked readonly db set source.</param>
        public static void AddRangeToReadOnlySource<TEntity>(this DbSet<TEntity> mockedReadOnlyDbSet, IEnumerable<TEntity> items) where TEntity : class
        {
            EnsureArgument.IsNotNull(mockedReadOnlyDbSet, nameof(mockedReadOnlyDbSet));
            EnsureArgument.IsNotEmpty(items, nameof(items));

            var list = mockedReadOnlyDbSet.ToList();
            list.AddRange(items);

            mockedReadOnlyDbSet.SetSource(list);
        }

        /// <summary>
        ///     Removes all items from the mocked readonly db set source.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="mockedReadOnlyDbSet">The mocked readonly db set.</param>
        public static void ClearReadOnlySource<TEntity>(this DbQuery<TEntity> mockedReadOnlyDbSet) where TEntity : class
        {
            EnsureArgument.IsNotNull(mockedReadOnlyDbSet, nameof(mockedReadOnlyDbSet));
            ((DbSet<TEntity>) mockedReadOnlyDbSet).ClearReadOnlySource();
        }

        /// <summary>
        ///     Removes all items from the mocked readonly db set source.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="mockedReadOnlyDbSet">The mocked readonly db set.</param>
        public static void ClearReadOnlySource<TEntity>(this DbSet<TEntity> mockedReadOnlyDbSet) where TEntity : class
        {
            EnsureArgument.IsNotNull(mockedReadOnlyDbSet, nameof(mockedReadOnlyDbSet));
            mockedReadOnlyDbSet.SetSource(new List<TEntity>());
        }
    }
}
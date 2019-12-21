using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Testing.NSubstitute.Extensions
{
    /// <summary>Extensions for the db query type.</summary>
    public static partial class DbQueryExtensions
    {
        /// <summary>Creates and sets up a substitute db query.</summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="dbQuery">The db query to mock.</param>
        /// <returns>A substitute db query.</returns>
        [Obsolete("This will be removed in a future version. Use DbQueryExtensions.CreateDbQuerySubstitute instead.")]
        public static DbQuery<TQuery> CreateMock<TQuery>(this DbQuery<TQuery> dbQuery)
            where TQuery : class
        {
            return dbQuery.CreateSubstituteDbQuery();
        }

        /// <summary>Adds an item to the end of the substitute db query source.</summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="substituteDbQuery">The substitute db query.</param>
        /// <param name="item">The item to be added to the end of the substitute db query source.</param>
        [Obsolete(
            "This has been replaced by DbQueryExtensions.AddToReadOnlySource to avoid conflicts with the " +
            "EntityFrameworkCore 3.0.0 read only set Add method.")
        ]
        public static void Add<TQuery>(this DbQuery<TQuery> substituteDbQuery, TQuery item)
            where TQuery : class
        {
            substituteDbQuery.AddToReadOnlySource(item);
        }

        /// <summary>Adds the items of the specified sequence to the end of the substitute db query source.</summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="substituteDbQuery">The substitute db query.</param>
        /// <param name="items">The sequence whose items should be added to the end of the substitute db query source.</param>
        [Obsolete(
            "This has been replaced by DbQueryExtensions.AddRangeToReadOnlySource to avoid conflicts with the " +
            "EntityFrameworkCore 3.0.0 read only set AddRange method.")
        ]
        public static void AddRange<TQuery>(this DbQuery<TQuery> substituteDbQuery, IEnumerable<TQuery> items)
            where TQuery : class
        {
            substituteDbQuery.AddRangeToReadOnlySource(items);
        }

        /// <summary>Removes all items from the substitute db query source.</summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="substituteDbQuery">The substitute db query.</param>
        [Obsolete("This has been replaced by DbQueryExtensions.ClearReadOnlySource.")]
        public static void Clear<TQuery>(this DbQuery<TQuery> substituteDbQuery)
            where TQuery : class
        {
            substituteDbQuery.ClearReadOnlySource();
        }
    }
}
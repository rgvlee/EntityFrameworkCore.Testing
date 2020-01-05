using System;
using System.Collections.Generic;
using EntityFrameworkCore.Testing.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Testing.Moq.Extensions
{
    /// <summary>Extensions for the db query type.</summary>
    public static partial class DbQueryExtensions
    {
        /// <summary>Creates and sets up a mocked db query.</summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="dbQuery">The db query to mock.</param>
        /// <returns>A mocked db query.</returns>
        [Obsolete("This will be removed in a future version. Use EntityFrameworkCore.Testing.Moq.Create.MockedDbContextFor with the params object[] parameter instead.")]
        public static DbQuery<TQuery> CreateMock<TQuery>(this DbQuery<TQuery> dbQuery)
            where TQuery : class
        {
            EnsureArgument.IsNotNull(dbQuery, nameof(dbQuery));
            return dbQuery.CreateMockedDbQuery();
        }

        /// <summary>Adds an item to the end of the mocked db query source.</summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="mockedDbQuery">The mocked db query.</param>
        /// <param name="item">The item to be added to the end of the mocked db query source.</param>
        [Obsolete(
            "This has been replaced by DbQueryExtensions.AddToReadOnlySource to avoid conflicts with the " +
            "EntityFrameworkCore 3.0.0 read only set Add method.")
        ]
        public static void Add<TQuery>(this DbQuery<TQuery> mockedDbQuery, TQuery item)
            where TQuery : class
        {
            EnsureArgument.IsNotNull(mockedDbQuery, nameof(mockedDbQuery));
            mockedDbQuery.AddToReadOnlySource(item);
        }

        /// <summary>Adds the items of the specified sequence to the end of the mocked db query source.</summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="mockedDbQuery">The mocked db query.</param>
        /// <param name="items">The sequence whose items should be added to the end of the mocked db query source.</param>
        [Obsolete(
            "This has been replaced by DbQueryExtensions.AddRangeToReadOnlySource to avoid conflicts with the " +
            "EntityFrameworkCore 3.0.0 read only set AddRange method.")
        ]
        public static void AddRange<TQuery>(this DbQuery<TQuery> mockedDbQuery, IEnumerable<TQuery> items)
            where TQuery : class
        {
            EnsureArgument.IsNotNull(mockedDbQuery, nameof(mockedDbQuery));
            mockedDbQuery.AddRangeToReadOnlySource(items);
        }

        /// <summary>Removes all items from the mocked db query source.</summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="mockedDbQuery">The mocked db query.</param>
        [Obsolete("This has been replaced by DbQueryExtensions.ClearReadOnlySource.")]
        public static void Clear<TQuery>(this DbQuery<TQuery> mockedDbQuery)
            where TQuery : class
        {
            EnsureArgument.IsNotNull(mockedDbQuery, nameof(mockedDbQuery));
            mockedDbQuery.ClearReadOnlySource();
        }
    }
}
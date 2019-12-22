using System;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Testing.NSubstitute.Extensions
{
    /// <summary>Extensions for read-only db sets.</summary>
    public static partial class ReadOnlyDbSetExtensions
    {
        /// <summary>Creates and sets up a mocked db query.</summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="dbQuery">The db query to mock.</param>
        /// <returns>A mocked readonly db query.</returns>
        [Obsolete("This method will remain until EntityFrameworkCore no longer supports the DbQuery<TQuery> type. Use ReadOnlyDbSetExtensions.CreateMockedReadOnlyDbSet instead.")]
        public static DbQuery<TQuery> CreateMockedDbQuery<TQuery>(this DbQuery<TQuery> dbQuery)
            where TQuery : class
        {
            var mockedReadOnlyDbSet = dbQuery.CreateMockedReadOnlyDbSet();
            return (DbQuery<TQuery>) mockedReadOnlyDbSet;
        }
    }
}
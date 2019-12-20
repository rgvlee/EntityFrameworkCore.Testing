using System;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Testing.NSubstitute.Extensions
{
    /// <summary>Extensions for read-only db sets.</summary>
    public static partial class ReadOnlyDbSetExtensions
    {
        /// <summary>Creates and sets up a substitute db query.</summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="dbQuery">The db query to mock.</param>
        /// <returns>A substitute readonly db query.</returns>
        [Obsolete("This method will remain until EntityFrameworkCore no longer supports the DbQuery<TQuery> type. Use ReadOnlyDbSetExtensions.CreateSubstituteReadOnlyDbSet instead.")]
        public static DbQuery<TQuery> CreateSubstituteDbQuery<TQuery>(this DbQuery<TQuery> dbQuery)
            where TQuery : class
        {
            var substituteReadOnlyDbSet = dbQuery.CreateSubstituteReadOnlyDbSet();
            return (DbQuery<TQuery>) substituteReadOnlyDbSet;
        }
    }
}
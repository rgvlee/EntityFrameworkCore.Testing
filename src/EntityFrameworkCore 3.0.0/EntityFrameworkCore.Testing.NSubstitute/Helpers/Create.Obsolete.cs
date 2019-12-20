using System;
using EntityFrameworkCore.Testing.Common;
using EntityFrameworkCore.Testing.NSubstitute.Extensions;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Testing.NSubstitute.Helpers
{
    /// <summary>Factory for creating mocked instances.</summary>
    public static partial class Create
    {
        /// <summary>Creates a substitute db query.</summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="dbQueryToMock">The db query to mock.</param>
        /// <returns>A substitute db query.</returns>
        [Obsolete("This method will remain until EntityFrameworkCore no longer supports the DbQuery<TQuery> type. Use Create.SubstituteReadOnlyDbSetFor instead.")]
        public static DbQuery<TQuery> SubstituteDbQueryFor<TQuery>(DbQuery<TQuery> dbQueryToMock)
            where TQuery : class
        {
            EnsureArgument.IsNotNull(dbQueryToMock, nameof(dbQueryToMock));

            return dbQueryToMock.CreateSubstituteDbQuery();
        }
    }
}
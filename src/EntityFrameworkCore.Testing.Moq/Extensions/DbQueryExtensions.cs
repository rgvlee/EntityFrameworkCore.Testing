using System.Collections.Generic;
using EntityFrameworkCore.Testing.Moq.Helpers;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace EntityFrameworkCore.Testing.Moq.Extensions
{
    /// <summary>
    /// Extensions for db queries.
    /// </summary>
    public static class DbQueryExtensions
    {
        /// <summary>
        /// Creates and sets up a DbQuery mock for the specified entity.
        /// </summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="dbQuery">The DbQuery to mock.</param>
        /// <param name="sequence">The sequence to use for the DbQuery.</param>
        /// <returns>A DbQuery mock for the specified entity.</returns>
        public static Mock<DbQuery<TQuery>> CreateDbQueryMock<TQuery>(this DbQuery<TQuery> dbQuery,
            IEnumerable<TQuery> sequence)
            where TQuery : class
        {
            return DbQueryHelper.CreateDbQueryMock(sequence);
        }
    }
}
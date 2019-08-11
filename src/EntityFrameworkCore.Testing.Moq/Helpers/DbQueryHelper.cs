using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EntityFrameworkCore.Testing.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Moq;

namespace EntityFrameworkCore.Testing.Moq.Helpers
{
    /// <summary>
    /// Helper methods for db queries.
    /// </summary>
    public class DbQueryHelper
    {
        /// <summary>
        /// Creates and sets up a DbQuery mock for the specified entity.
        /// </summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="sequence">The sequence to use for the DbQuery.</param>
        /// <returns>A DbQuery mock for the specified entity.</returns>
        public static Mock<DbQuery<TQuery>> CreateDbQueryMock<TQuery>(IEnumerable<TQuery> sequence)
            where TQuery : class
        {
            var dbQueryMock = new Mock<DbQuery<TQuery>>();
            var queryableSequence = sequence.AsQueryable();

            dbQueryMock.As<IAsyncEnumerableAccessor<TQuery>>().Setup(m => m.AsyncEnumerable)
                .Returns(queryableSequence.ToAsyncEnumerable());
            dbQueryMock.As<IQueryable<TQuery>>().Setup(m => m.ElementType)
                .Returns(queryableSequence.ElementType); //.Callback(() => Logger.LogDebug("ElementType invoked"));
            dbQueryMock.As<IQueryable<TQuery>>().Setup(m => m.Expression)
                .Returns(queryableSequence.Expression); //.Callback(() => Logger.LogDebug("Expression invoked"));
            dbQueryMock.As<IEnumerable>().Setup(m => m.GetEnumerator())
                .Returns(queryableSequence
                    .GetEnumerator()); //.Callback(() => Logger.LogDebug("IEnumerable.GetEnumerator invoked"));
            dbQueryMock.As<IEnumerable<TQuery>>().Setup(m => m.GetEnumerator())
                .Returns(queryableSequence
                    .GetEnumerator()); //.Callback(() => Logger.LogDebug("IEnumerable<TQuery>.GetEnumerator invoked"));

            dbQueryMock.As<IInfrastructure<IServiceProvider>>().Setup(m => m.Instance).Returns(() =>
                ((IInfrastructure<IServiceProvider>) dbQueryMock.Object)
                .Instance); //.Callback(() => Logger.LogDebug("Instance invoked"));

            //We need to provide a provider that implement IAsyncQueryProvider
            dbQueryMock.As<IQueryable<TQuery>>().Setup(m => m.Provider)
                .Returns(new AsyncQueryProvider<TQuery>(
                    queryableSequence)); //.Callback(() => Logger.LogDebug("Provider invoked"));

            return dbQueryMock;
        }
    }
}
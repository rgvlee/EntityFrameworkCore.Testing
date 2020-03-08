using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EntityFrameworkCore.Testing.Common;
using EntityFrameworkCore.Testing.Common.Helpers;
using Microsoft.Extensions.Logging;
using Moq;

namespace EntityFrameworkCore.Testing.Moq.Extensions
{
    public static partial class QueryProviderExtensions
    {
        internal static IQueryProvider CreateMockedQueryProvider<T>(this IQueryProvider queryProviderToMock, IEnumerable<T> collection) where T : class
        {
            EnsureArgument.IsNotNull(queryProviderToMock, nameof(queryProviderToMock));
            EnsureArgument.IsNotNull(collection, nameof(collection));

            var queryProviderMock = new Mock<AsyncQueryProvider<T>>(collection.AsQueryable());
            queryProviderMock.CallBase = true;

            queryProviderMock.As<IQueryProvider>()
                .Setup(m => m.CreateQuery<T>(It.Is<MethodCallExpression>(mce => mce.Method.Name.Equals("FromSqlOnQueryable"))))
                .Callback((Expression providedExpression) =>
                {
                    Logger.LogDebug("Catch all exception invoked");
                })
                .Throws<NotSupportedException>();

            return queryProviderMock.Object;
        }

        internal static void SetSource<T>(this AsyncQueryProvider<T> mockedQueryProvider, IEnumerable<T> source) where T : class
        {
            EnsureArgument.IsNotNull(mockedQueryProvider, nameof(mockedQueryProvider));
            EnsureArgument.IsNotNull(source, nameof(source));

            var queryProviderMock = Mock.Get(mockedQueryProvider);

            var queryable = source.AsQueryable();
            queryProviderMock.Setup(m => m.Source).Returns(queryable);
        }
    }
}
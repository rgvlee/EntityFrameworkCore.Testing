using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EntityFrameworkCore.Testing.Common;
using EntityFrameworkCore.Testing.Common.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.Extensions;

namespace EntityFrameworkCore.Testing.NSubstitute.Extensions
{
    public static partial class QueryProviderExtensions
    {
        internal static IQueryProvider CreateMockedQueryProvider<T>(this IQueryProvider queryProviderToMock, IEnumerable<T> collection) where T : class
        {
            EnsureArgument.IsNotNull(queryProviderToMock, nameof(queryProviderToMock));
            EnsureArgument.IsNotNull(collection, nameof(collection));

            var mockedQueryProvider = Substitute.ForPartsOf<AsyncQueryProvider<T>>(collection.AsQueryable());

            mockedQueryProvider.Configure()
                .CreateQuery<T>(Arg.Is<MethodCallExpression>(mce => mce.Method.Name.Equals(nameof(RelationalQueryableExtensions.FromSql))))
                .Throws(callInfo =>
                {
                    Logger.LogDebug("Catch all exception invoked");
                    return new NotSupportedException();
                });

            return mockedQueryProvider;
        }

        internal static void SetSource<T>(this AsyncQueryProvider<T> mockedQueryProvider, IEnumerable<T> source) where T : class
        {
            EnsureArgument.IsNotNull(mockedQueryProvider, nameof(mockedQueryProvider));
            EnsureArgument.IsNotNull(source, nameof(source));

            var queryable = source.AsQueryable();
            mockedQueryProvider.Configure().Source.Returns(callInfo => queryable);
        }
    }
}
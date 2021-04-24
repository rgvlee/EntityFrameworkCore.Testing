using System.Collections.Generic;
using System.Linq;
using EntityFrameworkCore.Testing.Common;
using Moq;
using rgvlee.Core.Common.Helpers;

namespace EntityFrameworkCore.Testing.Moq.Extensions
{
    public static partial class QueryProviderExtensions
    {
        internal static IQueryProvider CreateMockedQueryProvider<T>(this IQueryProvider queryProviderToMock, IEnumerable<T> collection) where T : class
        {
            EnsureArgument.IsNotNull(queryProviderToMock, nameof(queryProviderToMock));
            EnsureArgument.IsNotNull(collection, nameof(collection));

            var queryProviderMock = new Mock<AsyncQueryProvider<T>>(collection);
            queryProviderMock.CallBase = true;
            return queryProviderMock.Object;
        }

        internal static void SetSource<T>(this AsyncQueryProvider<T> mockedQueryProvider, IEnumerable<T> source) where T : class
        {
            EnsureArgument.IsNotNull(mockedQueryProvider, nameof(mockedQueryProvider));
            EnsureArgument.IsNotNull(source, nameof(source));

            mockedQueryProvider.Source = source.AsQueryable();
        }
    }
}
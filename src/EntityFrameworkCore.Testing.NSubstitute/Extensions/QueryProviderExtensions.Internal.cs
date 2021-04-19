using System.Collections.Generic;
using System.Linq;
using EntityFrameworkCore.Testing.Common;
using NSubstitute;
using NSubstitute.Extensions;
using rgvlee.Core.Common.Helpers;

namespace EntityFrameworkCore.Testing.NSubstitute.Extensions
{
    public static partial class QueryProviderExtensions
    {
        internal static IQueryProvider CreateMockedQueryProvider<T>(this IQueryProvider queryProviderToMock, IEnumerable<T> collection) where T : class
        {
            EnsureArgument.IsNotNull(queryProviderToMock, nameof(queryProviderToMock));
            EnsureArgument.IsNotNull(collection, nameof(collection));

            var mockedQueryProvider = Substitute.ForPartsOf<AsyncQueryProvider<T>>(collection);
            return mockedQueryProvider;
        }

        internal static void SetSource<T>(this AsyncQueryProvider<T> mockedQueryProvider, IEnumerable<T> source) where T : class
        {
            EnsureArgument.IsNotNull(mockedQueryProvider, nameof(mockedQueryProvider));
            EnsureArgument.IsNotNull(source, nameof(source));

            mockedQueryProvider.Source = source.AsQueryable();
        }
    }
}
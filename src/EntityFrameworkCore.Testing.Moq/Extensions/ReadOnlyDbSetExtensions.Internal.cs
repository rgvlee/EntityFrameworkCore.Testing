using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using EntityFrameworkCore.Testing.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Moq;
using rgvlee.Core.Common.Helpers;

namespace EntityFrameworkCore.Testing.Moq.Extensions
{
    public static partial class ReadOnlyDbSetExtensions
    {
        internal static DbSet<TEntity> CreateMockedReadOnlyDbSet<TEntity>(this DbSet<TEntity> readOnlyDbSet) where TEntity : class
        {
            EnsureArgument.IsNotNull(readOnlyDbSet, nameof(readOnlyDbSet));

            var readOnlyDbSetMock = new Mock<DbSet<TEntity>>();

            var queryable = new List<TEntity>().AsQueryable();
            var asyncEnumerable = new AsyncEnumerable<TEntity>(queryable);
            var mockedQueryProvider = ((IQueryable<TEntity>) readOnlyDbSet).Provider.CreateMockedQueryProvider(asyncEnumerable);

            var invalidOperationException = new InvalidOperationException(
                $"Unable to track an instance of type '{typeof(TEntity).Name}' because it does not have a primary key. Only entity types with primary keys may be tracked.");

            readOnlyDbSetMock.Setup(m => m.Add(It.IsAny<TEntity>())).Throws(invalidOperationException);
            readOnlyDbSetMock.Setup(m => m.AddAsync(It.IsAny<TEntity>(), It.IsAny<CancellationToken>())).Throws(invalidOperationException);
            readOnlyDbSetMock.Setup(m => m.AddRange(It.IsAny<IEnumerable<TEntity>>())).Throws(invalidOperationException);
            readOnlyDbSetMock.Setup(m => m.AddRange(It.IsAny<TEntity[]>())).Throws(invalidOperationException);
            readOnlyDbSetMock.Setup(m => m.AddRangeAsync(It.IsAny<IEnumerable<TEntity>>(), It.IsAny<CancellationToken>())).Throws(invalidOperationException);
            readOnlyDbSetMock.Setup(m => m.AddRangeAsync(It.IsAny<TEntity[]>())).Throws(invalidOperationException);

            readOnlyDbSetMock.Setup(m => m.Attach(It.IsAny<TEntity>())).Throws(invalidOperationException);
            readOnlyDbSetMock.Setup(m => m.AttachRange(It.IsAny<IEnumerable<TEntity>>())).Throws(invalidOperationException);
            readOnlyDbSetMock.Setup(m => m.AttachRange(It.IsAny<TEntity[]>())).Throws(invalidOperationException);

            readOnlyDbSetMock.As<IListSource>().Setup(m => m.ContainsListCollection).Returns(false);

            readOnlyDbSetMock.As<IQueryable<TEntity>>().Setup(m => m.ElementType).Returns(asyncEnumerable.ElementType);
            readOnlyDbSetMock.As<IQueryable<TEntity>>().Setup(m => m.Expression).Returns(asyncEnumerable.Expression);

            readOnlyDbSetMock.Setup(m => m.Find(It.IsAny<object[]>())).Throws(new NullReferenceException());
            readOnlyDbSetMock.Setup(m => m.FindAsync(It.IsAny<object[]>())).Throws(new NullReferenceException());
            readOnlyDbSetMock.Setup(m => m.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>())).Throws(new NullReferenceException());

            readOnlyDbSetMock.As<IAsyncEnumerable<TEntity>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns((CancellationToken providedCancellationToken) => asyncEnumerable.GetAsyncEnumerator(providedCancellationToken));

            readOnlyDbSetMock.As<IEnumerable>().Setup(m => m.GetEnumerator()).Returns(((IEnumerable) asyncEnumerable).GetEnumerator());
            readOnlyDbSetMock.As<IEnumerable<TEntity>>().Setup(m => m.GetEnumerator()).Returns(((IEnumerable<TEntity>) asyncEnumerable).GetEnumerator());

            readOnlyDbSetMock.As<IListSource>().Setup(m => m.GetList()).Returns(asyncEnumerable.ToList());

            readOnlyDbSetMock.As<IInfrastructure<IServiceProvider>>().Setup(m => m.Instance).Returns(((IInfrastructure<IServiceProvider>) readOnlyDbSet).Instance);

            readOnlyDbSetMock.Setup(m => m.Local)
                .Throws(new InvalidOperationException($"The invoked method cannot be used for the entity type '{typeof(TEntity).Name}' because it does not have a primary key."));

            readOnlyDbSetMock.Setup(m => m.Remove(It.IsAny<TEntity>())).Throws(invalidOperationException);
            readOnlyDbSetMock.Setup(m => m.RemoveRange(It.IsAny<IEnumerable<TEntity>>())).Throws(invalidOperationException);
            readOnlyDbSetMock.Setup(m => m.RemoveRange(It.IsAny<TEntity[]>())).Throws(invalidOperationException);

            readOnlyDbSetMock.Setup(m => m.Update(It.IsAny<TEntity>())).Throws(invalidOperationException);
            readOnlyDbSetMock.Setup(m => m.UpdateRange(It.IsAny<IEnumerable<TEntity>>())).Throws(invalidOperationException);
            readOnlyDbSetMock.Setup(m => m.UpdateRange(It.IsAny<TEntity[]>())).Throws(invalidOperationException);

            readOnlyDbSetMock.As<IQueryable<TEntity>>().Setup(m => m.Provider).Returns(mockedQueryProvider);

            readOnlyDbSetMock.Setup(m => m.AsAsyncEnumerable()).Returns(asyncEnumerable);
            readOnlyDbSetMock.Setup(m => m.AsQueryable()).Returns(asyncEnumerable);

            return readOnlyDbSetMock.Object;
        }

        internal static void SetSource<TEntity>(this DbSet<TEntity> mockedReadOnlyDbSet, IEnumerable<TEntity> source) where TEntity : class
        {
            EnsureArgument.IsNotNull(mockedReadOnlyDbSet, nameof(mockedReadOnlyDbSet));
            EnsureArgument.IsNotNull(source, nameof(source));

            var readOnlyDbSetMock = Mock.Get(mockedReadOnlyDbSet);

            var queryable = source.AsQueryable();
            var asyncEnumerable = new AsyncEnumerable<TEntity>(queryable);

            readOnlyDbSetMock.As<IQueryable<TEntity>>().Setup(m => m.Expression).Returns(asyncEnumerable.Expression);

            readOnlyDbSetMock.As<IAsyncEnumerable<TEntity>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns((CancellationToken providedCancellationToken) => asyncEnumerable.GetAsyncEnumerator(providedCancellationToken));

            readOnlyDbSetMock.As<IEnumerable>().Setup(m => m.GetEnumerator()).Returns(((IEnumerable) asyncEnumerable).GetEnumerator());
            readOnlyDbSetMock.As<IEnumerable<TEntity>>().Setup(m => m.GetEnumerator()).Returns(((IEnumerable<TEntity>) asyncEnumerable).GetEnumerator());

            var provider = ((IQueryable<TEntity>) mockedReadOnlyDbSet).Provider;
            ((AsyncQueryProvider<TEntity>) provider).SetSource(asyncEnumerable);

            readOnlyDbSetMock.Setup(m => m.AsAsyncEnumerable()).Returns(asyncEnumerable);
            readOnlyDbSetMock.Setup(m => m.AsQueryable()).Returns(asyncEnumerable);
        }
    }
}
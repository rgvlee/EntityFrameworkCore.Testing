using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EntityFrameworkCore.Testing.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Moq;

namespace EntityFrameworkCore.Testing.Moq.Extensions
{
    /// <summary>Extensions for the db set type.</summary>
    public static class ReadOnlyDbSetExtensions
    {
        /// <summary>Creates and sets up a mocked readonly db set.</summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="readOnlyDbSet">The readonly db set to mock.</param>
        /// <returns>A mocked readonly db set.</returns>
        public static DbSet<TEntity> CreateReadOnlyMock<TEntity>(this DbSet<TEntity> readOnlyDbSet) where TEntity : class
        {
            EnsureArgument.IsNotNull(readOnlyDbSet, nameof(readOnlyDbSet));

            //This is deliberate; we cannot cast a Mock<DbSet<>> to a Mock<DbQuery<>> and we still need to support the latter
            var readOnlyDbSetMock = new Mock<DbQuery<TEntity>>();

            var queryable = new List<TEntity>().AsQueryable();

            readOnlyDbSetMock.As<IQueryable<TEntity>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            readOnlyDbSetMock.As<IQueryable<TEntity>>().Setup(m => m.Expression).Returns(queryable.Expression);
            readOnlyDbSetMock.As<IAsyncEnumerable<TEntity>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns((CancellationToken providedCancellationToken) => ((IAsyncEnumerable<TEntity>) queryable).GetAsyncEnumerator(providedCancellationToken));
            readOnlyDbSetMock.As<IEnumerable>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
            readOnlyDbSetMock.As<IEnumerable<TEntity>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            readOnlyDbSetMock.As<IInfrastructure<IServiceProvider>>().Setup(m => m.Instance).Returns(((IInfrastructure<IServiceProvider>) readOnlyDbSet).Instance);

            var mockedQueryProvider = ((IQueryable<TEntity>) readOnlyDbSet).Provider.CreateMock(new List<TEntity>());
            readOnlyDbSetMock.As<IQueryable<TEntity>>().Setup(m => m.Provider).Returns(mockedQueryProvider);

            return readOnlyDbSetMock.Object;
        }

        /// <summary>Adds an item to the end of the mocked readonly db set source.</summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="mockedReadOnlyDbSet">The mocked readonly db set.</param>
        /// <param name="item">The item to be added to the end of the mocked readonly db set source.</param>
        public static void AddToReadOnlySource<TEntity>(this DbSet<TEntity> mockedReadOnlyDbSet, TEntity item) where TEntity : class
        {
            EnsureArgument.IsNotNull(mockedReadOnlyDbSet, nameof(mockedReadOnlyDbSet));
            EnsureArgument.IsNotNull(item, nameof(item));

            var list = mockedReadOnlyDbSet.ToList();
            list.Add(item);
            var queryable = list.AsQueryable();

            mockedReadOnlyDbSet.SetSource(queryable);
        }

        /// <summary>Adds the items of the specified sequence to the end of the mocked readonly db set source.</summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="mockedReadOnlyDbSet">The mocked readonly db set.</param>
        /// <param name="items">The sequence whose items should be added to the end of the mocked readonly db set source.</param>
        public static void AddRangeToReadOnlySource<TEntity>(this DbSet<TEntity> mockedReadOnlyDbSet, IEnumerable<TEntity> items) where TEntity : class
        {
            EnsureArgument.IsNotNull(mockedReadOnlyDbSet, nameof(mockedReadOnlyDbSet));
            EnsureArgument.IsNotNull(items, nameof(items));
            EnsureArgument.IsNotEmpty(items, nameof(items));

            var list = mockedReadOnlyDbSet.ToList();
            list.AddRange(items);
            var queryable = list.AsQueryable();

            mockedReadOnlyDbSet.SetSource(queryable);
        }

        /// <summary>Removes all items from the mocked readonly db set source.</summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="mockedReadOnlyDbSet">The mocked readonly db set.</param>
        public static void ClearReadOnlySource<TEntity>(this DbSet<TEntity> mockedReadOnlyDbSet) where TEntity : class
        {
            EnsureArgument.IsNotNull(mockedReadOnlyDbSet, nameof(mockedReadOnlyDbSet));

            mockedReadOnlyDbSet.SetSource(new List<TEntity>());
        }

        internal static void SetSource<TEntity>(this DbSet<TEntity> mockedReadOnlyDbSet, IEnumerable<TEntity> source) where TEntity : class
        {
            EnsureArgument.IsNotNull(mockedReadOnlyDbSet, nameof(mockedReadOnlyDbSet));
            EnsureArgument.IsNotNull(source, nameof(source));

            var readOnlyDbSetMock = Mock.Get((DbQuery<TEntity>) mockedReadOnlyDbSet);

            var queryable = source.AsQueryable();

            readOnlyDbSetMock.As<IQueryable<TEntity>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            readOnlyDbSetMock.As<IQueryable<TEntity>>().Setup(m => m.Expression).Returns(queryable.Expression);
            readOnlyDbSetMock.As<IAsyncEnumerable<TEntity>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns((CancellationToken providedCancellationToken) => ((IAsyncEnumerable<TEntity>) queryable).GetAsyncEnumerator(providedCancellationToken));
            readOnlyDbSetMock.As<IEnumerable>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
            readOnlyDbSetMock.As<IEnumerable<TEntity>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            var provider = ((IQueryable<TEntity>) mockedReadOnlyDbSet).Provider;
            ((AsyncQueryProvider<TEntity>) provider).SetSource(queryable);
        }
    }
}
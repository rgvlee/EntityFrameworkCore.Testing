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

namespace EntityFrameworkCore.Testing.Moq.Extensions
{
    /// <summary>Extensions for the db set type.</summary>
    public static class ReadOnlyDbSetExtensions
    {
        /// <summary>Creates and sets up a mocked db query.</summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="dbQuery">The db query to mock.</param>
        /// <returns>A mocked readonly db query.</returns>
        [Obsolete("This method will remain until EntityFrameworkCore no longer supports the DbQuery<TQuery> type. Use ReadOnlyDbSetExtensions.CreateMockedReadOnlyDbSet instead.")]
        public static DbQuery<TQuery> CreateMockedDbQuery<TQuery>(this DbQuery<TQuery> dbQuery) 
            where TQuery : class
        {
            var mockedReadOnlyDbSet = dbQuery.CreateMockedReadOnlyDbSet();
            return (DbQuery<TQuery>) mockedReadOnlyDbSet;
        }
        
        /// <summary>Creates and sets up a mocked readonly db set.</summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="readOnlyDbSet">The readonly db set to mock.</param>
        /// <returns>A mocked readonly db set.</returns>
        public static DbSet<TEntity> CreateMockedReadOnlyDbSet<TEntity>(this DbSet<TEntity> readOnlyDbSet) 
            where TEntity : class
        {
            EnsureArgument.IsNotNull(readOnlyDbSet, nameof(readOnlyDbSet));

            //This is deliberate; we cannot cast a Mock<DbSet<>> to a Mock<DbQuery<>> and we still need to support the latter
            var readOnlyDbSetMock = new Mock<DbQuery<TEntity>>();

            var queryable = new List<TEntity>().AsQueryable();

            var invalidOperationException = new InvalidOperationException($"Unable to track an instance of type '{typeof(TEntity).Name}' because it does not have a primary key. Only entity types with primary keys may be tracked.");

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

            readOnlyDbSetMock.As<IQueryable<TEntity>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            readOnlyDbSetMock.As<IQueryable<TEntity>>().Setup(m => m.Expression).Returns(queryable.Expression);

            readOnlyDbSetMock.Setup(m => m.Find(It.IsAny<object[]>())).Throws(new NullReferenceException());
            readOnlyDbSetMock.Setup(m => m.FindAsync(It.IsAny<object[]>())).Throws(new NullReferenceException());
            readOnlyDbSetMock.Setup(m => m.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>())).Throws(new NullReferenceException());

            readOnlyDbSetMock.As<IAsyncEnumerable<TEntity>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns((CancellationToken providedCancellationToken) => ((IAsyncEnumerable<TEntity>) queryable).GetAsyncEnumerator(providedCancellationToken));

            readOnlyDbSetMock.As<IEnumerable>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
            readOnlyDbSetMock.As<IEnumerable<TEntity>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            readOnlyDbSetMock.As<IListSource>().Setup(m => m.GetList()).Returns(queryable.ToList());

            readOnlyDbSetMock.As<IInfrastructure<IServiceProvider>>().Setup(m => m.Instance).Returns(((IInfrastructure<IServiceProvider>) readOnlyDbSet).Instance);

            readOnlyDbSetMock.Setup(m => m.Local).Throws(new InvalidOperationException($"The invoked method is cannot be used for the entity type '{typeof(TEntity).Name}' because it does not have a primary key."));

            readOnlyDbSetMock.Setup(m => m.Remove(It.IsAny<TEntity>())).Throws(invalidOperationException);
            readOnlyDbSetMock.Setup(m => m.RemoveRange(It.IsAny<IEnumerable<TEntity>>())).Throws(invalidOperationException);
            readOnlyDbSetMock.Setup(m => m.RemoveRange(It.IsAny<TEntity[]>())).Throws(invalidOperationException);

            readOnlyDbSetMock.Setup(m => m.Update(It.IsAny<TEntity>())).Throws(invalidOperationException);
            readOnlyDbSetMock.Setup(m => m.UpdateRange(It.IsAny<IEnumerable<TEntity>>())).Throws(invalidOperationException);
            readOnlyDbSetMock.Setup(m => m.UpdateRange(It.IsAny<TEntity[]>())).Throws(invalidOperationException);

            var mockedQueryProvider = ((IQueryable<TEntity>) readOnlyDbSet).Provider.CreateMockedQueryProvider(new List<TEntity>());
            readOnlyDbSetMock.As<IQueryable<TEntity>>().Setup(m => m.Provider).Returns(mockedQueryProvider);

            return readOnlyDbSetMock.Object;
        }

        /// <summary>Creates and sets up a mocked db query.</summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="dbQuery">The db query to mock.</param>
        /// <returns>A mocked readonly db query.</returns>
        [Obsolete("This will be removed in a future version. Use ReadOnlyDbSetExtensions.CreateMockedDbQuery instead.")]
        public static DbQuery<TQuery> CreateReadOnlyMock<TQuery>(this DbQuery<TQuery> dbQuery)
            where TQuery : class
        {
            return dbQuery.CreateMockedDbQuery();
        }

        /// <summary>Creates and sets up a mocked readonly db set.</summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="readOnlyDbSet">The readonly db set to mock.</param>
        /// <returns>A mocked readonly db set.</returns>
        [Obsolete("This will be removed in a future version. Use ReadOnlyDbSetExtensions.CreateMockedReadOnlyDbSet instead.")]
        public static DbSet<TEntity> CreateReadOnlyMock<TEntity>(this DbSet<TEntity> readOnlyDbSet)
            where TEntity : class
        {
            return readOnlyDbSet.CreateMockedReadOnlyDbSet();
        }

        /// <summary>Adds an item to the end of the mocked readonly db set source.</summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="mockedReadOnlyDbSet">The mocked readonly db set.</param>
        /// <param name="item">The item to be added to the end of the mocked readonly db set source.</param>
        public static void AddToReadOnlySource<TEntity>(this DbQuery<TEntity> mockedReadOnlyDbSet, TEntity item) 
            where TEntity : class
        {
            ((DbSet<TEntity>) mockedReadOnlyDbSet).AddToReadOnlySource(item);
        }

        /// <summary>Adds an item to the end of the mocked readonly db set source.</summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="mockedReadOnlyDbSet">The mocked readonly db set.</param>
        /// <param name="item">The item to be added to the end of the mocked readonly db set source.</param>
        public static void AddToReadOnlySource<TEntity>(this DbSet<TEntity> mockedReadOnlyDbSet, TEntity item) 
            where TEntity : class
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
        public static void AddRangeToReadOnlySource<TEntity>(this DbQuery<TEntity> mockedReadOnlyDbSet, IEnumerable<TEntity> items) 
            where TEntity : class
        {
            ((DbSet<TEntity>) mockedReadOnlyDbSet).AddRangeToReadOnlySource(items);
        }

        /// <summary>Adds the items of the specified sequence to the end of the mocked readonly db set source.</summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="mockedReadOnlyDbSet">The mocked readonly db set.</param>
        /// <param name="items">The sequence whose items should be added to the end of the mocked readonly db set source.</param>
        public static void AddRangeToReadOnlySource<TEntity>(this DbSet<TEntity> mockedReadOnlyDbSet, IEnumerable<TEntity> items) 
            where TEntity : class
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
        public static void ClearReadOnlySource<TEntity>(this DbQuery<TEntity> mockedReadOnlyDbSet) 
            where TEntity : class
        {
            ((DbSet<TEntity>) mockedReadOnlyDbSet).ClearReadOnlySource();
        }

        /// <summary>Removes all items from the mocked readonly db set source.</summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="mockedReadOnlyDbSet">The mocked readonly db set.</param>
        public static void ClearReadOnlySource<TEntity>(this DbSet<TEntity> mockedReadOnlyDbSet) 
            where TEntity : class
        {
            EnsureArgument.IsNotNull(mockedReadOnlyDbSet, nameof(mockedReadOnlyDbSet));

            mockedReadOnlyDbSet.SetSource(new List<TEntity>());
        }

        internal static void SetSource<TEntity>(this DbSet<TEntity> mockedReadOnlyDbSet, IEnumerable<TEntity> source) 
            where TEntity : class
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
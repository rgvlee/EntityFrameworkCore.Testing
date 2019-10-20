using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using EntityFrameworkCore.Testing.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Moq;

namespace EntityFrameworkCore.Testing.Moq.Extensions
{
    /// <summary>
    ///     Extensions for the db set type.
    /// </summary>
    public static class DbSetExtensions
    {
        /// <summary>
        ///     Creates and sets up a mocked db set.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="dbSet">The db set to mock/proxy.</param>
        /// <returns>A mocked db set.</returns>
        public static DbSet<TEntity> CreateMock<TEntity>(this DbSet<TEntity> dbSet) where TEntity : class
        {
            EnsureArgument.IsNotNull(dbSet, nameof(dbSet));

            var dbSetMock = new Mock<DbSet<TEntity>>();
            dbSetMock.SetUp(dbSet);
            return dbSetMock.Object;
        }

        /// <summary>
        ///     Sets up a db set mock.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="dbSetMock">The db set mock to set up.</param>
        /// <param name="dbSet">The db set to mock/proxy.</param>
        internal static void SetUp<TEntity>(this Mock<DbSet<TEntity>> dbSetMock, DbSet<TEntity> dbSet) where TEntity : class
        {
            EnsureArgument.IsNotNull(dbSetMock, nameof(dbSetMock));
            EnsureArgument.IsNotNull(dbSet, nameof(dbSet));

            dbSetMock.Setup(m => m.Add(It.IsAny<TEntity>())).Returns((TEntity entity) => dbSet.Add(entity));

            dbSetMock.Setup(m => m.AddAsync(It.IsAny<TEntity>(), It.IsAny<CancellationToken>()))
                .Returns((TEntity entity, CancellationToken cancellationToken) => dbSet.AddAsync(entity, cancellationToken));

            dbSetMock.Setup(m => m.AddRange(It.IsAny<IEnumerable<TEntity>>())).Callback((IEnumerable<TEntity> entities) => dbSet.AddRange(entities));
            dbSetMock.Setup(m => m.AddRange(It.IsAny<TEntity[]>())).Callback((TEntity[] entities) => dbSet.AddRange(entities));

            dbSetMock.Setup(m => m.AddRangeAsync(It.IsAny<IEnumerable<TEntity>>(), It.IsAny<CancellationToken>()))
                .Returns((IEnumerable<TEntity> entities, CancellationToken cancellationToken) => dbSet.AddRangeAsync(entities, cancellationToken));
            dbSetMock.Setup(m => m.AddRangeAsync(It.IsAny<TEntity[]>())).Returns((TEntity[] entities) => dbSet.AddRangeAsync(entities));

            dbSetMock.As<IAsyncEnumerableAccessor<TEntity>>().Setup(m => m.AsyncEnumerable).Returns(((IAsyncEnumerableAccessor<TEntity>) dbSet).AsyncEnumerable);

            dbSetMock.Setup(m => m.Attach(It.IsAny<TEntity>())).Returns((TEntity entity) => dbSet.Attach(entity));
            dbSetMock.Setup(m => m.AttachRange(It.IsAny<IEnumerable<TEntity>>())).Callback((IEnumerable<TEntity> entities) => dbSet.AttachRange(entities));
            dbSetMock.Setup(m => m.AttachRange(It.IsAny<TEntity[]>())).Callback((TEntity[] entities) => dbSet.AttachRange(entities));

            dbSetMock.As<IListSource>().Setup(m => m.ContainsListCollection).Returns(((IListSource) dbSet).ContainsListCollection);

            dbSetMock.As<IQueryable<TEntity>>().Setup(m => m.ElementType).Returns(((IQueryable<TEntity>) dbSet).ElementType);
            dbSetMock.As<IQueryable<TEntity>>().Setup(m => m.Expression).Returns(((IQueryable<TEntity>) dbSet).Expression);

            dbSetMock.Setup(m => m.Find(It.IsAny<object[]>())).Returns((object[] keyValues) => dbSet.Find(keyValues));

            dbSetMock.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns((object[] keyValues) => dbSet.FindAsync(keyValues));
            dbSetMock.Setup(m => m.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .Returns((object[] keyValues, CancellationToken cancellationToken) => dbSet.FindAsync(keyValues, cancellationToken));

            dbSetMock.As<IEnumerable>().Setup(m => m.GetEnumerator()).Returns(() => ((IEnumerable) dbSet).GetEnumerator());

            dbSetMock.As<IEnumerable<TEntity>>().Setup(m => m.GetEnumerator()).Returns(() => ((IEnumerable<TEntity>) dbSet).GetEnumerator());

            /*
             * System.NotSupportedException : Data binding directly to a store query is not supported. Instead populate a DbSet with data,
             * for example by calling Load on the DbSet, and then bind to local data to avoid sending a query to the database each time the
             * databound control iterates the data. For WPF bind to 'DbSet.Local.ToObservableCollection()'. For WinForms bind to
             * 'DbSet.Local.ToBindingList()'. For ASP.NET WebForms bind to 'DbSet.ToList()' or use Model Binding.
             */
            dbSetMock.As<IListSource>().Setup(m => m.GetList()).Returns(dbSet.ToList());

            dbSetMock.As<IInfrastructure<IServiceProvider>>().Setup(m => m.Instance).Returns(((IInfrastructure<IServiceProvider>) dbSet).Instance);

            dbSetMock.Setup(m => m.Local).Returns(dbSet.Local);

            dbSetMock.Setup(m => m.Remove(It.IsAny<TEntity>())).Returns((TEntity entity) => dbSet.Remove(entity));
            dbSetMock.Setup(m => m.RemoveRange(It.IsAny<IEnumerable<TEntity>>())).Callback((IEnumerable<TEntity> entities) => dbSet.RemoveRange(entities));
            dbSetMock.Setup(m => m.RemoveRange(It.IsAny<TEntity[]>())).Callback((TEntity[] entities) => dbSet.RemoveRange(entities));

            dbSetMock.Setup(m => m.Update(It.IsAny<TEntity>())).Returns((TEntity entity) => dbSet.Update(entity));
            dbSetMock.Setup(m => m.UpdateRange(It.IsAny<IEnumerable<TEntity>>())).Callback((IEnumerable<TEntity> entities) => dbSet.UpdateRange(entities));
            dbSetMock.Setup(m => m.UpdateRange(It.IsAny<TEntity[]>())).Callback((TEntity[] entities) => dbSet.UpdateRange(entities));

            dbSetMock.As<IQueryable<TEntity>>().Setup(m => m.Provider).Returns(((IQueryable<TEntity>) dbSet).Provider.CreateMock(dbSet));
        }
    }
}
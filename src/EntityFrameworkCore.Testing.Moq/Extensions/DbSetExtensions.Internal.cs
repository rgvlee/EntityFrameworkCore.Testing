using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Moq;
using rgvlee.Core.Common.Helpers;

namespace EntityFrameworkCore.Testing.Moq.Extensions
{
    /// <summary>
    ///     Extensions for db sets.
    /// </summary>
    public static class DbSetExtensions
    {
        internal static DbSet<TEntity> CreateMockedDbSet<TEntity>(this DbSet<TEntity> dbSet) where TEntity : class
        {
            EnsureArgument.IsNotNull(dbSet, nameof(dbSet));

            var dbSetMock = new Mock<DbSet<TEntity>>();

            var mockedQueryProvider = ((IQueryable<TEntity>) dbSet).Provider.CreateMockedQueryProvider(dbSet);

            dbSetMock.Setup(m => m.Add(It.IsAny<TEntity>())).Returns((TEntity providedEntity) => dbSet.Add(providedEntity));
            dbSetMock.Setup(m => m.AddAsync(It.IsAny<TEntity>(), It.IsAny<CancellationToken>()))
                .Returns((TEntity providedEntity, CancellationToken providedCancellationToken) => dbSet.AddAsync(providedEntity, providedCancellationToken));
            dbSetMock.Setup(m => m.AddRange(It.IsAny<IEnumerable<TEntity>>())).Callback((IEnumerable<TEntity> providedEntities) => dbSet.AddRange(providedEntities));
            dbSetMock.Setup(m => m.AddRange(It.IsAny<TEntity[]>())).Callback((TEntity[] providedEntities) => dbSet.AddRange(providedEntities));
            dbSetMock.Setup(m => m.AddRangeAsync(It.IsAny<IEnumerable<TEntity>>(), It.IsAny<CancellationToken>()))
                .Returns((IEnumerable<TEntity> providedEntities, CancellationToken providedCancellationToken) => dbSet.AddRangeAsync(providedEntities, providedCancellationToken));
            dbSetMock.Setup(m => m.AddRangeAsync(It.IsAny<TEntity[]>())).Returns((TEntity[] providedEntities) => dbSet.AddRangeAsync(providedEntities));

            dbSetMock.Setup(m => m.Attach(It.IsAny<TEntity>())).Returns((TEntity providedEntity) => dbSet.Attach(providedEntity));
            dbSetMock.Setup(m => m.AttachRange(It.IsAny<IEnumerable<TEntity>>())).Callback((IEnumerable<TEntity> providedEntities) => dbSet.AttachRange(providedEntities));
            dbSetMock.Setup(m => m.AttachRange(It.IsAny<TEntity[]>())).Callback((TEntity[] providedEntities) => dbSet.AttachRange(providedEntities));

            dbSetMock.As<IListSource>().Setup(m => m.ContainsListCollection).Returns(() => ((IListSource) dbSet).ContainsListCollection);

            dbSetMock.As<IQueryable<TEntity>>().Setup(m => m.ElementType).Returns(() => ((IQueryable<TEntity>) dbSet).ElementType);
            dbSetMock.Setup(m => m.EntityType).Returns(() => dbSet.EntityType);
            dbSetMock.As<IQueryable<TEntity>>().Setup(m => m.Expression).Returns(() => ((IQueryable<TEntity>) dbSet).Expression);

            dbSetMock.Setup(m => m.Find(It.IsAny<object[]>())).Returns((object[] providedKeyValues) => dbSet.Find(providedKeyValues));
            dbSetMock.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns((object[] providedKeyValues) => dbSet.FindAsync(providedKeyValues));
            dbSetMock.Setup(m => m.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .Returns((object[] providedKeyValues, CancellationToken providedCancellationToken) => dbSet.FindAsync(providedKeyValues, providedCancellationToken));

            dbSetMock.As<IAsyncEnumerable<TEntity>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns((CancellationToken providedCancellationToken) => ((IAsyncEnumerable<TEntity>) dbSet).GetAsyncEnumerator(providedCancellationToken));

            dbSetMock.As<IEnumerable>().Setup(m => m.GetEnumerator()).Returns(() => ((IEnumerable) dbSet).GetEnumerator());
            dbSetMock.As<IEnumerable<TEntity>>().Setup(m => m.GetEnumerator()).Returns(() => ((IEnumerable<TEntity>) dbSet).GetEnumerator());

            /*
             * System.NotSupportedException : Data binding directly to a store query is not supported. Instead populate a DbSet with data,
             * for example by calling Load on the DbSet, and then bind to local data to avoid sending a query to the database each time the
             * databound control iterates the data. For WPF bind to 'DbSet.Local.ToObservableCollection()'. For WinForms bind to
             * 'DbSet.Local.ToBindingList()'. For ASP.NET WebForms bind to 'DbSet.ToList()' or use Model Binding.
             */
            dbSetMock.As<IListSource>().Setup(m => m.GetList()).Returns(() => dbSet.ToList());

            dbSetMock.As<IInfrastructure<IServiceProvider>>().Setup(m => m.Instance).Returns(() => ((IInfrastructure<IServiceProvider>) dbSet).Instance);

            dbSetMock.Setup(m => m.Local).Returns(() => dbSet.Local);

            dbSetMock.Setup(m => m.Remove(It.IsAny<TEntity>())).Returns((TEntity providedEntity) => dbSet.Remove(providedEntity));
            dbSetMock.Setup(m => m.RemoveRange(It.IsAny<IEnumerable<TEntity>>())).Callback((IEnumerable<TEntity> providedEntities) => dbSet.RemoveRange(providedEntities));
            dbSetMock.Setup(m => m.RemoveRange(It.IsAny<TEntity[]>())).Callback((TEntity[] providedEntities) => dbSet.RemoveRange(providedEntities));

            dbSetMock.Setup(m => m.Update(It.IsAny<TEntity>())).Returns((TEntity providedEntity) => dbSet.Update(providedEntity));
            dbSetMock.Setup(m => m.UpdateRange(It.IsAny<IEnumerable<TEntity>>())).Callback((IEnumerable<TEntity> providedEntities) => dbSet.UpdateRange(providedEntities));
            dbSetMock.Setup(m => m.UpdateRange(It.IsAny<TEntity[]>())).Callback((TEntity[] providedEntities) => dbSet.UpdateRange(providedEntities));

            dbSetMock.As<IQueryable<TEntity>>().Setup(m => m.Provider).Returns(() => mockedQueryProvider);

            dbSetMock.Setup(m => m.AsAsyncEnumerable()).Returns(() => dbSet.AsAsyncEnumerable());
            dbSetMock.Setup(m => m.AsQueryable()).Returns(() => dbSet.AsQueryable());

            return dbSetMock.Object;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query.Internal;
using NSubstitute;
using rgvlee.Core.Common.Helpers;

namespace EntityFrameworkCore.Testing.NSubstitute.Extensions
{
    /// <summary>
    ///     Extensions for db sets.
    /// </summary>
    public static class DbSetExtensions
    {
        internal static DbSet<TEntity> CreateMockedDbSet<TEntity>(this DbSet<TEntity> dbSet) where TEntity : class
        {
            EnsureArgument.IsNotNull(dbSet, nameof(dbSet));

            var mockedDbSet = (DbSet<TEntity>) Substitute.For(new[] {
                    typeof(DbSet<TEntity>),
                    typeof(IAsyncEnumerableAccessor<TEntity>),
                    typeof(IEnumerable),
                    typeof(IEnumerable<TEntity>),
                    typeof(IInfrastructure<IServiceProvider>),
                    typeof(IListSource),
                    typeof(IQueryable<TEntity>)
                },
                new object[] { });

            var mockedQueryProvider = ((IQueryable<TEntity>) dbSet).Provider.CreateMockedQueryProvider(dbSet);

            mockedDbSet.Add(Arg.Any<TEntity>()).Returns(callInfo => dbSet.Add(callInfo.Arg<TEntity>()));
            mockedDbSet.AddAsync(Arg.Any<TEntity>(), Arg.Any<CancellationToken>()).Returns(callInfo => dbSet.AddAsync(callInfo.Arg<TEntity>(), callInfo.Arg<CancellationToken>()));
            mockedDbSet.When(x => x.AddRange(Arg.Any<IEnumerable<TEntity>>())).Do(callInfo => dbSet.AddRange(callInfo.Arg<IEnumerable<TEntity>>()));
            mockedDbSet.When(x => x.AddRange(Arg.Any<TEntity[]>())).Do(callInfo => dbSet.AddRange(callInfo.Arg<TEntity[]>()));
            mockedDbSet.AddRangeAsync(Arg.Any<IEnumerable<TEntity>>(), Arg.Any<CancellationToken>())
                .Returns(callInfo => dbSet.AddRangeAsync(callInfo.Arg<IEnumerable<TEntity>>(), callInfo.Arg<CancellationToken>()));
            mockedDbSet.AddRangeAsync(Arg.Any<TEntity[]>()).Returns(callInfo => dbSet.AddRangeAsync(callInfo.Arg<TEntity[]>()));

            ((IAsyncEnumerableAccessor<TEntity>) mockedDbSet).AsyncEnumerable.Returns(callInfo => ((IAsyncEnumerableAccessor<TEntity>) dbSet).AsyncEnumerable);

            mockedDbSet.Attach(Arg.Any<TEntity>()).Returns(callInfo => dbSet.Attach(callInfo.Arg<TEntity>()));
            mockedDbSet.When(x => x.AttachRange(Arg.Any<IEnumerable<TEntity>>())).Do(callInfo => dbSet.AttachRange(callInfo.Arg<IEnumerable<TEntity>>()));
            mockedDbSet.When(x => x.AttachRange(Arg.Any<TEntity[]>())).Do(callInfo => dbSet.AttachRange(callInfo.Arg<TEntity[]>()));

            ((IListSource) mockedDbSet).ContainsListCollection.Returns(callInfo => ((IListSource) dbSet).ContainsListCollection);

            ((IQueryable<TEntity>) mockedDbSet).ElementType.Returns(callInfo => ((IQueryable<TEntity>) dbSet).ElementType);
            ((IQueryable<TEntity>) mockedDbSet).Expression.Returns(callInfo => ((IQueryable<TEntity>) dbSet).Expression);

            mockedDbSet.Find(Arg.Any<object[]>()).Returns(callInfo => dbSet.Find(callInfo.Arg<object[]>()));
            mockedDbSet.FindAsync(Arg.Any<object[]>()).Returns(callInfo => dbSet.FindAsync(callInfo.Arg<object[]>()));
            mockedDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
                .Returns(callInfo => dbSet.FindAsync(callInfo.Arg<object[]>(), callInfo.Arg<CancellationToken>()));

            ((IEnumerable) mockedDbSet).GetEnumerator().Returns(callInfo => ((IEnumerable) dbSet).GetEnumerator());
            ((IEnumerable<TEntity>) mockedDbSet).GetEnumerator().Returns(callInfo => ((IEnumerable<TEntity>) dbSet).GetEnumerator());

            /*
             * System.NotSupportedException : Data binding directly to a store query is not supported. Instead populate a DbSet with data,
             * for example by calling Load on the DbSet, and then bind to local data to avoid sending a query to the database each time the
             * databound control iterates the data. For WPF bind to 'DbSet.Local.ToObservableCollection()'. For WinForms bind to
             * 'DbSet.Local.ToBindingList()'. For ASP.NET WebForms bind to 'DbSet.ToList()' or use Model Binding.
             */
            ((IListSource) mockedDbSet).GetList().Returns(callInfo => dbSet.ToList());

            ((IInfrastructure<IServiceProvider>) mockedDbSet).Instance.Returns(callInfo => ((IInfrastructure<IServiceProvider>) dbSet).Instance);

            mockedDbSet.Local.Returns(callInfo => dbSet.Local);

            mockedDbSet.Remove(Arg.Any<TEntity>()).Returns(callInfo => dbSet.Remove(callInfo.Arg<TEntity>()));
            mockedDbSet.When(x => x.RemoveRange(Arg.Any<IEnumerable<TEntity>>())).Do(callInfo => dbSet.RemoveRange(callInfo.Arg<IEnumerable<TEntity>>()));
            mockedDbSet.When(x => x.RemoveRange(Arg.Any<TEntity[]>())).Do(callInfo => dbSet.RemoveRange(callInfo.Arg<TEntity[]>()));

            mockedDbSet.Update(Arg.Any<TEntity>()).Returns(callInfo => dbSet.Update(callInfo.Arg<TEntity>()));
            mockedDbSet.When(x => x.UpdateRange(Arg.Any<IEnumerable<TEntity>>())).Do(callInfo => dbSet.UpdateRange(callInfo.Arg<IEnumerable<TEntity>>()));
            mockedDbSet.When(x => x.UpdateRange(Arg.Any<TEntity[]>())).Do(callInfo => dbSet.UpdateRange(callInfo.Arg<TEntity[]>()));

            ((IQueryable<TEntity>) mockedDbSet).Provider.Returns(callInfo => mockedQueryProvider);

            return mockedDbSet;
        }
    }
}
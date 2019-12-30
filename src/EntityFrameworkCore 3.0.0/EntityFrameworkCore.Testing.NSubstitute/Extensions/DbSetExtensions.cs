using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using EntityFrameworkCore.Testing.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NSubstitute;

namespace EntityFrameworkCore.Testing.NSubstitute.Extensions
{
    /// <summary>Extensions for the db set type.</summary>
    public static class DbSetExtensions
    {
        /// <summary>Creates and sets up a mocked db set.</summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="dbSet">The db set to mock/proxy.</param>
        /// <returns>A mocked db set.</returns>
        internal static DbSet<TEntity> CreateMockedDbSet<TEntity>(this DbSet<TEntity> dbSet)
            where TEntity : class
        {
            EnsureArgument.IsNotNull(dbSet, nameof(dbSet));

            var mockedDbSet = (DbSet<TEntity>)
                Substitute.For(
                    new[] {
                        typeof(DbSet<TEntity>),
                        typeof(IAsyncEnumerable<TEntity>),
                        typeof(IEnumerable),
                        typeof(IEnumerable<TEntity>),
                        typeof(IInfrastructure<IServiceProvider>),
                        typeof(IListSource),
                        typeof(IQueryable<TEntity>)
                    },
                    new object[] { }
                );

            mockedDbSet.Add(Arg.Any<TEntity>()).Returns(callInfo => dbSet.Add(callInfo.Arg<TEntity>()));
            mockedDbSet.AddAsync(Arg.Any<TEntity>(), Arg.Any<CancellationToken>()).Returns(callInfo => dbSet.AddAsync(callInfo.Arg<TEntity>(), callInfo.Arg<CancellationToken>()));
            mockedDbSet.When(x => x.AddRange(Arg.Any<IEnumerable<TEntity>>())).Do(callInfo => dbSet.AddRange(callInfo.Arg<IEnumerable<TEntity>>()));
            mockedDbSet.When(x => x.AddRange(Arg.Any<TEntity[]>())).Do(callInfo => dbSet.AddRange(callInfo.Arg<TEntity[]>()));
            mockedDbSet.AddRangeAsync(Arg.Any<IEnumerable<TEntity>>(), Arg.Any<CancellationToken>()).Returns(callInfo => dbSet.AddRangeAsync(callInfo.Arg<IEnumerable<TEntity>>(), callInfo.Arg<CancellationToken>()));
            mockedDbSet.AddRangeAsync(Arg.Any<TEntity[]>()).Returns(callInfo => dbSet.AddRangeAsync(callInfo.Arg<TEntity[]>()));

            mockedDbSet.Attach(Arg.Any<TEntity>()).Returns(callInfo => dbSet.Attach(callInfo.Arg<TEntity>()));
            mockedDbSet.When(x => x.AttachRange(Arg.Any<IEnumerable<TEntity>>())).Do(callInfo => dbSet.AttachRange(callInfo.Arg<IEnumerable<TEntity>>()));
            mockedDbSet.When(x => x.AttachRange(Arg.Any<TEntity[]>())).Do(callInfo => dbSet.AttachRange(callInfo.Arg<TEntity[]>()));

            ((IListSource) mockedDbSet).ContainsListCollection.Returns(callInfo => ((IListSource) dbSet).ContainsListCollection);

            ((IQueryable<TEntity>) mockedDbSet).ElementType.Returns(callInfo => ((IQueryable<TEntity>) dbSet).ElementType);
            ((IQueryable<TEntity>) mockedDbSet).Expression.Returns(callInfo => ((IQueryable<TEntity>) dbSet).Expression);

            mockedDbSet.Find(Arg.Any<object[]>()).Returns(callInfo => dbSet.Find(callInfo.Arg<object[]>()));
            mockedDbSet.FindAsync(Arg.Any<object[]>()).Returns(callInfo => dbSet.FindAsync(callInfo.Arg<object[]>()));
            mockedDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>()).Returns(callInfo => dbSet.FindAsync(callInfo.Arg<object[]>(), callInfo.Arg<CancellationToken>()));

            ((IAsyncEnumerable<TEntity>) mockedDbSet).GetAsyncEnumerator(Arg.Any<CancellationToken>()).Returns(callInfo => ((IAsyncEnumerable<TEntity>) dbSet).GetAsyncEnumerator(callInfo.Arg<CancellationToken>()));

            ((IEnumerable) mockedDbSet).GetEnumerator().Returns(callInfo => ((IEnumerable) dbSet).GetEnumerator());
            ((IEnumerable<TEntity>) mockedDbSet).GetEnumerator().Returns(callInfo => ((IEnumerable<TEntity>) dbSet).GetEnumerator());

            ((IListSource) mockedDbSet).GetList().Returns(callInfo => dbSet.ToList());

            ((IInfrastructure<IServiceProvider>) mockedDbSet).Instance.Returns(callInfo => ((IInfrastructure<IServiceProvider>) dbSet).Instance);

            mockedDbSet.Local.Returns(callInfo => dbSet.Local);

            mockedDbSet.Remove(Arg.Any<TEntity>()).Returns(callInfo => dbSet.Remove(callInfo.Arg<TEntity>()));
            mockedDbSet.When(x => x.RemoveRange(Arg.Any<IEnumerable<TEntity>>())).Do(callInfo => dbSet.RemoveRange(callInfo.Arg<IEnumerable<TEntity>>()));
            mockedDbSet.When(x => x.RemoveRange(Arg.Any<TEntity[]>())).Do(callInfo => dbSet.RemoveRange(callInfo.Arg<TEntity[]>()));

            mockedDbSet.Update(Arg.Any<TEntity>()).Returns(callInfo => dbSet.Update(callInfo.Arg<TEntity>()));
            mockedDbSet.When(x => x.UpdateRange(Arg.Any<IEnumerable<TEntity>>())).Do(callInfo => dbSet.UpdateRange(callInfo.Arg<IEnumerable<TEntity>>()));
            mockedDbSet.When(x => x.UpdateRange(Arg.Any<TEntity[]>())).Do(callInfo => dbSet.UpdateRange(callInfo.Arg<TEntity[]>()));

            var mockedQueryProvider = ((IQueryable<TEntity>) dbSet).Provider.CreateMockedQueryProvider(dbSet);
            ((IQueryable<TEntity>) mockedDbSet).Provider.Returns(callInfo => mockedQueryProvider);

            return mockedDbSet;
        }
    }
}
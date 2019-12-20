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
        /// <summary>Creates and sets up a substitute db set.</summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="dbSet">The db set to mock/proxy.</param>
        /// <returns>A substitute db set.</returns>
        public static DbSet<TEntity> CreateSubstituteDbSet<TEntity>(this DbSet<TEntity> dbSet)
            where TEntity : class
        {
            EnsureArgument.IsNotNull(dbSet, nameof(dbSet));

            var substituteDbSet = (DbSet<TEntity>)
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

            substituteDbSet.Add(Arg.Any<TEntity>()).Returns(callInfo => dbSet.Add(callInfo.Arg<TEntity>()));
            substituteDbSet.AddAsync(Arg.Any<TEntity>(), Arg.Any<CancellationToken>()).Returns(callInfo => dbSet.AddAsync(callInfo.Arg<TEntity>(), callInfo.Arg<CancellationToken>()));
            substituteDbSet.When(x => x.AddRange(Arg.Any<IEnumerable<TEntity>>())).Do(callInfo => dbSet.AddRange(callInfo.Arg<IEnumerable<TEntity>>()));
            substituteDbSet.When(x => x.AddRange(Arg.Any<TEntity[]>())).Do(callInfo => dbSet.AddRange(callInfo.Arg<TEntity[]>()));
            substituteDbSet.AddRangeAsync(Arg.Any<IEnumerable<TEntity>>(), Arg.Any<CancellationToken>()).Returns(callInfo => dbSet.AddRangeAsync(callInfo.Arg<IEnumerable<TEntity>>(), callInfo.Arg<CancellationToken>()));
            substituteDbSet.AddRangeAsync(Arg.Any<TEntity[]>()).Returns(callInfo => dbSet.AddRangeAsync(callInfo.Arg<TEntity[]>()));

            substituteDbSet.Attach(Arg.Any<TEntity>()).Returns(callInfo => dbSet.Attach(callInfo.Arg<TEntity>()));
            substituteDbSet.When(x => x.AttachRange(Arg.Any<IEnumerable<TEntity>>())).Do(callInfo => dbSet.AttachRange(callInfo.Arg<IEnumerable<TEntity>>()));
            substituteDbSet.When(x => x.AttachRange(Arg.Any<TEntity[]>())).Do(callInfo => dbSet.AttachRange(callInfo.Arg<TEntity[]>()));

            ((IListSource) substituteDbSet).ContainsListCollection.Returns(callInfo => ((IListSource) dbSet).ContainsListCollection);

            ((IQueryable<TEntity>) substituteDbSet).ElementType.Returns(callInfo => ((IQueryable<TEntity>) dbSet).ElementType);
            ((IQueryable<TEntity>) substituteDbSet).Expression.Returns(callInfo => ((IQueryable<TEntity>) dbSet).Expression);

            substituteDbSet.Find(Arg.Any<object[]>()).Returns(callInfo => dbSet.Find(callInfo.Arg<object[]>()));
            substituteDbSet.FindAsync(Arg.Any<object[]>()).Returns(callInfo => dbSet.FindAsync(callInfo.Arg<object[]>()));
            substituteDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>()).Returns(callInfo => dbSet.FindAsync(callInfo.Arg<object[]>(), callInfo.Arg<CancellationToken>()));

            ((IAsyncEnumerable<TEntity>) substituteDbSet).GetAsyncEnumerator(Arg.Any<CancellationToken>()).Returns(callInfo => ((IAsyncEnumerable<TEntity>) dbSet).GetAsyncEnumerator(callInfo.Arg<CancellationToken>()));

            ((IEnumerable) substituteDbSet).GetEnumerator().Returns(callInfo => ((IEnumerable) dbSet).GetEnumerator());
            ((IEnumerable<TEntity>) substituteDbSet).GetEnumerator().Returns(callInfo => ((IEnumerable<TEntity>) dbSet).GetEnumerator());
            
            ((IListSource) substituteDbSet).GetList().Returns(callInfo => dbSet.ToList());

            ((IInfrastructure<IServiceProvider>) substituteDbSet).Instance.Returns(callInfo => ((IInfrastructure<IServiceProvider>) dbSet).Instance);

            substituteDbSet.Local.Returns(callInfo => dbSet.Local);

            substituteDbSet.Remove(Arg.Any<TEntity>()).Returns(callInfo => dbSet.Remove(callInfo.Arg<TEntity>()));
            substituteDbSet.When(x => x.RemoveRange(Arg.Any<IEnumerable<TEntity>>())).Do(callInfo => dbSet.RemoveRange(callInfo.Arg<IEnumerable<TEntity>>()));
            substituteDbSet.When(x => x.RemoveRange(Arg.Any<TEntity[]>())).Do(callInfo => dbSet.RemoveRange(callInfo.Arg<TEntity[]>()));

            substituteDbSet.Update(Arg.Any<TEntity>()).Returns(callInfo => dbSet.Update(callInfo.Arg<TEntity>()));
            substituteDbSet.When(x => x.UpdateRange(Arg.Any<IEnumerable<TEntity>>())).Do(callInfo => dbSet.UpdateRange(callInfo.Arg<IEnumerable<TEntity>>()));
            substituteDbSet.When(x => x.UpdateRange(Arg.Any<TEntity[]>())).Do(callInfo => dbSet.UpdateRange(callInfo.Arg<TEntity[]>()));

            var substituteQueryProvider = ((IQueryable<TEntity>) dbSet).Provider.CreateSubstituteQueryProvider(dbSet);
            ((IQueryable<TEntity>) substituteDbSet).Provider.Returns(callInfo => substituteQueryProvider);

            return substituteDbSet;
        }
    }
}
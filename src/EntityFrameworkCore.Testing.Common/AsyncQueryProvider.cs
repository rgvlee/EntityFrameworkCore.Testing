using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using rgvlee.Core.Common.Helpers;

namespace EntityFrameworkCore.Testing.Common
{
    /// <inheritdoc />
    public class AsyncQueryProvider<T> : IAsyncQueryProvider
    {
        private static readonly ILogger Logger = LoggingHelper.CreateLogger(typeof(AsyncQueryProvider<T>));

        public AsyncQueryProvider(IEnumerable<T> enumerable)
        {
            Source = enumerable.AsQueryable();
        }

        /// <summary>
        ///     The query provider source.
        /// </summary>
        public virtual IQueryable<T> Source { get; set; }

        /// <inheritdoc />
        public virtual IQueryable CreateQuery(Expression expression)
        {
            //Handles cases where we are projecting to another type
            if (expression is MethodCallExpression methodCallExpression)
            {
                var returnType = methodCallExpression.Method.ReturnType;
                if (returnType.GetGenericTypeDefinition() != typeof(IQueryable<>))
                {
                    throw new InvalidOperationException($"Expected IQueryable<>; actual {returnType.FullName}");
                }

                var method = typeof(IQueryProvider).GetMethods().Single(x => x.Name.Equals(nameof(IQueryProvider.CreateQuery)) && x.IsGenericMethod);
                var enumerable = method.MakeGenericMethod(returnType.GetGenericArguments().Single()).Invoke(Source.Provider, new[] { expression });

                return (IQueryable) Activator.CreateInstance(typeof(AsyncEnumerable<>).GetGenericTypeDefinition().MakeGenericType(returnType.GetGenericArguments().Single()),
                    enumerable);
            }

            return CreateQuery<T>(expression);
        }

        /// <inheritdoc />
        public virtual IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new AsyncEnumerable<TElement>(Source.Provider.CreateQuery<TElement>(expression));
        }

        /// <inheritdoc />
        public virtual object Execute(Expression expression)
        {
            return Source.Provider.Execute(expression);
        }

        /// <inheritdoc />
        public virtual TResult Execute<TResult>(Expression expression)
        {
            return Source.Provider.Execute<TResult>(expression);
        }

        /// <inheritdoc />
        public virtual TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            //TResult is a Task<T>. The provider requires T.
            return (TResult) typeof(AsyncQueryProvider<T>).GetMethod(nameof(InternalExecuteAsync), BindingFlags.Instance | BindingFlags.NonPublic)
                .MakeGenericMethod(typeof(TResult).GetGenericArguments())
                .Invoke(this, new object[] { expression, cancellationToken });
        }

        internal Task<TResult> InternalExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute<TResult>(expression));
        }
    }
}
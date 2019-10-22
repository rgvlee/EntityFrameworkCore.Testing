using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Testing.Common.Helpers;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Logging;

namespace EntityFrameworkCore.Testing.Common
{
    /// <inheritdoc />
    public class AsyncQueryProvider<T> : IAsyncQueryProvider
    {
        private static readonly ILogger Logger = LoggerHelper.CreateLogger(typeof(AsyncQueryProvider<T>));

        /// <summary>
        ///     The query provider source.
        /// </summary>
        public virtual IQueryable<T> Source { get; set; }

        /// <inheritdoc />
        public virtual IQueryable CreateQuery(Expression expression)
        {
            return new AsyncEnumerable<T>(expression);
        }

        /// <inheritdoc />
        public virtual IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            //Why is this done here? Primarily because we can't change the Moq mock interface once we have started using the mocked instance.
            //It is not possible to set up every possible interface as the method can be invoked with an anonymous type.
            //We don't lose anything doing it this way. We can still verify invocations and we don't have to duplicate the functionality for each project.
            if (expression is MethodCallExpression methodCallExpression)
            {
                if (methodCallExpression.Method.Name.Equals(nameof(Queryable.Select)))
                {
                    Logger.LogDebug($"{methodCallExpression.Method.Name} invoked; expression: '{methodCallExpression}'");

                    var unaryExpression = (UnaryExpression) methodCallExpression.Arguments[1];
                    var predicateExpression = unaryExpression.Operand;
                    if (predicateExpression.Type.GetGenericArguments().ToList().Count.Equals(3))
                    {
                        var predicate = ((Expression<Func<T, int, TElement>>) predicateExpression).Compile();
                        return Source.Cast<T>().ToList().Select((x, i) => predicate(x, i)).AsQueryable();
                    }
                    else
                    {
                        var predicate = ((Expression<Func<T, TElement>>) predicateExpression).Compile();
                        return Source.Cast<T>().ToList().Select(x => predicate(x)).AsQueryable();
                    }
                }

                if (methodCallExpression.Method.Name.Equals(nameof(Queryable.SkipWhile)) ||
                    methodCallExpression.Method.Name.Equals(nameof(Queryable.TakeWhile)))
                {
                    var unaryExpression = (UnaryExpression) methodCallExpression.Arguments[1];
                    var predicateExpression = unaryExpression.Operand;
                    if (predicateExpression.Type.GetGenericArguments().ToList().Count.Equals(3))
                    {
                        var predicate = ((Expression<Func<TElement, int, bool>>) predicateExpression).Compile();
                        return methodCallExpression.Method.Name.Equals(nameof(Queryable.SkipWhile))
                            ? Source.Cast<TElement>().ToList().SkipWhile((x, i) => predicate(x, i)).AsQueryable()
                            : Source.Cast<TElement>().ToList().TakeWhile((x, i) => predicate(x, i)).AsQueryable();
                    }
                    else
                    {
                        var predicate = ((Expression<Func<TElement, bool>>) predicateExpression).Compile();
                        return methodCallExpression.Method.Name.Equals(nameof(Queryable.SkipWhile))
                            ? Source.Cast<TElement>().ToList().SkipWhile(x => predicate(x)).AsQueryable()
                            : Source.Cast<TElement>().ToList().TakeWhile(x => predicate(x)).AsQueryable();
                    }
                }
            }

            return new AsyncEnumerable<TElement>(expression);
        }

        /// <inheritdoc />
        public virtual object Execute(Expression expression)
        {
            return Source.Provider.Execute(expression);
        }

        /// <inheritdoc />
        public virtual TResult Execute<TResult>(Expression expression)
        {
            if (expression is MethodCallExpression methodCallExpression)
                if (
                    methodCallExpression.Method.Name.Equals(nameof(Queryable.ElementAt)) ||
                    methodCallExpression.Method.Name.Equals(nameof(Queryable.ElementAtOrDefault))
                )
                {
                    var mce = methodCallExpression;
                    var index = (int) ((ConstantExpression) mce.Arguments[1]).Value;
                    return mce.Method.Name.Equals(nameof(Queryable.ElementAt))
                        ? Source.Cast<TResult>().ToList().ElementAt(index)
                        : Source.Cast<TResult>().ToList().ElementAtOrDefault(index);
                }

            return Source.Provider.Execute<TResult>(expression);
        }

        /// <inheritdoc />
        public virtual IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
        {
            return Task.FromResult(Execute<TResult>(expression)).ToAsyncEnumerable();
        }

        /// <inheritdoc />
        public virtual Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute<TResult>(expression));
        }
    }
}
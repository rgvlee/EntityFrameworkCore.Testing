using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.Logging;
using rgvlee.Core.Common.Helpers;

namespace EntityFrameworkCore.Testing.Common.Helpers
{
    /// <summary>
    ///     A helper for expressions.
    /// </summary>
    public static class ExpressionHelper
    {
        private static readonly ILogger Logger = LoggingHelper.CreateLogger(typeof(ExpressionHelper));

        /// <summary>
        ///     Creates a property expression for the specified property.
        /// </summary>
        /// <typeparam name="TParameter">The expression parameter.</typeparam>
        /// <typeparam name="TProperty">The expression property.</typeparam>
        /// <param name="propertyInfo">The property info of the property to create the expression for.</param>
        /// <returns>A property expression for the specified property.</returns>
        public static Expression<Func<TParameter, TProperty>> CreatePropertyExpression<TParameter, TProperty>(PropertyInfo propertyInfo)
        {
            EnsureArgument.IsNotNull(propertyInfo, nameof(propertyInfo));

            var parameter = Expression.Parameter(typeof(TParameter));
            return Expression.Lambda<Func<TParameter, TProperty>>(Expression.Property(parameter, propertyInfo), parameter);
        }

        public static bool SqlAndParametersMatchFromSqlExpression(string sql, IEnumerable<object> parameters, MethodCallExpression expression)
        {
            EnsureArgument.IsNotNull(expression, nameof(expression));
            EnsureArgument.IsNotNull(parameters, nameof(parameters));

            var result = expression.Method.Name.Equals("FromSqlOnQueryable") &&
                         SqlMatchesFromSqlExpression(sql, expression) &&
                         ParameterMatchingHelper.DoInvocationParametersMatchSetUpParameters(parameters, (object[]) ((ConstantExpression) expression.Arguments[2]).Value);

            Logger.LogDebug("Match? {result}", result);

            return result;
        }

        private static bool SqlMatchesFromSqlExpression(string sql, MethodCallExpression expression)
        {
            EnsureArgument.IsNotNull(expression, nameof(expression));

            var expressionSql = (string) ((ConstantExpression) expression.Arguments[1]).Value;
            var parts = new List<string>();
            parts.Add($"Invocation sql: '{expressionSql}'");
            parts.Add($"Set up sql: '{sql}'");
            Logger.LogDebug(string.Join(Environment.NewLine, parts));

            var result = expressionSql.Contains(sql, StringComparison.OrdinalIgnoreCase);

            Logger.LogDebug("Match? {result}", result);

            return result;
        }

        public static string StringifyFromSqlExpression(MethodCallExpression expression)
        {
            EnsureArgument.IsNotNull(expression, nameof(expression));

            var expressionSql = (string) ((ConstantExpression) expression.Arguments[1]).Value;
            var expressionParameters = (object[]) ((ConstantExpression) expression.Arguments[2]).Value;
            var parts = new List<string>();
            parts.Add($"Invocation sql: '{expressionSql}'");
            parts.Add("Invocation Parameters:");
            parts.Add(ParameterMatchingHelper.StringifyParameters(expressionParameters));
            return string.Join(Environment.NewLine, parts);
        }

        public static void ThrowIfExpressionIsNotSupported(Expression expression)
        {
            if (expression is MethodCallExpression mce)
            {
                Logger.LogDebug("{methodName} invoked; expression: '{expression}'", mce.Method.Name, mce);

                if (mce.Method.Name.Equals(nameof(Queryable.ElementAt)))
                {
                    throw new InvalidOperationException();
                }

                if (mce.Method.Name.Equals(nameof(Queryable.ElementAtOrDefault)))
                {
                    throw new InvalidOperationException();
                }

                if (mce.Method.Name.Equals(nameof(Queryable.Select)))
                {
                    var unaryExpression = (UnaryExpression) mce.Arguments[1];
                    var predicateExpression = unaryExpression.Operand;
                    if (predicateExpression.Type.GetGenericArguments().ToList().Count.Equals(3))
                    {
                        throw new InvalidOperationException();
                    }
                }

                if (mce.Method.Name.Equals(nameof(Queryable.SkipWhile)))
                {
                    throw new InvalidOperationException();
                }

                if (mce.Method.Name.Equals(nameof(Queryable.TakeWhile)))
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}
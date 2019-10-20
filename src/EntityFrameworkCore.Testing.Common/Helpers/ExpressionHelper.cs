using System;
using System.Linq.Expressions;
using System.Reflection;

namespace EntityFrameworkCore.Testing.Common.Helpers
{
    /// <summary>
    ///     A helper for creating expressions.
    /// </summary>
    public static class ExpressionHelper
    {
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
    }
}
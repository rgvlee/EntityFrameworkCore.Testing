using System;

namespace EntityFrameworkCore.Testing.Common.Extensions
{
    /// <summary>
    ///     Extensions for the type type.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        ///     Gets the default value for the specified type.
        /// </summary>
        /// <param name="type">The type instance.</param>
        /// <returns>The default value for the specified type.</returns>
        public static object GetDefaultValue(this Type type)
        {
            EnsureArgument.IsNotNull(type, nameof(type));

            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}
using System;

namespace EntityFrameworkCore.Testing.Common.Extensions
{
    /// <summary>Extensions for the <see cref="Type" /> type.</summary>
    public static class TypeExtensions
    {
        /// <summary>Gets the default value for the specified type.</summary>
        /// <param name="type">The type to get the default value for.</param>
        /// <returns>The default value for the specified type.</returns>
        public static object GetDefaultValue(this Type type)
        {
            EnsureArgument.IsNotNull(type, nameof(type));
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        public static bool HasParameterlessConstructor(this Type type)
        {
            EnsureArgument.IsNotNull(type, nameof(type));
            return type.HasConstructorWithParametersOfType(new Type[] { });
        }

        public static bool HasConstructorWithParameterOfType(this Type type, Type parameterType)
        {
            EnsureArgument.IsNotNull(type, nameof(type));
            EnsureArgument.IsNotNull(parameterType, nameof(parameterType));
            return type.HasConstructorWithParametersOfType(new[] { parameterType });
        }

        public static bool HasConstructorWithParametersOfType(this Type type, Type[] parameterTypes)
        {
            EnsureArgument.IsNotNull(type, nameof(type));
            EnsureArgument.IsNotNull(parameterTypes, nameof(parameterTypes));
            return type.IsValueType || type.GetConstructor(parameterTypes) != null;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace EntityFrameworkCore.Testing.Common.Helpers
{
    /// <summary>
    ///     A helper to perform checks on arguments.
    /// </summary>
    public class EnsureArgument
    {
        /// <summary>
        ///     Ensures that a string argument is not null or empty.
        /// </summary>
        /// <param name="string">The string argument.</param>
        /// <param name="argumentName">The argument name.</param>
        /// <returns>The string argument.</returns>
        /// <exception cref="ArgumentNullException">If the string argument is null.</exception>
        /// <exception cref="ArgumentException">If the string argument is empty.</exception>
        public static string IsNotNullOrEmpty(string @string, string argumentName)

        {
            if (!string.IsNullOrEmpty(@string)) return @string;

            IsNotNull(argumentName, nameof(argumentName));
            IsNotNullOrEmpty(argumentName, nameof(argumentName));

            var ex = @string == null ? new ArgumentNullException(argumentName) : new ArgumentException(argumentName);
            throw ex;
        }

        /// <summary>
        ///     Ensures that an argument is not null.
        /// </summary>
        /// <typeparam name="T">The argument type.</typeparam>
        /// <param name="argument">The argument.</param>
        /// <param name="argumentName">The argument name.</param>
        /// <returns>The argument.</returns>
        /// <exception cref="ArgumentNullException">If the argument is null.</exception>
        public static T IsNotNull<T>(T argument, string argumentName)
        {
            if (argument != null) return argument;

            IsNotNull(argumentName, nameof(argumentName));
            IsNotNullOrEmpty(argumentName, nameof(argumentName));

            var ex = new ArgumentNullException(argumentName);
            throw ex;
        }

        /// <summary>
        ///     Ensures that a collection is not empty.
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="argumentName">The collection argument name.</param>
        /// <returns>The collection.</returns>
        /// <exception cref="ArgumentException">If the collection is empty.</exception>
        public static IEnumerable<T> IsNotEmpty<T>(IEnumerable<T> collection, string argumentName)
        {
            IsNotNull(collection, argumentName);

            if (collection.Any()) return collection;

            IsNotNull(argumentName, nameof(argumentName));
            IsNotNullOrEmpty(argumentName, nameof(argumentName));

            var ex = new ArgumentException(argumentName);
            throw ex;
        }
    }
}
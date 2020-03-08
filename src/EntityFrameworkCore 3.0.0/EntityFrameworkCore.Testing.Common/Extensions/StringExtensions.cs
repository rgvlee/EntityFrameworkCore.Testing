using System;
using EntityFrameworkCore.Testing.Common.Helpers;

namespace EntityFrameworkCore.Testing.Common.Extensions
{
    /// <summary>Extensions for the <see cref="string" /> type.</summary>
    public static class StringExtensions
    {
        /// <summary>Checks to see if the target string contains the search for string using the specified string comparer.</summary>
        /// <param name="target">The string to search.</param>
        /// <param name="searchFor">The string to search for within the target.</param>
        /// <param name="comparer">The string comparer.</param>
        /// <returns>true if the target string contains the search for string using the specified string comparer.</returns>
        public static bool Contains(this string target, string searchFor, StringComparison comparer)
        {
            EnsureArgument.IsNotNull(target, nameof(target));
            EnsureArgument.IsNotNull(searchFor, nameof(searchFor));

            return target.IndexOf(searchFor, 0, comparer) != -1;
        }
    }
}
using EntityFrameworkCore.Testing.Common;
using EntityFrameworkCore.Testing.Common.Helpers;
using Microsoft.Extensions.Logging;
using Moq;

namespace EntityFrameworkCore.Testing.Moq.Helpers
{
    /// <summary>
    ///     A helper for the <see cref="Mock{T}" /> type.
    /// </summary>
    internal class MockHelper
    {
        private static readonly ILogger Logger = LoggerHelper.CreateLogger(typeof(MockExtensions));

        /// <summary>
        ///     Attempts to get a mock from an instance.
        /// </summary>
        /// <typeparam name="T">The instance type.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="mock">The mock for the instance.</param>
        /// <returns>true if the instance is a mock.</returns>
        internal static bool TryGetMock<T>(T instance, out Mock<T> mock) where T : class
        {
            EnsureArgument.IsNotNull(instance, nameof(instance));

            mock = null;

            try
            {
                mock = Mock.Get(instance);
                return true;
            }
            catch
            {
                Logger.LogDebug($"{nameof(instance)} ('{typeof(T)}') is not a mock");
                return false;
            }
        }
    }
}
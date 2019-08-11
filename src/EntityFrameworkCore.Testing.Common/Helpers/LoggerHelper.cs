using System;
using Microsoft.Extensions.Logging;

namespace EntityFrameworkCore.Testing.Common.Helpers {
    /// <summary>
    /// Helper for creating logger instances.
    /// </summary>
    public static class LoggerHelper {
        private static ILoggerFactory _factory;
        
        /// <summary>
        /// Gets or sets the logger factory.
        /// </summary>
        public static ILoggerFactory LoggerFactory {
            get => _factory ?? (_factory = new LoggerFactory());
            set => _factory = value;
        }

        /// <summary>
        /// Creates a new ILogger instance using the full name of the specified type.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <returns>A new <see cref="ILogger{T}"/> instance.</returns>
        public static ILogger<T> CreateLogger<T>() => LoggerFactory.CreateLogger<T>();

        /// <summary>
        /// Creates a new <see cref="ILogger"/> instance.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        /// <returns>A new <see cref="ILogger"/> instance.</returns>
        public static ILogger CreateLogger(string categoryName) => LoggerFactory.CreateLogger(categoryName);

        /// <summary>
        /// Creates a new ILogger instance using the full name of the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A new <see cref="ILogger"/> instance.</returns>
        public static ILogger CreateLogger(Type type) => LoggerFactory.CreateLogger(type);
    }
}

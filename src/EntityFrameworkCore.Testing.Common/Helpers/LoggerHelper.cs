using System;
using Microsoft.Extensions.Logging;

namespace EntityFrameworkCore.Testing.Common.Helpers
{
    /// <summary>A helper for creating loggers.</summary>
    public static class LoggerHelper
    {
        private static ILoggerFactory _factory;

        /// <summary>Gets or sets the logger factory used to create loggers.</summary>
        public static ILoggerFactory LoggerFactory {
            get => _factory ?? (_factory = new LoggerFactory());
            set => _factory = value;
        }

        /// <summary>Creates a new logger instance using the full name of the specified type.</summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <returns>A new logger instance.</returns>
        public static ILogger<T> CreateLogger<T>()
        {
            return LoggerFactory.CreateLogger<T>();
        }

        /// <summary>Creates a new logger instance.</summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        /// <returns>A new logger instance.</returns>
        public static ILogger CreateLogger(string categoryName)
        {
            EnsureArgument.IsNotNullOrEmpty(categoryName, nameof(categoryName));

            return LoggerFactory.CreateLogger(categoryName);
        }

        /// <summary>Creates a new logger instance using the full name of the specified type.</summary>
        /// <param name="type">The type.</param>
        /// <returns>A new logger instance.</returns>
        public static ILogger CreateLogger(Type type)
        {
            EnsureArgument.IsNotNull(type, nameof(type));

            return LoggerFactory.CreateLogger(type);
        }
    }
}
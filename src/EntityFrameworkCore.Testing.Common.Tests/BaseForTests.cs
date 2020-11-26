using AutoFixture;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using rgvlee.Core.Common.Helpers;

namespace EntityFrameworkCore.Testing.Common.Tests
{
    public abstract class BaseForTests
    {
        protected static readonly ILogger Logger = LoggingHelper.CreateLogger(typeof(BaseForTests));

        protected Fixture Fixture;

        [SetUp]
        public virtual void SetUp()
        {
            LoggingHelper.LoggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug));
            Fixture = new Fixture();
        }
    }
}
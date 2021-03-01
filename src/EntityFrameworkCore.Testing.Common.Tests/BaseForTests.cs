using AutoFixture;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using rgvlee.Core.Common.Helpers;

namespace EntityFrameworkCore.Testing.Common.Tests
{
    public abstract class BaseForTests
    {
        protected Fixture Fixture;

        [SetUp]
        public virtual void SetUp()
        {
            LoggingHelper.LoggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Trace));

            Fixture = new Fixture();
        }

        [TearDown]
        public virtual void TearDown()
        {
            LoggingHelper.LoggerFactory.Dispose();
        }
    }
}
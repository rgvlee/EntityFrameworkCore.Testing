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
#pragma warning disable 618
            LoggingHelper.LoggerFactory = new LoggerFactory().AddConsole(LogLevel.Trace);
#pragma warning restore 618
            Fixture = new Fixture();
        }

        [TearDown]
        public virtual void TearDown()
        {
            LoggingHelper.LoggerFactory.Dispose();
        }
    }
}
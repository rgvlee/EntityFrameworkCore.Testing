using AutoFixture;
using EntityFrameworkCore.Testing.Common.Helpers;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Common.Tests
{
    [TestFixture]
    public abstract class BaseForTests
    {
        [SetUp]
        public virtual void SetUp()
        {
            LoggerHelper.LoggerFactory.AddConsole(LogLevel.Debug);
            Fixture = new Fixture();
        }

        protected Fixture Fixture;

        protected static readonly ILogger Logger = LoggerHelper.CreateLogger(typeof(BaseForTests));
    }
}
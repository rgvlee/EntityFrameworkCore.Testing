using AutoFixture;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using rgvlee.Core.Common.Helpers;

namespace EntityFrameworkCore.Testing.Common.Tests
{
    [TestFixture]
    public abstract class BaseForTests
    {
        [SetUp]
        public virtual void SetUp()
        {
#pragma warning disable 618
            LoggingHelper.LoggerFactory.AddConsole(LogLevel.Debug);
#pragma warning restore 618
            Fixture = new Fixture();
        }

        protected Fixture Fixture;

        protected static readonly ILogger Logger = LoggingHelper.CreateLogger(typeof(BaseForTests));
    }
}
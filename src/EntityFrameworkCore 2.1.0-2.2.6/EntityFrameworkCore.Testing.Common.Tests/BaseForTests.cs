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
#pragma warning disable 618
            LoggerHelper.LoggerFactory.AddConsole(LogLevel.Debug);
#pragma warning restore 618
            Fixture = new Fixture();
        }

        protected Fixture Fixture;

        protected static readonly ILogger Logger = LoggerHelper.CreateLogger(typeof(BaseForTests));
    }
}
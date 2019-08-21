using EntityFrameworkCore.Testing.Common.Helpers;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Common.Tests {
    [TestFixture]
    public abstract class TestBase {
        protected static readonly ILogger Logger = LoggerHelper.CreateLogger(typeof(TestBase));
        
        [SetUp]
        public virtual void SetUp() {
            LoggerHelper.LoggerFactory.AddConsole(LogLevel.Debug);
        }
    }
}

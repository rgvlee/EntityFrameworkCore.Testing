using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using EntityFrameworkCore.Testing.Common.Tests;
using EntityFrameworkCore.Testing.NSubstitute.Extensions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.NSubstitute.PackageVerification.Tests
{
    public class Issue1Tests
    {
        [SetUp]
        public virtual void SetUp()
        {
            //LoggerHelper.LoggerFactory.AddConsole(LogLevel.Debug);
        }

        [Test]
        public async Task ExecuteSqlRawAsync_SpecifiedSqlAndSqlParameter_ReturnsExpectedResultAndSetsOutputParameterValue()
        {
            var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();
            mockedDbContext.AddExecuteSqlRawResult(-1, (sql, parameters) =>
            {
                ((SqlParameter) parameters.ElementAt(0)).Value = "Cookie";
            });

            var outcomeParam = new SqlParameter("Outcome", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
            var result = await mockedDbContext.Database.ExecuteSqlRawAsync(@"EXEC [GiveMeCookie] @Outcome = @Outcome OUT", outcomeParam);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(-1, result);
                Assert.That(outcomeParam.Value.ToString(), Is.EqualTo("Cookie"));
            });
        }

        [Test]
        public void GiveMeCookie_SetsOutputParameterValue()
        {
            var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();
            mockedDbContext.AddExecuteSqlRawResult(-1, (sql, parameters) =>
            {
                ((SqlParameter) parameters.ElementAt(0)).Value = "Cookie";
            });

            var service = new MyService(mockedDbContext);

            Assert.That(service.GiveMeCookie().Result, Is.EqualTo("Cookie"));
        }

        [Test]
        public void CreateMockedDbContextFor_ParametersForSpecificConstructor_CreatesSubstitute()
        {
            var testContext = Create.MockedDbContextFor<MyContext>(
                new DbContextOptionsBuilder<MyContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).EnableSensitiveDataLogging().Options,
                Substitute.For<ITimeProvider>());

            Assert.Multiple(() =>
            {
                Assert.That(testContext, Is.Not.Null);
                Assert.That(ProxyUtil.IsProxy(testContext), Is.True);
            });
        }

        public class MyService
        {
            private readonly DbContext _context;

            public MyService(DbContext context)
            {
                _context = context;
            }

            public async Task<string> GiveMeCookie()
            {
                var outcomeParam = new SqlParameter("Outcome", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
                await _context.Database.ExecuteSqlRawAsync(@"EXEC [GiveMeCookie] @Outcome = @Outcome OUT", outcomeParam);
                return outcomeParam.Value.ToString();
            }
        }

        public interface ITimeProvider { }

        public class MyContext : DbContext
        {
            public MyContext(DbContextOptions<MyContext> options, ITimeProvider timeProvider) : base(options) { }
        }
    }
}
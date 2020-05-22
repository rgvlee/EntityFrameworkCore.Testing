using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using EntityFrameworkCore.Testing.Common.Helpers;
using EntityFrameworkCore.Testing.Common.Tests;
using EntityFrameworkCore.Testing.Moq.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Moq.Tests
{
    public class Issue1Tests
    {
        [SetUp]
        public virtual void SetUp()
        {
            LoggerHelper.LoggerFactory.AddConsole(LogLevel.Debug);
        }

        [Test]
        public async Task ExecuteSqlCommandAsync_SpecifiedSqlAndSqlParameter_ReturnsExpectedResultAndSetsOutputParameterValue()
        {
            var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();
            mockedDbContext.AddExecuteSqlCommandResult(-1,
                (sql, parameters) =>
                {
                    ((SqlParameter) parameters.ElementAt(0)).Value = "Cookie";
                });

            var outcomeParam = new SqlParameter("Outcome", SqlDbType.NVarChar, 255) { Direction = ParameterDirection.Output };
            var result = await mockedDbContext.Database.ExecuteSqlCommandAsync(@"EXEC [GiveMeCookie] @Outcome = @Outcome OUT", outcomeParam);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(-1, result);
                Assert.That(outcomeParam.Value.ToString(), Is.EqualTo("Cookie"));
            });
        }
    }
}
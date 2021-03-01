using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Castle.DynamicProxy;
using EntityFrameworkCore.Testing.Common.Tests;
using EntityFrameworkCore.Testing.NSubstitute.Extensions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.NSubstitute.Tests
{
    public class Issue1Tests : BaseForTests
    {
        [Test]
        public void CreateMockedDbContextFor_ParametersForSpecificConstructor_CreatesSubstitute()
        {
            var mockedDbContext = Create.MockedDbContextFor<TestDbContext>(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options);

            Assert.Multiple(() =>
            {
                Assert.That(mockedDbContext, Is.Not.Null);
                Assert.That(ProxyUtil.IsProxy(mockedDbContext), Is.True);
            });
        }

        [Test]
        public async Task ExecuteSqlRawAsync_SpecifiedSqlAndSqlParameter_ReturnsExpectedResultAndSetsOutputParameterValue()
        {
            var expectedResult = Fixture.Create<string>();

            var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();
            mockedDbContext.AddExecuteSqlRawResult(-1,
                (sql, parameters) =>
                {
                    ((SqlParameter) parameters.ElementAt(0)).Value = expectedResult;
                });

            var outputParameter = new SqlParameter("OutputParameter", SqlDbType.NVarChar, 255) { Direction = ParameterDirection.Output };
            var result = await mockedDbContext.Database.ExecuteSqlRawAsync(@"EXEC [StoredProcedureWithOutputParameter] @OutputParameter = @Result OUTPUT", outputParameter);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(-1));
                Assert.That(outputParameter.Value.ToString(), Is.EqualTo(expectedResult));
            });
        }
    }
}
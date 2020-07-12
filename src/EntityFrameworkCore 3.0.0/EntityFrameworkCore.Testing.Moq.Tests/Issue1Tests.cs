using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Castle.DynamicProxy;
using EntityFrameworkCore.Testing.Common.Tests;
using EntityFrameworkCore.Testing.Moq.Extensions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Moq.Tests
{
    public class Issue1Tests : BaseForTests
    {
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
                Assert.AreEqual(-1, result);
                Assert.That(outputParameter.Value.ToString(), Is.EqualTo(expectedResult));
            });
        }

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
    }
}
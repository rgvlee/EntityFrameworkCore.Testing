using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Testing.Moq.Tests {
    [TestFixture]
    public class MockedExecuteSqlCommandTests {
        [Test]
        public void Execute_AnySqlWithNoParameters_ReturnsExpectedResult() {
            var commandText = "";
            var expectedResult = 1;

            var builder = new DbContextMockBuilder<TestDbContext>();
            builder.AddExecuteSqlCommandResult(commandText, new List<SqlParameter>(), expectedResult);
            var mockedContext = builder.GetMockedDbContext();

            var result1 = mockedContext.Database.ExecuteSqlCommand("sp_NoParams");
            var result2 = mockedContext.Database.ExecuteSqlCommand("sp_NoParams");

            Assert.Multiple(() => {
                Assert.AreEqual(result1, result2);
                Assert.AreEqual(expectedResult, result1);
            });
        }

        [Test]
        public async Task ExecuteAsync_AnySqlWithNoParameters_ReturnsExpectedResult() {
            var commandText = "";
            var expectedResult = 1;

            var builder = new DbContextMockBuilder<TestDbContext>();
            builder.AddExecuteSqlCommandResult(commandText, new List<SqlParameter>(), expectedResult);

            var mockedContext = builder.GetMockedDbContext();

            var result1 = await mockedContext.Database.ExecuteSqlCommandAsync("sp_NoParams");
            var result2 = await mockedContext.Database.ExecuteSqlCommandAsync("sp_NoParams");

            Assert.Multiple(() => {
                Assert.AreEqual(result1, result2);
                Assert.AreEqual(expectedResult, result1);
            });
        }

        [Test]
        public void Execute_SpecifiedSqlWithNoParameters_ReturnsExpectedResult() {
            var commandText = "sp_NoParams";
            var expectedResult = 1;

            var builder = new DbContextMockBuilder<TestDbContext>();
            builder.AddExecuteSqlCommandResult(commandText, new List<SqlParameter>(), expectedResult);
            var mockedContext = builder.GetMockedDbContext();

            var result1 = mockedContext.Database.ExecuteSqlCommand("sp_NoParams");
            var result2 = mockedContext.Database.ExecuteSqlCommand("sp_NoParams");

            Assert.Multiple(() => {
                Assert.AreEqual(result1, result2);
                Assert.AreEqual(expectedResult, result1);
            });
        }

        [Test]
        public async Task ExecuteAsync_SpecifiedSqlWithNoParameters_ReturnsExpectedResult() {
            var commandText = "sp_NoParams";
            var expectedResult = 1;

            var builder = new DbContextMockBuilder<TestDbContext>();
            builder.AddExecuteSqlCommandResult(commandText, new List<SqlParameter>(), expectedResult);
            var mockedContext = builder.GetMockedDbContext();

            var result1 = await mockedContext.Database.ExecuteSqlCommandAsync("sp_NoParams");
            var result2 = await mockedContext.Database.ExecuteSqlCommandAsync("sp_NoParams");

            Assert.Multiple(() => {
                Assert.AreEqual(result1, result2);
                Assert.AreEqual(expectedResult, result1);
            });
        }

        [Test]
        public void Execute_SpecifiedSqlWithParameters_ReturnsExpectedResult() {
            var commandText = "sp_WithParams";
            var sqlParameters = new List<SqlParameter>() {new SqlParameter("@SomeParameter2", "Value2")};
            var expectedResult = 1;

            var builder = new DbContextMockBuilder<TestDbContext>();
            builder.AddExecuteSqlCommandResult(commandText, sqlParameters, expectedResult);
            var mockedContext = builder.GetMockedDbContext();

            var result1 = mockedContext.Database.ExecuteSqlCommand("[dbo.[sp_WithParams] @SomeParameter2", sqlParameters);
            var result2 = mockedContext.Database.ExecuteSqlCommand("[dbo.[sp_WithParams] @SomeParameter2", sqlParameters);

            Assert.Multiple(() => {
                Assert.AreEqual(result1, result2);
                Assert.AreEqual(expectedResult, result1);
            });
        }

        [Test]
        public async Task ExecuteAsync_SpecifiedSqlWithParameters_ReturnsExpectedResult() {
            var commandText = "sp_WithParams";
            var sqlParameters = new List<SqlParameter>() { new SqlParameter("@SomeParameter2", "Value2") };
            var expectedResult = 1;

            var builder = new DbContextMockBuilder<TestDbContext>();
            builder.AddExecuteSqlCommandResult(commandText, sqlParameters, expectedResult);
            var mockedContext = builder.GetMockedDbContext();

            var result1 = await mockedContext.Database.ExecuteSqlCommandAsync("[dbo.[sp_WithParams] @SomeParameter2", sqlParameters);
            var result2 = await mockedContext.Database.ExecuteSqlCommandAsync("[dbo.[sp_WithParams] @SomeParameter2", sqlParameters);

            Assert.Multiple(() => {
                Assert.AreEqual(result1, result2);
                Assert.AreEqual(expectedResult, result1);
            });
        }

        [Test]
        public void Execute_SpecifiedSqlWithNoParametersThatDoesNotMatchSetUp_ThrowsException() {
            var commandText = "asdf";
            var expectedResult = 1;

            var builder = new DbContextMockBuilder<TestDbContext>();
            builder.AddExecuteSqlCommandResult(commandText, new List<SqlParameter>(), expectedResult);

            var mockedContext = builder.GetMockedDbContext();
            
            Assert.Throws<NullReferenceException>(() => {
                var result = mockedContext.Database.ExecuteSqlCommand("sp_NoParams");
            });
        }

        [Test]
        public void ExecuteAsync_SpecifiedSqlWithNoParametersThatDoesNotMatchSetUp_ThrowsException() {
            var commandText = "asdf";
            var expectedResult = 1;

            var builder = new DbContextMockBuilder<TestDbContext>();
            builder.AddExecuteSqlCommandResult(commandText, new List<SqlParameter>(), expectedResult);

            var mockedContext = builder.GetMockedDbContext();

            Assert.ThrowsAsync<NullReferenceException>(async () => {
                var result = await mockedContext.Database.ExecuteSqlCommandAsync("sp_NoParams");
            });
        }
    }
}

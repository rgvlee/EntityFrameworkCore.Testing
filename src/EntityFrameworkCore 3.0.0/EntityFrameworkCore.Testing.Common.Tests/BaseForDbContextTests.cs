using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Common.Tests
{
    [TestFixture]
    public abstract class BaseForDbContextTests<TDbContext> : BaseForTests
        where TDbContext : DbContext
    {
        protected TDbContext MockedDbContext;

        public abstract void AddExecuteSqlCommandResult(TDbContext mockedDbContext, int expectedResult);
        public abstract void AddExecuteSqlCommandResult(TDbContext mockedDbContext, int expectedResult, Action<string, IEnumerable<object>> callback);
        public abstract void AddExecuteSqlCommandResult(TDbContext mockedDbContext, string sql, int expectedResult);
        public abstract void AddExecuteSqlCommandResult(TDbContext mockedDbContext, string sql, int expectedResult, Action<string, IEnumerable<object>> callback);
        public abstract void AddExecuteSqlCommandResult(TDbContext mockedDbContext, string sql, IEnumerable<object> parameters, int expectedResult);
        public abstract void AddExecuteSqlCommandResult(TDbContext mockedDbContext, string sql, IEnumerable<object> parameters, int expectedResult, Action<string, IEnumerable<object>> callback);

        public abstract void AddExecuteSqlInterpolatedResult(TDbContext mockedDbContext, int expectedResult);
        public abstract void AddExecuteSqlInterpolatedResult(TDbContext mockedDbContext, FormattableString sql, int expectedResult);
        public abstract void AddExecuteSqlInterpolatedResult(TDbContext mockedDbContext, string sql, IEnumerable<object> parameters, int expectedResult);

        public abstract void AddExecuteSqlRawResult(TDbContext mockedDbContext, int expectedResult);
        public abstract void AddExecuteSqlRawResult(TDbContext mockedDbContext, string sql, int expectedResult);
        public abstract void AddExecuteSqlRawResult(TDbContext mockedDbContext, string sql, IEnumerable<object> parameters, int expectedResult);

        public static IEnumerable<TestCaseData> ExecuteSqlCommandWithCallback_InvokesCallback_TestCases()
        {
            yield return new TestCaseData(string.Empty, string.Empty, null, 1);
            yield return new TestCaseData("sql", string.Empty, null, 1);
            yield return new TestCaseData("sql", "someparameter", "somevalue", 1);
        }

        [TestCaseSource(nameof(ExecuteSqlCommandWithCallback_InvokesCallback_TestCases))]
        public void ExecuteSqlCommandWithCallback_InvokesCallback(string sql, string parameterName, object parameterValue, int expectedResult)
        {
            var itemsToCreate = 3;
            var source = Fixture.CreateMany<string>(itemsToCreate).ToList();

            var preSetUpFirst = source.First();
            var preSetUpCount = source.Count;

            void Callback(string providedSql, IEnumerable<object> providedParameters)
            {
                Logger.LogDebug($"Before callback invoked: {source.Count}");
                source = source.Take(1).ToList();
                Logger.LogDebug($"After callback invoked: {source.Count}");
            }

            var parameters = new List<object>();
            if (!string.IsNullOrWhiteSpace(parameterName) &&
                parameterValue != null)
            {
                parameters.Add(new SqlParameter(parameterName, parameterValue));
            }

            Logger.LogDebug("Setting up ExecuteSqlCommand");
            if (!string.IsNullOrWhiteSpace(sql) &&
                parameters.Any())
            {
                AddExecuteSqlCommandResult(MockedDbContext, sql, parameters, expectedResult, Callback);
            }
            else if (!string.IsNullOrWhiteSpace(sql))
            {
                AddExecuteSqlCommandResult(MockedDbContext, sql, expectedResult, Callback);
            }
            else
            {
                AddExecuteSqlCommandResult(MockedDbContext, expectedResult, Callback);
            }

            var postSetUpCount = source.Count;

            Logger.LogDebug("Invoking ExecuteSqlCommand");
            var actualResult1 = default(int);
            var actualResult2 = default(int);

#pragma warning disable 618
            if (!string.IsNullOrWhiteSpace(sql) &&
                parameters.Any())
            {
                actualResult1 = MockedDbContext.Database.ExecuteSqlCommand(sql, parameters);
                actualResult2 = MockedDbContext.Database.ExecuteSqlCommand(sql, parameters);
            }
            else if (!string.IsNullOrWhiteSpace(sql))
            {
                actualResult1 = MockedDbContext.Database.ExecuteSqlCommand(sql);
                actualResult2 = MockedDbContext.Database.ExecuteSqlCommand(sql);
            }
            else
            {
                actualResult1 = MockedDbContext.Database.ExecuteSqlCommand("sp_NoParams");
                actualResult2 = MockedDbContext.Database.ExecuteSqlCommand("sp_NoParams");
            }
#pragma warning restore 618

            Logger.LogDebug("ExecuteSqlCommand invoked");

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
                Assert.That(preSetUpCount, Is.EqualTo(itemsToCreate));
                Assert.That(postSetUpCount, Is.EqualTo(preSetUpCount));
                Assert.That(source.Count, Is.EqualTo(1));
                Assert.That(source.First(), Is.EqualTo(preSetUpFirst));
            });
        }

        [TestCaseSource(nameof(ExecuteSqlCommandWithCallback_InvokesCallback_TestCases))]
        public async Task ExecuteSqlCommandAsyncWithCallback_InvokesCallback(string sql, string parameterName, object parameterValue, int expectedResult)
        {
            var itemsToCreate = 3;
            var source = Fixture.CreateMany<string>(itemsToCreate).ToList();

            var preSetUpFirst = source.First();
            var preSetUpCount = source.Count;

            void Callback(string providedSql, IEnumerable<object> providedParameters)
            {
                Logger.LogDebug($"Before callback invoked: {source.Count}");
                source = source.Take(1).ToList();
                Logger.LogDebug($"After callback invoked: {source.Count}");
            }

            var parameters = new List<object>();
            if (!string.IsNullOrWhiteSpace(parameterName) &&
                parameterValue != null)
            {
                parameters.Add(new SqlParameter(parameterName, parameterValue));
            }

            Logger.LogDebug("Setting up ExecuteSqlCommand");
            if (!string.IsNullOrWhiteSpace(sql) &&
                parameters.Any())
            {
                AddExecuteSqlCommandResult(MockedDbContext, sql, parameters, expectedResult, Callback);
            }
            else if (!string.IsNullOrWhiteSpace(sql))
            {
                AddExecuteSqlCommandResult(MockedDbContext, sql, expectedResult, Callback);
            }
            else
            {
                AddExecuteSqlCommandResult(MockedDbContext, expectedResult, Callback);
            }

            var postSetUpCount = source.Count;

            Logger.LogDebug("Invoking ExecuteSqlCommand");
            var actualResult1 = default(int);
            var actualResult2 = default(int);

#pragma warning disable 618
            if (!string.IsNullOrWhiteSpace(sql) &&
                parameters.Any())
            {
                actualResult1 = await MockedDbContext.Database.ExecuteSqlCommandAsync(sql, parameters);
                actualResult2 = await MockedDbContext.Database.ExecuteSqlCommandAsync(sql, parameters);
            }
            else if (!string.IsNullOrWhiteSpace(sql))
            {
                actualResult1 = await MockedDbContext.Database.ExecuteSqlCommandAsync(sql);
                actualResult2 = await MockedDbContext.Database.ExecuteSqlCommandAsync(sql);
            }
            else
            {
                actualResult1 = await MockedDbContext.Database.ExecuteSqlCommandAsync("sp_NoParams");
                actualResult2 = await MockedDbContext.Database.ExecuteSqlCommandAsync("sp_NoParams");
            }
#pragma warning restore 618

            Logger.LogDebug("ExecuteSqlCommand invoked");

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
                Assert.That(preSetUpCount, Is.EqualTo(itemsToCreate));
                Assert.That(postSetUpCount, Is.EqualTo(preSetUpCount));
                Assert.That(source.Count, Is.EqualTo(1));
                Assert.That(source.First(), Is.EqualTo(preSetUpFirst));
            });
        }

        [Test]
        public void ExecuteSqlCommand_AnySql_ReturnsExpectedResult()
        {
            var expectedResult = 1;
            AddExecuteSqlCommandResult(MockedDbContext, expectedResult);

#pragma warning disable 618
            var actualResult1 = MockedDbContext.Database.ExecuteSqlCommand("sp_NoParams");
            var actualResult2 = MockedDbContext.Database.ExecuteSqlCommand("sp_NoParams");
#pragma warning restore 618

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }

        [Test]
        public void ExecuteSqlCommand_SpecifiedSql_ReturnsExpectedResult()
        {
            var sql = "sp_NoParams";
            var expectedResult = 1;
            AddExecuteSqlCommandResult(MockedDbContext, sql, expectedResult);

#pragma warning disable 618
            var actualResult1 = MockedDbContext.Database.ExecuteSqlCommand("sp_NoParams");
            var actualResult2 = MockedDbContext.Database.ExecuteSqlCommand("sp_NoParams");
#pragma warning restore 618

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }

        [Test]
        public void ExecuteSqlCommand_SpecifiedSqlThatDoesNotMatchSetUp_ThrowsException()
        {
            var sql = "asdf";
            var expectedResult = 1;
            AddExecuteSqlCommandResult(MockedDbContext, sql, expectedResult);

            Assert.Throws<InvalidOperationException>(() =>
            {
#pragma warning disable 618
                var actualResult = MockedDbContext.Database.ExecuteSqlCommand("sp_NoParams");
#pragma warning restore 618
            });
        }

        [Test]
        public void ExecuteSqlCommand_SpecifiedSqlWithSqlParameterParameters_ReturnsExpectedResult()
        {
            var sql = "sp_WithParams";
            var parameters = new List<SqlParameter> { new SqlParameter("@SomeParameter2", "Value2") };
            var expectedResult = 1;
            AddExecuteSqlCommandResult(MockedDbContext, sql, parameters, expectedResult);

#pragma warning disable 618
            var actualResult1 = MockedDbContext.Database.ExecuteSqlCommand("[dbo].[sp_WithParams] @SomeParameter1 @SomeParameter2", parameters);
            var actualResult2 = MockedDbContext.Database.ExecuteSqlCommand("[dbo].[sp_WithParams] @SomeParameter1 @SomeParameter2", parameters);
#pragma warning restore 618

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }

        [Test]
        public void ExecuteSqlCommand_SpecifiedSqlWithSqlParameterParametersThatDoNotMatchSetUp_ThrowsException()
        {
            var sql = "sp_WithParams";
            var setUpParameters = new List<SqlParameter> { new SqlParameter("@SomeParameter3", "Value3") };
            var invocationParameters = new List<SqlParameter> { new SqlParameter("@SomeParameter1", "Value1"), new SqlParameter("@SomeParameter2", "Value2") };
            var expectedResult = 1;
            AddExecuteSqlCommandResult(MockedDbContext, sql, setUpParameters, expectedResult);

#pragma warning disable 618
            Assert.Throws<InvalidOperationException>(() =>
            {
                var actualResult1 = MockedDbContext.Database.ExecuteSqlCommand("[dbo].[sp_WithParams] @SomeParameter1 @SomeParameter2", invocationParameters);
            });

            Assert.Throws<InvalidOperationException>(() =>
            {
                var actualResult2 = MockedDbContext.Database.ExecuteSqlCommand("[dbo].[sp_WithParams] @SomeParameter1 @SomeParameter2", invocationParameters);
            });
#pragma warning restore 618
        }

        [Test]
        public void ExecuteSqlCommand_SpecifiedSqlWithStringParameterParameters_ReturnsExpectedResult()
        {
            var sql = "sp_WithParams";
            var parameters = new List<string> { "Value2" };
            var expectedResult = 1;
            AddExecuteSqlCommandResult(MockedDbContext, sql, parameters, expectedResult);

#pragma warning disable 618
            var actualResult1 = MockedDbContext.Database.ExecuteSqlCommand("[dbo].[sp_WithParams] @SomeParameter1 @SomeParameter2", parameters);
            var actualResult2 = MockedDbContext.Database.ExecuteSqlCommand("[dbo].[sp_WithParams] @SomeParameter1 @SomeParameter2", parameters);
#pragma warning restore 618

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }

        [Test]
        public void ExecuteSqlCommand_WithNoMatchesAdded_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
#pragma warning disable 618
                var actualResult = MockedDbContext.Database.ExecuteSqlCommand("sp_NoParams");
#pragma warning restore 618
            });
        }

        [Test]
        public async Task ExecuteSqlCommandAsync_AnySql_ReturnsExpectedResult()
        {
            var expectedResult = 1;
            AddExecuteSqlCommandResult(MockedDbContext, expectedResult);

#pragma warning disable 618
            var actualResult1 = await MockedDbContext.Database.ExecuteSqlCommandAsync("sp_NoParams");
            var actualResult2 = await MockedDbContext.Database.ExecuteSqlCommandAsync("sp_NoParams");
#pragma warning restore 618

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }

        [Test]
        public async Task ExecuteSqlCommandAsync_SpecifiedSql_ReturnsExpectedResult()
        {
            var sql = "sp_NoParams";
            var expectedResult = 1;
            AddExecuteSqlCommandResult(MockedDbContext, sql, expectedResult);

#pragma warning disable 618
            var actualResult1 = await MockedDbContext.Database.ExecuteSqlCommandAsync("sp_NoParams");
            var actualResult2 = await MockedDbContext.Database.ExecuteSqlCommandAsync("sp_NoParams");
#pragma warning restore 618

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }

        [Test]
        public void ExecuteSqlCommandAsync_SpecifiedSqlThatDoesNotMatchSetUp_ThrowsException()
        {
            var sql = "asdf";
            var expectedResult = 1;
            AddExecuteSqlCommandResult(MockedDbContext, sql, expectedResult);

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
#pragma warning disable 618
                var actualResult = await MockedDbContext.Database.ExecuteSqlCommandAsync("sp_NoParams");
#pragma warning restore 618
            });
        }

        [Test]
        public async Task ExecuteSqlCommandAsync_SpecifiedSqlWithSqlParameterParameters_ReturnsExpectedResult()
        {
            var sql = "sp_WithParams";
            var parameters = new List<SqlParameter> { new SqlParameter("@SomeParameter2", "Value2") };
            var expectedResult = 1;
            AddExecuteSqlCommandResult(MockedDbContext, sql, parameters, expectedResult);

#pragma warning disable 618
            var actualResult1 = await MockedDbContext.Database.ExecuteSqlCommandAsync("[dbo].[sp_WithParams] @SomeParameter2", parameters);
            var actualResult2 = await MockedDbContext.Database.ExecuteSqlCommandAsync("[dbo].[sp_WithParams] @SomeParameter2", parameters);
#pragma warning restore 618

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }

        [Test]
        public async Task ExecuteSqlCommandAsync_SpecifiedSqlWithStringParameterParameters_ReturnsExpectedResult()
        {
            var sql = "sp_WithParams";
            var parameters = new List<string> { "Value2" };
            var expectedResult = 1;
            AddExecuteSqlCommandResult(MockedDbContext, sql, parameters, expectedResult);

#pragma warning disable 618
            var actualResult1 = await MockedDbContext.Database.ExecuteSqlCommandAsync("[dbo].[sp_WithParams] @SomeParameter2", parameters);
            var actualResult2 = await MockedDbContext.Database.ExecuteSqlCommandAsync("[dbo].[sp_WithParams] @SomeParameter2", parameters);
#pragma warning restore 618

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }

        [Test]
        public void ExecuteSqlCommandWithMultipleSetUps_SpecifiedSql_ReturnsExpectedResult()
        {
            var sql = "sp_NoParams1";
            var expectedResult = 1;
            AddExecuteSqlCommandResult(MockedDbContext, sql, expectedResult);
            AddExecuteSqlCommandResult(MockedDbContext, "sp_NoParams2", 2);

#pragma warning disable 618
            var actualResult1 = MockedDbContext.Database.ExecuteSqlCommand("[dbo].sp_NoParams1");
            var actualResult2 = MockedDbContext.Database.ExecuteSqlCommand("[dbo].sp_NoParams1");
#pragma warning restore 618

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }

        [Test]
        public void ExecuteSqlInterpolated_AnySql_ReturnsExpectedResult()
        {
            var expectedResult = 1;
            AddExecuteSqlInterpolatedResult(MockedDbContext, expectedResult);

            var actualResult1 = MockedDbContext.Database.ExecuteSqlInterpolated($"sp_NoParams");
            var actualResult2 = MockedDbContext.Database.ExecuteSqlInterpolated($"sp_NoParams");

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }

        [Test]
        public void ExecuteSqlInterpolated_SpecifiedFormattableStringWithSqlParameterParameters_ReturnsExpectedResult()
        {
            var sql = "sp_WithParams";
            var parameters = new List<SqlParameter> { new SqlParameter("@SomeParameter2", "Value2") };
            var expectedResult = 1;
            AddExecuteSqlInterpolatedResult(MockedDbContext, sql, parameters, expectedResult);

            var actualResult1 = MockedDbContext.Database.ExecuteSqlInterpolated($"[dbo].[sp_WithParams] {parameters[0]} {Fixture.Create<string>()}");
            var actualResult2 = MockedDbContext.Database.ExecuteSqlInterpolated($"[dbo].[sp_WithParams] {parameters[0]} {Fixture.Create<string>()}");

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }

        [Test]
        public void ExecuteSqlInterpolated_SpecifiedFormattableStringWithSqlParameterParametersThatDoNotMatchSetUp_ThrowsException()
        {
            var sql = "sp_WithParams";
            var setUpParameters = new List<SqlParameter> { new SqlParameter("@SomeParameter3", "Value3") };
            var invocationParameters = new List<SqlParameter> { new SqlParameter("@SomeParameter1", "Value1"), new SqlParameter("@SomeParameter2", "Value2") };
            var expectedResult = 1;
            AddExecuteSqlInterpolatedResult(MockedDbContext, sql, setUpParameters, expectedResult);

            Assert.Throws<InvalidOperationException>(() =>
            {
                var actualResult1 = MockedDbContext.Database.ExecuteSqlInterpolated($"[dbo].[sp_WithParams] {invocationParameters[0]}, {invocationParameters[1]}");
            });

            Assert.Throws<InvalidOperationException>(() =>
            {
                var actualResult2 = MockedDbContext.Database.ExecuteSqlInterpolated($"[dbo].[sp_WithParams] {invocationParameters[0]}, {invocationParameters[1]}");
            });
        }

        [Test]
        public void ExecuteSqlInterpolated_SpecifiedSql_ReturnsExpectedResult()
        {
            var sql = (FormattableString) $"sp_NoParams";
            var expectedResult = 1;
            AddExecuteSqlInterpolatedResult(MockedDbContext, sql, expectedResult);

            var actualResult1 = MockedDbContext.Database.ExecuteSqlInterpolated($"sp_NoParams");
            var actualResult2 = MockedDbContext.Database.ExecuteSqlInterpolated($"sp_NoParams");

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }

        [Test]
        public void ExecuteSqlInterpolated_SpecifiedSqlThatDoesNotMatchSetUp_ThrowsException()
        {
            var sql = (FormattableString) $"asdf";
            var expectedResult = 1;
            AddExecuteSqlInterpolatedResult(MockedDbContext, sql, expectedResult);

            Assert.Throws<InvalidOperationException>(() =>
            {
                var actualResult = MockedDbContext.Database.ExecuteSqlInterpolated($"sp_NoParams");
            });
        }

        [Test]
        public void ExecuteSqlInterpolated_WithNoMatchesAdded_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var actualResult = MockedDbContext.Database.ExecuteSqlInterpolated($"sp_NoParams");
            });
        }

        [Test]
        public async Task ExecuteSqlInterpolatedAsync_AnySql_ReturnsExpectedResult()
        {
            var expectedResult = 1;
            AddExecuteSqlInterpolatedResult(MockedDbContext, expectedResult);

            var actualResult1 = await MockedDbContext.Database.ExecuteSqlInterpolatedAsync($"sp_NoParams");
            var actualResult2 = await MockedDbContext.Database.ExecuteSqlInterpolatedAsync($"sp_NoParams");

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }

        [Test]
        public async Task ExecuteSqlInterpolatedAsync_SpecifiedFormattableStringWithSqlParameterParameters_ReturnsExpectedResult()
        {
            var sql = "sp_WithParams";
            var parameters = new List<SqlParameter> { new SqlParameter("@SomeParameter2", "Value2") };
            var expectedResult = 1;
            AddExecuteSqlInterpolatedResult(MockedDbContext, sql, parameters, expectedResult);

            var actualResult1 = await MockedDbContext.Database.ExecuteSqlInterpolatedAsync($"[dbo].[sp_WithParams] {parameters[0]} {Fixture.Create<string>()}");
            var actualResult2 = await MockedDbContext.Database.ExecuteSqlInterpolatedAsync($"[dbo].[sp_WithParams] {parameters[0]} {Fixture.Create<string>()}");

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }

        [Test]
        public async Task ExecuteSqlInterpolatedAsync_SpecifiedFormattableStringWithStringParameterParameters_ReturnsExpectedResult()
        {
            var sql = "sp_WithParams";
            var parameters = new List<string> { "Value2" };
            var expectedResult = 1;
            AddExecuteSqlInterpolatedResult(MockedDbContext, sql, parameters, expectedResult);

            var actualResult1 = await MockedDbContext.Database.ExecuteSqlInterpolatedAsync($"[dbo].[sp_WithParams] {parameters[0]} {Fixture.Create<string>()}");
            var actualResult2 = await MockedDbContext.Database.ExecuteSqlInterpolatedAsync($"[dbo].[sp_WithParams] {parameters[0]} {Fixture.Create<string>()}");

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }

        [Test]
        public async Task ExecuteSqlInterpolatedAsync_SpecifiedSql_ReturnsExpectedResult()
        {
            var sql = (FormattableString) $"sp_NoParams";
            var expectedResult = 1;
            AddExecuteSqlInterpolatedResult(MockedDbContext, sql, expectedResult);

            var actualResult1 = await MockedDbContext.Database.ExecuteSqlInterpolatedAsync($"sp_NoParams");
            var actualResult2 = await MockedDbContext.Database.ExecuteSqlInterpolatedAsync($"sp_NoParams");

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }

        [Test]
        public void ExecuteSqlInterpolatedAsync_SpecifiedSqlThatDoesNotMatchSetUp_ThrowsException()
        {
            var sql = (FormattableString) $"asdf";
            var expectedResult = 1;
            AddExecuteSqlInterpolatedResult(MockedDbContext, sql, expectedResult);

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                var actualResult = await MockedDbContext.Database.ExecuteSqlInterpolatedAsync($"sp_NoParams");
            });
        }

        [Test]
        public void ExecuteSqlInterpolatedUsingFormattableStringSetUp_SpecifiedFormattableStringWithStringParameterParameters_ReturnsExpectedResult()
        {
            var parameters = new List<string> { "Value2" };
            var sql = (FormattableString) $"[sp_WithParams] {parameters[0]}";
            var expectedResult = 1;
            AddExecuteSqlInterpolatedResult(MockedDbContext, sql, expectedResult);

            var actualResult1 = MockedDbContext.Database.ExecuteSqlInterpolated($"[dbo].[sp_WithParams] {parameters[0]} {Fixture.Create<string>()}");
            var actualResult2 = MockedDbContext.Database.ExecuteSqlInterpolated($"[dbo].[sp_WithParams] {parameters[0]} {Fixture.Create<string>()}");

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }

        [Test]
        public void ExecuteSqlInterpolatedUsingFormattableStringSetUp_SpecifiedFormattableStringWithStringParameterParametersThatDoesNotMatchSetUp_ThrowsException()
        {
            var parameters = new List<string> { "Value2" };
            var sql = (FormattableString) $"[sp_WithParams] {parameters[0]}";
            var expectedResult = 1;
            AddExecuteSqlInterpolatedResult(MockedDbContext, sql, expectedResult);

            var actualResult1 = MockedDbContext.Database.ExecuteSqlInterpolated($"[dbo].[sp_WithParams] {Fixture.Create<string>()} {parameters[0]}");
            var actualResult2 = MockedDbContext.Database.ExecuteSqlInterpolated($"[dbo].[sp_WithParams] {Fixture.Create<string>()} {parameters[0]}");

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }

        [Test]
        public void ExecuteSqlInterpolatedUsingSqlAndParameterSetUp_SpecifiedFormattableStringWithStringParameterParameters_ReturnsExpectedResult()
        {
            var sql = "sp_WithParams";
            var parameters = new List<string> { "Value2" };
            var expectedResult = 1;
            AddExecuteSqlInterpolatedResult(MockedDbContext, sql, parameters, expectedResult);

            var actualResult1 = MockedDbContext.Database.ExecuteSqlInterpolated($"[dbo].[sp_WithParams] {parameters[0]} {Fixture.Create<string>()}");
            var actualResult2 = MockedDbContext.Database.ExecuteSqlInterpolated($"[dbo].[sp_WithParams] {parameters[0]} {Fixture.Create<string>()}");

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }

        [Test]
        public void ExecuteSqlRaw_AnySql_ReturnsExpectedResult()
        {
            var expectedResult = 1;
            AddExecuteSqlRawResult(MockedDbContext, expectedResult);

            var actualResult1 = MockedDbContext.Database.ExecuteSqlRaw("sp_NoParams");
            var actualResult2 = MockedDbContext.Database.ExecuteSqlRaw("sp_NoParams");

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }

        [Test]
        public void ExecuteSqlRaw_SpecifiedSql_ReturnsExpectedResult()
        {
            var sql = "sp_NoParams";
            var expectedResult = 1;
            AddExecuteSqlRawResult(MockedDbContext, sql, expectedResult);

            var actualResult1 = MockedDbContext.Database.ExecuteSqlRaw("sp_NoParams");
            var actualResult2 = MockedDbContext.Database.ExecuteSqlRaw("sp_NoParams");

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }

        [Test]
        public void ExecuteSqlRaw_SpecifiedSqlThatDoesNotMatchSetUp_ThrowsException()
        {
            var sql = "asdf";
            var expectedResult = 1;
            AddExecuteSqlRawResult(MockedDbContext, sql, expectedResult);

            Assert.Throws<InvalidOperationException>(() =>
            {
                var actualResult = MockedDbContext.Database.ExecuteSqlRaw("sp_NoParams");
            });
        }

        [Test]
        public void ExecuteSqlRaw_SpecifiedSqlWithSqlParameterParameters_ReturnsExpectedResult()
        {
            var sql = "sp_WithParams";
            var parameters = new List<SqlParameter> { new SqlParameter("@SomeParameter2", "Value2") };
            var expectedResult = 1;
            AddExecuteSqlRawResult(MockedDbContext, sql, parameters, expectedResult);

            var actualResult1 = MockedDbContext.Database.ExecuteSqlRaw("[dbo].[sp_WithParams] @SomeParameter1 @SomeParameter2", parameters);
            var actualResult2 = MockedDbContext.Database.ExecuteSqlRaw("[dbo].[sp_WithParams] @SomeParameter1 @SomeParameter2", parameters);

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }

        [Test]
        public void ExecuteSqlRaw_SpecifiedSqlWithSqlParameterParametersThatDoNotMatchSetUp_ThrowsException()
        {
            var sql = "sp_WithParams";
            var setUpParameters = new List<SqlParameter> { new SqlParameter("@SomeParameter3", "Value3") };
            var invocationParameters = new List<SqlParameter> { new SqlParameter("@SomeParameter1", "Value1"), new SqlParameter("@SomeParameter2", "Value2") };
            var expectedResult = 1;
            AddExecuteSqlRawResult(MockedDbContext, sql, setUpParameters, expectedResult);

            Assert.Throws<InvalidOperationException>(() =>
            {
                var actualResult1 = MockedDbContext.Database.ExecuteSqlRaw("[dbo].[sp_WithParams] @SomeParameter1 @SomeParameter2", invocationParameters);
            });

            Assert.Throws<InvalidOperationException>(() =>
            {
                var actualResult2 = MockedDbContext.Database.ExecuteSqlRaw("[dbo].[sp_WithParams] @SomeParameter1 @SomeParameter2", invocationParameters);
            });
        }

        [Test]
        public void ExecuteSqlRaw_SpecifiedSqlWithStringParameterParameters_ReturnsExpectedResult()
        {
            var sql = "sp_WithParams";
            var parameters = new List<string> { "Value2" };
            var expectedResult = 1;
            AddExecuteSqlRawResult(MockedDbContext, sql, parameters, expectedResult);

            var actualResult1 = MockedDbContext.Database.ExecuteSqlRaw("[dbo].[sp_WithParams] @SomeParameter1 @SomeParameter2", parameters);
            var actualResult2 = MockedDbContext.Database.ExecuteSqlRaw("[dbo].[sp_WithParams] @SomeParameter1 @SomeParameter2", parameters);

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }

        [Test]
        public void ExecuteSqlRaw_WithNoMatchesAdded_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var actualResult = MockedDbContext.Database.ExecuteSqlRaw("sp_NoParams");
            });
        }

        [Test]
        public async Task ExecuteSqlRawAsync_AnySql_ReturnsExpectedResult()
        {
            var expectedResult = 1;
            AddExecuteSqlRawResult(MockedDbContext, expectedResult);

            var actualResult1 = await MockedDbContext.Database.ExecuteSqlRawAsync("sp_NoParams");
            var actualResult2 = await MockedDbContext.Database.ExecuteSqlRawAsync("sp_NoParams");

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }

        [Test]
        public async Task ExecuteSqlRawAsync_SpecifiedSql_ReturnsExpectedResult()
        {
            var sql = "sp_NoParams";
            var expectedResult = 1;
            AddExecuteSqlRawResult(MockedDbContext, sql, expectedResult);

            var actualResult1 = await MockedDbContext.Database.ExecuteSqlRawAsync("sp_NoParams");
            var actualResult2 = await MockedDbContext.Database.ExecuteSqlRawAsync("sp_NoParams");

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }

        [Test]
        public void ExecuteSqlRawAsync_SpecifiedSqlThatDoesNotMatchSetUp_ThrowsException()
        {
            var sql = "asdf";
            var expectedResult = 1;
            AddExecuteSqlRawResult(MockedDbContext, sql, expectedResult);

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                var actualResult = await MockedDbContext.Database.ExecuteSqlRawAsync("sp_NoParams");
            });
        }

        [Test]
        public async Task ExecuteSqlRawAsync_SpecifiedSqlWithSqlParameterParameters_ReturnsExpectedResult()
        {
            var sql = "sp_WithParams";
            var parameters = new List<SqlParameter> { new SqlParameter("@SomeParameter2", "Value2") };
            var expectedResult = 1;
            AddExecuteSqlRawResult(MockedDbContext, sql, parameters, expectedResult);

            var actualResult1 = await MockedDbContext.Database.ExecuteSqlRawAsync("[dbo].[sp_WithParams] @SomeParameter2", parameters);
            var actualResult2 = await MockedDbContext.Database.ExecuteSqlRawAsync("[dbo].[sp_WithParams] @SomeParameter2", parameters);

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }

        [Test]
        public async Task ExecuteSqlRawAsync_SpecifiedSqlWithStringParameterParameters_ReturnsExpectedResult()
        {
            var sql = "sp_WithParams";
            var parameters = new List<string> { "Value2" };
            var expectedResult = 1;
            AddExecuteSqlRawResult(MockedDbContext, sql, parameters, expectedResult);

            var actualResult1 = await MockedDbContext.Database.ExecuteSqlRawAsync("[dbo].[sp_WithParams] @SomeParameter2", parameters);
            var actualResult2 = await MockedDbContext.Database.ExecuteSqlRawAsync("[dbo].[sp_WithParams] @SomeParameter2", parameters);

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }
    }
}
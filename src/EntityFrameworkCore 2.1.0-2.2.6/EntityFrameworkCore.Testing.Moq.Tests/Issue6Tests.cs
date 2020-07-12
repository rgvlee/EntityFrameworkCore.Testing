using System;
using System.Collections.Generic;
using AutoFixture;
using EntityFrameworkCore.Testing.Common.Tests;
using EntityFrameworkCore.Testing.Moq.Extensions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Moq.Tests
{
    public class Issue6Tests : BaseForTests
    {
        private static IEnumerable<TestCaseData> FromSql_SpecifiedFormattableStringSqlWithNullParameters_TestCases {
            get
            {
                yield return new TestCaseData(null, null);
                yield return new TestCaseData(null, 1);
                yield return new TestCaseData(DateTime.Parse("21 May 2020 9:05 PM"), null);
            }
        }

        [Test]
        public void FromSql_SpecifiedFormattableStringSqlWithDbNullParameters_ReturnsExpectedResult()
        {
            var expectedResult = new List<TestEntity> { Fixture.Create<TestEntity>() };

            var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();
            mockedDbContext.Set<TestEntity>().AddFromSqlResult($"SELECT * FROM [SqlFunctionWithNullableParameters]({DBNull.Value}, {DBNull.Value})", expectedResult);

            var actualResult = mockedDbContext.Set<TestEntity>().FromSql($"SELECT * FROM [SqlFunctionWithNullableParameters]({DBNull.Value}, {DBNull.Value})");

            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [TestCaseSource(nameof(FromSql_SpecifiedFormattableStringSqlWithNullParameters_TestCases))]
        public void FromSql_SpecifiedFormattableStringSqlWithNullParameters_ReturnsExpectedResult(DateTime? dateTimeValue, int? intValue)
        {
            var expectedResult = new List<TestEntity> { Fixture.Create<TestEntity>() };

            var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();
            mockedDbContext.Set<TestEntity>().AddFromSqlResult($"SELECT * FROM [SqlFunctionWithNullableParameters]({dateTimeValue}, {intValue})", expectedResult);

            var actualResult = mockedDbContext.Set<TestEntity>().FromSql($"SELECT * FROM [SqlFunctionWithNullableParameters]({dateTimeValue}, {intValue})");

            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }
    }
}
using System;
using System.Collections.Generic;
using AutoFixture;
using EntityFrameworkCore.Testing.Common.Helpers;
using EntityFrameworkCore.Testing.Common.Tests;
using EntityFrameworkCore.Testing.NSubstitute.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.NSubstitute.Tests
{
    public class Issue6Tests
    {
        private Fixture _fixture;

        private static IEnumerable<TestCaseData> FromSqlInterpolated_SpecifiedSqlWithNullParameters_TestCases {
            get
            {
                yield return new TestCaseData(null, null);
                yield return new TestCaseData(null, 1);
                yield return new TestCaseData(DateTime.Parse("21 May 2020 9:05 PM"), null);
            }
        }

        [SetUp]
        public virtual void SetUp()
        {
            LoggerHelper.LoggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _fixture = new Fixture();
        }

        [Test]
        public void FromSqlInterpolated_SpecifiedSqlWithDbNullParameters_ReturnsExpectedResult()
        {
            var expectedResult = new List<TestEntity> { _fixture.Create<TestEntity>() };

            var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();
            mockedDbContext.Set<TestEntity>().AddFromSqlInterpolatedResult($"SELECT * FROM [SqlFunctionWithNullableParameters]({DBNull.Value}, {DBNull.Value})", expectedResult);

            var actualResult = mockedDbContext.Set<TestEntity>().FromSqlInterpolated($"SELECT * FROM [SqlFunctionWithNullableParameters]({DBNull.Value}, {DBNull.Value})");

            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [TestCaseSource(nameof(FromSqlInterpolated_SpecifiedSqlWithNullParameters_TestCases))]
        public void FromSqlInterpolated_SpecifiedSqlWithNullParameters_ReturnsExpectedResult(DateTime? dateTimeValue, int? intValue)
        {
            var expectedResult = new List<TestEntity> { _fixture.Create<TestEntity>() };

            var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();
            mockedDbContext.Set<TestEntity>().AddFromSqlInterpolatedResult($"SELECT * FROM [SqlFunctionWithNullableParameters]({dateTimeValue}, {intValue})", expectedResult);

            var actualResult = mockedDbContext.Set<TestEntity>().FromSqlInterpolated($"SELECT * FROM [SqlFunctionWithNullableParameters]({dateTimeValue}, {intValue})");

            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }
    }
}
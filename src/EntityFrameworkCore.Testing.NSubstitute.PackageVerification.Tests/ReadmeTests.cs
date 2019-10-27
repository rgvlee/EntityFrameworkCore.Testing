using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using AutoFixture;
using EntityFrameworkCore.Testing.Common.Helpers;
using EntityFrameworkCore.Testing.Common.Tests;
using EntityFrameworkCore.Testing.NSubstitute.Extensions;
using EntityFrameworkCore.Testing.NSubstitute.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.NSubstitute.PackageVerification.Tests
{
    public class Tests
    {
        public Fixture Fixture = new Fixture();

        [SetUp]
        public virtual void SetUp()
        {
            LoggerHelper.LoggerFactory.AddConsole(LogLevel.Debug);
        }

        [Test]
        public void SetAddAndPersist_Item_Persists()
        {
            var dbContextToMock = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            var mockedContext = Create.SubstituteFor(dbContextToMock);

            var testEntity1 = Fixture.Create<TestEntity1>();

            mockedContext.Set<TestEntity1>().Add(testEntity1);
            mockedContext.SaveChanges();

            Assert.Multiple(() =>
            {
                Assert.AreNotEqual(default(Guid), testEntity1.Guid);
                Assert.DoesNotThrow(() => mockedContext.Set<TestEntity1>().Single());
                Assert.AreEqual(testEntity1, mockedContext.Find<TestEntity1>(testEntity1.Guid));
                mockedContext.Received(1).SaveChanges();
            });
        }

        [Test]
        public void SetUpFromSqlResult_AnyStoredProcedureWithNoParameters_ReturnsExpectedResult()
        {
            var dbContextToMock = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            var mockedContext = Create.SubstituteFor(dbContextToMock);

            var expectedResult = Fixture.CreateMany<TestEntity1>().ToList();

            mockedContext.Set<TestEntity1>().AddFromSqlResult(expectedResult);

            var actualResult = mockedContext.Set<TestEntity1>().FromSql("sp_NoParams").ToList();

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(actualResult);
                Assert.IsTrue(actualResult.Any());
                CollectionAssert.AreEquivalent(expectedResult, actualResult);
            });
        }

        [Test]
        public void SetUpFromSql_SpecifiedStoredProcedureAndParameters_ReturnsExpectedResult()
        {
            var dbContextToMock = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            var mockedContext = Create.SubstituteFor(dbContextToMock);

            var sqlParameters = new List<SqlParameter> {new SqlParameter("@SomeParameter2", "Value2")};
            var expectedResult = Fixture.CreateMany<TestEntity1>().ToList();

            mockedContext.Set<TestEntity1>().AddFromSqlResult("sp_Specified", sqlParameters, expectedResult);

            var actualResult = mockedContext.Set<TestEntity1>().FromSql("[dbo].[sp_Specified] @SomeParameter1 @SomeParameter2", new SqlParameter("@someparameter2", "Value2")).ToList();

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(actualResult);
                Assert.IsTrue(actualResult.Any());
                CollectionAssert.AreEquivalent(expectedResult, actualResult);
            });
        }

        [Test]
        public void QueryAddRange_Enumeration_AddsToQuerySource()
        {
            var dbContextToMock = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            var mockedContext = Create.SubstituteFor(dbContextToMock);

            var expectedResult = Fixture.CreateMany<TestQuery1>().ToList();

            mockedContext.Query<TestQuery1>().AddRange(expectedResult);

            Assert.Multiple(() =>
            {
                CollectionAssert.AreEquivalent(expectedResult, mockedContext.Query<TestQuery1>().ToList());
                CollectionAssert.AreEquivalent(mockedContext.Query<TestQuery1>().ToList(), mockedContext.TestView.ToList());
            });
        }

        [Test]
        public void ExecuteSqlCommand_SpecifiedStoredProcedure_ReturnsExpectedResult()
        {
            var dbContextToMock = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            var mockedContext = Create.SubstituteFor(dbContextToMock);

            var commandText = "sp_NoParams";
            var expectedResult = 1;

            mockedContext.AddExecuteSqlCommandResult(commandText, new List<SqlParameter>(), expectedResult);

            var result = mockedContext.Database.ExecuteSqlCommand("sp_NoParams");

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void ExecuteSqlCommand_SpecifiedStoredProcedureAndSqlParameters_ReturnsExpectedResult()
        {
            var dbContextToMock = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            var mockedContext = Create.SubstituteFor(dbContextToMock);

            var commandText = "sp_WithParams";
            var sqlParameters = new List<SqlParameter> {new SqlParameter("@SomeParameter2", "Value2")};
            var expectedResult = 1;

            mockedContext.AddExecuteSqlCommandResult(commandText, sqlParameters, expectedResult);

            var result = mockedContext.Database.ExecuteSqlCommand("[dbo.[sp_WithParams] @SomeParameter2", sqlParameters);

            Assert.AreEqual(expectedResult, result);
        }
    }
}
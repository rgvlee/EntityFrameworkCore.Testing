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
    public class ReadmeTests
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
            var mockedDbContext = Create.SubstituteDbContextFor(dbContextToMock);

            var testEntity = Fixture.Create<TestEntity>();

            mockedDbContext.Set<TestEntity>().Add(testEntity);
            mockedDbContext.SaveChanges();

            Assert.Multiple(() =>
            {
                Assert.AreNotEqual(default(Guid), testEntity.Guid);
                Assert.DoesNotThrow(() => mockedDbContext.Set<TestEntity>().Single());
                Assert.AreEqual(testEntity, mockedDbContext.Find<TestEntity>(testEntity.Guid));
            });
        }

        [Test]
        public void FromSql_AnyStoredProcedureWithNoParameters_ReturnsExpectedResult()
        {
            var dbContextToMock = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            var mockedDbContext = Create.SubstituteDbContextFor(dbContextToMock);

            var expectedResult = Fixture.CreateMany<TestEntity>().ToList();

            mockedDbContext.Set<TestEntity>().AddFromSqlResult(expectedResult);

            var actualResult = mockedDbContext.Set<TestEntity>().FromSql("sp_NoParams").ToList();

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(actualResult);
                Assert.IsTrue(actualResult.Any());
                CollectionAssert.AreEquivalent(expectedResult, actualResult);
            });
        }

        [Test]
        public void FromSql_SpecifiedStoredProcedureAndParameters_ReturnsExpectedResult()
        {
            var dbContextToMock = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            var mockedDbContext = Create.SubstituteDbContextFor(dbContextToMock);

            var sqlParameters = new List<SqlParameter> {new SqlParameter("@SomeParameter2", "Value2")};
            var expectedResult = Fixture.CreateMany<TestEntity>().ToList();

            mockedDbContext.Set<TestEntity>().AddFromSqlResult("sp_Specified", sqlParameters, expectedResult);

            var actualResult = mockedDbContext.Set<TestEntity>().FromSql("[dbo].[sp_Specified] @SomeParameter1 @SomeParameter2", new SqlParameter("@someparameter2", "Value2")).ToList();

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
            var mockedDbContext = Create.SubstituteDbContextFor(dbContextToMock);

            var expectedResult = Fixture.CreateMany<TestQuery>().ToList();

            mockedDbContext.Query<TestQuery>().AddRangeToReadOnlySource(expectedResult);

            Assert.Multiple(() =>
            {
                CollectionAssert.AreEquivalent(expectedResult, mockedDbContext.Query<TestQuery>().ToList());
                CollectionAssert.AreEquivalent(mockedDbContext.Query<TestQuery>().ToList(), mockedDbContext.TestView.ToList());
            });
        }

        [Test]
        public void ExecuteSqlCommand_SpecifiedStoredProcedure_ReturnsExpectedResult()
        {
            var dbContextToMock = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            var mockedDbContext = Create.SubstituteDbContextFor(dbContextToMock);

            var commandText = "sp_NoParams";
            var expectedResult = 1;

            mockedDbContext.AddExecuteSqlCommandResult(commandText, new List<SqlParameter>(), expectedResult);

            var result = mockedDbContext.Database.ExecuteSqlCommand("sp_NoParams");

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void ExecuteSqlCommand_SpecifiedStoredProcedureAndSqlParameters_ReturnsExpectedResult()
        {
            var dbContextToMock = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            var mockedDbContext = Create.SubstituteDbContextFor(dbContextToMock);

            var commandText = "sp_WithParams";
            var sqlParameters = new List<SqlParameter> {new SqlParameter("@SomeParameter2", "Value2")};
            var expectedResult = 1;

            mockedDbContext.AddExecuteSqlCommandResult(commandText, sqlParameters, expectedResult);

            var result = mockedDbContext.Database.ExecuteSqlCommand("[dbo].[sp_WithParams] @SomeParameter2", sqlParameters);

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void AddRangeThenSaveChanges_CanAssertInvocationCount()
        {
            var mockedDbContext = Create.SubstituteDbContextFor<TestDbContext>();

            mockedDbContext.Set<TestEntity>().AddRange(Fixture.CreateMany<TestEntity>().ToList());
            mockedDbContext.SaveChanges();

            Assert.Multiple(() =>
            {
                mockedDbContext.Received(1).SaveChanges();

                //The db set is a mock, so we need to ensure we invoke the verify on the db set mock
                mockedDbContext.Set<TestEntity>().Received(1).AddRange(Arg.Any<IEnumerable<TestEntity>>());

                //This is the same mock instance as above, just accessed a different way
                mockedDbContext.TestEntities.Received(1).AddRange(Arg.Any<IEnumerable<TestEntity>>());

                Assert.That(mockedDbContext.TestEntities, Is.SameAs(mockedDbContext.Set<TestEntity>()));
            });
        }
    }
}
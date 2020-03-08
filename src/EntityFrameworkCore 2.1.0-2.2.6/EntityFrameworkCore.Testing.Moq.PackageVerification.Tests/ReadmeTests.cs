using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using AutoFixture;
using EntityFrameworkCore.Testing.Common.Helpers;
using EntityFrameworkCore.Testing.Common.Tests;
using EntityFrameworkCore.Testing.Moq.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Moq.PackageVerification.Tests
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
        public void Method_WithSpecifiedInput_ReturnsAResult()
        {
            var mockedDbContext = Create.MockedDbContextFor<MyDbContext>();

            //...
        }

        [Test]
        public void AnotherMethod_WithSpecifiedInput_ReturnsAResult()
        {
            var mockedDbContext = Create.MockedDbContextFor<MyDbContextWithConstructorParameters>(Mock.Of<ILogger<MyDbContextWithConstructorParameters>>(),
                new DbContextOptionsBuilder<MyDbContextWithConstructorParameters>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

            //...
        }

        [Test]
        public void ExecuteSqlCommandWithCallback_InvokesCallback()
        {
            //Arrange
            var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();

            var itemsToCreate = 100;
            mockedDbContext.Set<TestEntity>().AddRange(Fixture.CreateMany<TestEntity>(itemsToCreate).ToList());
            mockedDbContext.SaveChanges();

            var numberOfRowsToDelete = itemsToCreate / 2;
            var rowsToDelete = mockedDbContext.Set<TestEntity>().Take(numberOfRowsToDelete).ToList();
            var remainingRows = mockedDbContext.Set<TestEntity>().Skip(numberOfRowsToDelete).ToList();

            mockedDbContext.AddExecuteSqlCommandResult("usp_MyStoredProc",
                numberOfRowsToDelete,
                (providedSql, providedParameters) =>
                {
                    mockedDbContext.Set<TestEntity>().RemoveRange(rowsToDelete);
                    mockedDbContext.SaveChanges();
                });

            //Act
            var actualResult = mockedDbContext.Database.ExecuteSqlCommand($"usp_MyStoredProc {numberOfRowsToDelete}");

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(actualResult, Is.EqualTo(numberOfRowsToDelete));
                Assert.That(mockedDbContext.Set<TestEntity>().Count(), Is.EqualTo(itemsToCreate - numberOfRowsToDelete));
                Assert.That(mockedDbContext.Set<TestEntity>().ToList(), Is.EquivalentTo(remainingRows));
            });
        }

        [Test]
        public void SetAddAndPersist_Item_AddsAndPersistsItem()
        {
            var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();

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
            var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();

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
            var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();

            var sqlParameters = new List<SqlParameter> { new SqlParameter("@SomeParameter2", "Value2") };
            var expectedResult = Fixture.CreateMany<TestEntity>().ToList();

            mockedDbContext.Set<TestEntity>().AddFromSqlResult("sp_Specified", sqlParameters, expectedResult);

            var actualResult = mockedDbContext.Set<TestEntity>()
                .FromSql("[dbo].[sp_Specified] @SomeParameter1 @SomeParameter2", new SqlParameter("@someparameter1", "Value1"), new SqlParameter("@someparameter2", "Value2"))
                .ToList();

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(actualResult);
                Assert.IsTrue(actualResult.Any());
                CollectionAssert.AreEquivalent(expectedResult, actualResult);
            });
        }

        [Test]
        public void QueryAddRangeToReadOnlySource_Enumeration_AddsEnumerationElementsToQuerySource()
        {
            var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();

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
            var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();

            var commandText = "sp_NoParams";
            var expectedResult = 1;

            mockedDbContext.AddExecuteSqlCommandResult(commandText, new List<SqlParameter>(), expectedResult);

            var result = mockedDbContext.Database.ExecuteSqlCommand("sp_NoParams");

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void ExecuteSqlCommand_SpecifiedStoredProcedureAndSqlParameters_ReturnsExpectedResult()
        {
            var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();

            var commandText = "sp_WithParams";
            var sqlParameters = new List<SqlParameter> { new SqlParameter("@SomeParameter2", "Value2") };
            var expectedResult = 1;

            mockedDbContext.AddExecuteSqlCommandResult(commandText, sqlParameters, expectedResult);

            var result = mockedDbContext.Database.ExecuteSqlCommand("[dbo].[sp_WithParams] @SomeParameter2", sqlParameters);

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void AddRangeThenSaveChanges_CanAssertInvocationCount()
        {
            var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();

            mockedDbContext.Set<TestEntity>().AddRange(Fixture.CreateMany<TestEntity>().ToList());
            mockedDbContext.SaveChanges();

            Assert.Multiple(() =>
            {
                var dbContextMock = Mock.Get(mockedDbContext);
                dbContextMock.Verify(m => m.SaveChanges(), Times.Once);

                //The db set is a mock, so we need to ensure we invoke the verify on the db set mock
                var byTypeDbSetMock = Mock.Get(mockedDbContext.Set<TestEntity>());
                byTypeDbSetMock.Verify(m => m.AddRange(It.IsAny<IEnumerable<TestEntity>>()), Times.Once);

                //This is the same mock instance as above, just accessed a different way
                var byPropertyDbSetMock = Mock.Get(mockedDbContext.TestEntities);

                Assert.That(byPropertyDbSetMock, Is.SameAs(byTypeDbSetMock));

                byPropertyDbSetMock.Verify(m => m.AddRange(It.IsAny<IEnumerable<TestEntity>>()), Times.Once);
            });
        }

        public class MyDbContext : DbContext
        {
            public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }
        }

        public class MyDbContextWithConstructorParameters : DbContext
        {
            public MyDbContextWithConstructorParameters(ILogger<MyDbContextWithConstructorParameters> logger, DbContextOptions<MyDbContextWithConstructorParameters> options) :
                base(options) { }
        }
    }
}
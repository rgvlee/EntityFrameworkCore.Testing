using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using AutoFixture;
using EntityFrameworkCore.Testing.Common.Tests;
using EntityFrameworkCore.Testing.NSubstitute.Extensions;
using EntityFrameworkCore.Testing.NSubstitute.Helpers;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using rgvlee.Core.Common.Helpers;

namespace EntityFrameworkCore.Testing.NSubstitute.PackageVerification.Tests
{
    public class ReadmeTests
    {
        public Fixture Fixture = new Fixture();

        [SetUp]
        public virtual void SetUp()
        {
            LoggingHelper.LoggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug));
        }

        [Test]
        public void UsageExample1()
        {
            var testEntity = Fixture.Create<TestEntity>();
            var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();
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
        public void UsageExample3()
        {
            var expectedResult = Fixture.CreateMany<TestEntity>().ToList();
            var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();
            mockedDbContext.Set<TestEntity>().AddFromSqlRawResult(expectedResult);

            var actualResult = mockedDbContext.Set<TestEntity>().FromSqlRaw("[dbo].[USP_StoredProcedureWithNoParameters]").ToList();

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(actualResult);
                Assert.IsTrue(actualResult.Any());
                CollectionAssert.AreEquivalent(expectedResult, actualResult);
            });
        }

        [Test]
        public void UsageExample4()
        {
            var expectedResult = Fixture.CreateMany<TestEntity>().ToList();
            var sqlParameters = new List<SqlParameter> { new SqlParameter("@Parameter2", "Value2") };
            var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();
            mockedDbContext.Set<TestEntity>().AddFromSqlRawResult("usp_StoredProcedureWithParameters", sqlParameters, expectedResult);

            var actualResult = mockedDbContext.Set<TestEntity>()
                .FromSqlRaw("[dbo].[USP_StoredProcedureWithParameters] @Parameter1 @Parameter2",
                    new SqlParameter("@parameter1", "Value1"),
                    new SqlParameter("@parameter2", "value2"))
                .ToList();

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(actualResult);
                Assert.IsTrue(actualResult.Any());
                CollectionAssert.AreEquivalent(expectedResult, actualResult);
            });
        }

        [Test]
        public void UsageExample5()
        {
            var expectedResult = Fixture.CreateMany<TestEntity>().ToList();
            var parameter1 = Fixture.Create<DateTime>();
            var parameter2 = Fixture.Create<string>();
            var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();
            mockedDbContext.Set<TestEntity>().AddFromSqlInterpolatedResult($"usp_StoredProcedureWithParameters {parameter1}, {parameter2.ToUpper()}", expectedResult);

            var actualResult = mockedDbContext.Set<TestEntity>().FromSqlInterpolated($"USP_StoredProcedureWithParameters {parameter1}, {parameter2.ToLower()}").ToList();

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(actualResult);
                Assert.IsTrue(actualResult.Any());
                CollectionAssert.AreEquivalent(expectedResult, actualResult);
            });
        }

        [Test]
        public void UsageExample6()
        {
            var expectedResult = Fixture.CreateMany<ViewEntity>().ToList();
            var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();

            mockedDbContext.Query<ViewEntity>().AddRangeToReadOnlySource(expectedResult);

            Assert.Multiple(() =>
            {
                CollectionAssert.AreEquivalent(expectedResult, mockedDbContext.Query<ViewEntity>().ToList());
                CollectionAssert.AreEquivalent(mockedDbContext.Query<ViewEntity>().ToList(), mockedDbContext.ViewEntities.ToList());
            });
        }

        [Test]
        public void UsageExample7()
        {
            var commandText = "usp_StoredProcedureWithNoParameters";
            var expectedResult = 1;
            var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();
            mockedDbContext.AddExecuteSqlRawResult(commandText, new List<SqlParameter>(), expectedResult);

            var result = mockedDbContext.Database.ExecuteSqlRaw("USP_StoredProcedureWithNoParameters");

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void UsageExample8()
        {
            //Arrange
            var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();

            var itemsToCreate = 100;
            mockedDbContext.Set<TestEntity>().AddRange(Fixture.CreateMany<TestEntity>(itemsToCreate).ToList());
            mockedDbContext.SaveChanges();

            var numberOfRowsToDelete = itemsToCreate / 2;
            var rowsToDelete = mockedDbContext.Set<TestEntity>().Take(numberOfRowsToDelete).ToList();
            var remainingRows = mockedDbContext.Set<TestEntity>().Skip(numberOfRowsToDelete).ToList();

            mockedDbContext.AddExecuteSqlRawResult("usp_MyStoredProc",
                numberOfRowsToDelete,
                (providedSql, providedParameters) =>
                {
                    mockedDbContext.Set<TestEntity>().RemoveRange(rowsToDelete);
                    mockedDbContext.SaveChanges();
                });

            //Act
            var actualResult = mockedDbContext.Database.ExecuteSqlRaw($"usp_MyStoredProc {numberOfRowsToDelete}");

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(actualResult, Is.EqualTo(numberOfRowsToDelete));
                Assert.That(mockedDbContext.Set<TestEntity>().Count(), Is.EqualTo(itemsToCreate - numberOfRowsToDelete));
                Assert.That(mockedDbContext.Set<TestEntity>().ToList(), Is.EquivalentTo(remainingRows));
            });
        }

        [Test]
        public void CreateExample1()
        {
            var mockedLogger = Substitute.For<ILogger<TestDbContext>>();
            var dbContextOptions = new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            var mockedDbContext = Create.MockedDbContextFor<TestDbContext>(mockedLogger, dbContextOptions);
        }

        [Test]
        public void CreateExample2()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            var dbContextToMock = new TestDbContext(options);
            var mockedDbContext = new MockedDbContextBuilder<TestDbContext>().UseDbContext(dbContextToMock).UseConstructorWithParameters(options).MockedDbContext;
        }

        [Test]
        public void CreateExample3()
        {
            using (var connection = new SqliteConnection("Filename=:memory:"))
            {
                connection.Open();
                var testEntity = Fixture.Create<TestEntity>();
                var dbContextToMock = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseSqlite(connection).Options);
                dbContextToMock.Database.EnsureCreated();
                var mockedDbContext = new MockedDbContextBuilder<TestDbContext>().UseDbContext(dbContextToMock).MockedDbContext;

                mockedDbContext.Set<TestEntity>().Add(testEntity);
                mockedDbContext.SaveChanges();

                Assert.Multiple(() =>
                {
                    Assert.AreNotEqual(default(Guid), testEntity.Guid);
                    Assert.DoesNotThrow(() => mockedDbContext.Set<TestEntity>().Single());
                    Assert.AreEqual(testEntity, mockedDbContext.Find<TestEntity>(testEntity.Guid));
                });
            }
        }
    }
}
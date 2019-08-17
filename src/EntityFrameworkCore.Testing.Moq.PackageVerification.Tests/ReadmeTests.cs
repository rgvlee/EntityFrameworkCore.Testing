using EntityFrameworkCore.DbContextBackedMock.Moq;
using EntityFrameworkCore.DbContextBackedMock.Moq.Extensions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using EntityFrameworkCore.Testing.Common.Tests;

namespace EntityFrameworkCore.Testing.Moq.PackageVerification.Tests {
    public class Tests {
        //[SetUp]
        //public virtual void SetUp() {
        //    LoggerHelper.LoggerFactory.AddConsole(LogLevel.Debug);
        //}

        [Test]
        public void Add_NewEntity_Persists() {
            var builder = new DbContextMockBuilder<TestDbContext>();
            var mockContext = builder.GetDbContextMock();
            var mockedContext = builder.GetMockedDbContext();
            var testEntity1 = new TestEntity1();

            mockedContext.Set<TestEntity1>().Add(testEntity1);
            mockedContext.SaveChanges();

            Assert.Multiple(() => {
                Assert.AreNotEqual(default(Guid), testEntity1.Id);
                Assert.DoesNotThrow(() => mockedContext.Set<TestEntity1>().Single());
                Assert.AreEqual(testEntity1, mockedContext.Find<TestEntity1>(testEntity1.Id));
                mockContext.Verify(m => m.SaveChanges(), Times.Once);
            });
        }

        [Test]
        public void AddWithSpecifiedDbContextAndDbSetSetUp_NewEntity_PersistsToBothDbSetAndDbContextDbSetProperty() {
            var contextToMock = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            var builder = new DbContextMockBuilder<TestDbContext>(contextToMock, false);
            builder.AddSetUpFor(x => x.TestEntities);
            var mockedContext = builder.GetMockedDbContext();

            mockedContext.Set<TestEntity1>().Add(new TestEntity1());
            mockedContext.SaveChanges();

            Assert.Multiple(() => {
                Assert.DoesNotThrow(() => mockedContext.Set<TestEntity1>().Single());
                CollectionAssert.AreEquivalent(mockedContext.Set<TestEntity1>().ToList(), mockedContext.TestEntities.ToList());
            });
        }

        [Test]
        public void SetUpFromSql_AnyStoredProcedureWithNoParameters_ReturnsExpectedResult() {
            var expectedResult = new List<TestEntity1> { new TestEntity1() };

            var builder = new DbContextMockBuilder<TestDbContext>();
            builder.AddFromSqlResultFor(x => x.TestEntities, expectedResult);
            var mockedContext = builder.GetMockedDbContext();

            var actualResult = mockedContext.Set<TestEntity1>().FromSql("sp_NoParams").ToList();

            Assert.Multiple(() => {
                Assert.IsNotNull(actualResult);
                Assert.IsTrue(actualResult.Any());
                CollectionAssert.AreEquivalent(expectedResult, actualResult);
            });
        }

        [Test]
        public void SetUpFromSql_SpecifiedStoredProcedureAndParameters_ReturnsExpectedResult() {
            var sqlParameters = new List<SqlParameter>() { new SqlParameter("@SomeParameter2", "Value2") };
            var expectedResult = new List<TestEntity1> { new TestEntity1() };

            var builder = new DbContextMockBuilder<TestDbContext>();
            builder.AddFromSqlResultFor(x => x.TestEntities, "sp_Specified", sqlParameters, expectedResult);
            var mockedContext = builder.GetMockedDbContext();

            var actualResult = mockedContext.Set<TestEntity1>().FromSql("[dbo].[sp_Specified] @SomeParameter1 @SomeParameter2", new SqlParameter("@someparameter2", "Value2")).ToList();

            Assert.Multiple(() => {
                Assert.IsNotNull(actualResult);
                Assert.IsTrue(actualResult.Any());
                CollectionAssert.AreEquivalent(expectedResult, actualResult);
            });
        }

        [Test]
        public void SetUpFromSql_MockQueryProviderWithSpecifiedStoredProcedureAndParameters_ReturnsExpectedResult() {
            var expectedResult = new List<TestEntity1> { new TestEntity1() };

            var mockQueryProvider = new Mock<IQueryProvider>();
            var sqlParameter = new SqlParameter("@SomeParameter2", "Value2");
            mockQueryProvider.SetUpFromSql("sp_Specified", new List<SqlParameter> { sqlParameter }, expectedResult);

            var builder = new DbContextMockBuilder<TestDbContext>();
            builder.AddQueryProviderMockFor(x => x.TestEntities, mockQueryProvider);
            var mockedContext = builder.GetMockedDbContext();

            var actualResult = mockedContext.Set<TestEntity1>().FromSql("[dbo].[sp_Specified] @SomeParameter1 @SomeParameter2", new SqlParameter("@someparameter2", "Value2")).ToList();

            Assert.Multiple(() => {
                Assert.IsNotNull(actualResult);
                Assert.IsTrue(actualResult.Any());
                CollectionAssert.AreEquivalent(expectedResult, actualResult);
            });
        }

        [Test]
        public void SetUpQuery_ReturnsEnumeration() {
            var list1 = new List<TestEntity2>() { new TestEntity2(), new TestEntity2() };

            var builder = new DbContextMockBuilder<TestDbContext>();
            builder.AddSetUpFor(x => x.TestView, list1);
            var mockedContext = builder.GetMockedDbContext();

            Assert.Multiple(() => {
                CollectionAssert.AreEquivalent(list1, mockedContext.Query<TestEntity2>().ToList());
                CollectionAssert.AreEquivalent(mockedContext.Query<TestEntity2>().ToList(), mockedContext.TestView.ToList());
            });
        }

        [Test]
        public void FromSql_SpecifiedStoredProcedureWithParameters_ReturnsExpectedResult() {
            var list1 = new List<TestEntity2> { new TestEntity2() };

            var builder = new DbContextMockBuilder<TestDbContext>();

            var mockQueryProvider = new Mock<IQueryProvider>();
            var sqlParameter = new SqlParameter("@SomeParameter2", "Value2");
            mockQueryProvider.SetUpFromSql("sp_Specified", new List<SqlParameter> { sqlParameter }, list1);
            builder.AddSetUpFor(x => x.TestView, list1).AddQueryProviderMockFor(x => x.TestView, mockQueryProvider);

            var context = builder.GetMockedDbContext();

            var result = context.Query<TestEntity2>().FromSql("[dbo].[sp_Specified] @SomeParameter1 @SomeParameter2", new SqlParameter("@someparameter2", "Value2")).ToList();

            Assert.Multiple(() => {
                Assert.IsNotNull(result);
                Assert.IsTrue(result.Any());
                CollectionAssert.AreEquivalent(list1, result);
            });
        }

        [Test]
        public void Execute_SetUpSpecifiedQuery_ReturnsExpectedResult() {
            var builder = new DbContextMockBuilder<TestDbContext>();

            var commandText = "sp_NoParams";
            var expectedResult = 1;

            builder.AddExecuteSqlCommandResult(commandText, new List<SqlParameter>(), expectedResult);

            var mockedContext = builder.GetMockedDbContext();

            var result = mockedContext.Database.ExecuteSqlCommand("sp_NoParams");

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void Execute_SetUpSpecifiedQueryWithSqlParameters_ReturnsExpectedResult() {
            var builder = new DbContextMockBuilder<TestDbContext>();

            var commandText = "sp_WithParams";
            var sqlParameters = new List<SqlParameter>() { new SqlParameter("@SomeParameter2", "Value2") };
            var expectedResult = 1;

            builder.AddExecuteSqlCommandResult(commandText, sqlParameters, expectedResult);

            var mockedContext = builder.GetMockedDbContext();

            var result = mockedContext.Database.ExecuteSqlCommand("[dbo.[sp_WithParams] @SomeParameter2", sqlParameters);

            Assert.AreEqual(expectedResult, result);
        }
    }
}
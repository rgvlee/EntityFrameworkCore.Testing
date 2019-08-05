using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using EntityFrameworkCore.Testing.Moq.Extensions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace EntityFrameworkCore.Testing.Moq.Tests {
    [TestFixture]
    public class MockedDbSetTests {
        [Test]
        public void AddAndPersist_Entity_Persists() {
            var testEntity1 = new TestEntity1();

            var builder = new DbContextMockBuilder<TestContext>();
            var mockedContext = builder.GetMockedDbContext();
            
            mockedContext.Set<TestEntity1>().Add(testEntity1);
            mockedContext.SaveChanges();

            Assert.Multiple(() => {
                Assert.AreNotEqual(default(Guid), testEntity1.Id);
                Assert.AreEqual(testEntity1, mockedContext.Set<TestEntity1>().Single());
                Assert.AreEqual(testEntity1, mockedContext.TestEntities.Single());
            });
        }

        [Test]
        public void AddAndPersistWithSpecifiedDbContextAndDbSetSetUp_Entity_Persists() {
            var testEntity1 = new TestEntity1();

            var contextToMock = new TestContext(new DbContextOptionsBuilder<TestContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            var builder = new DbContextMockBuilder<TestContext>(contextToMock, false);
            builder.AddSetUpFor(x => x.TestEntities);
            var mockedContext = builder.GetMockedDbContext();

            mockedContext.Set<TestEntity1>().Add(testEntity1);
            mockedContext.SaveChanges();

            Assert.Multiple(() => {
                Assert.AreNotEqual(default(Guid), testEntity1.Id);
                Assert.AreEqual(testEntity1, mockedContext.Set<TestEntity1>().Single());
                Assert.AreEqual(testEntity1, mockedContext.TestEntities.Single());
            });
        }

        [Test]
        public async Task AddAndPersistAsync_Enumeration_Persists() {
            var testEntities = new List<TestEntity1>() { new TestEntity1(), new TestEntity1() };

            var builder = new DbContextMockBuilder<TestContext>();
            var mockedContext = builder.GetMockedDbContext();

            await mockedContext.Set<TestEntity1>().AddRangeAsync(testEntities);
            await mockedContext.SaveChangesAsync();
            
            var result = mockedContext.Set<TestEntity1>().ToList();

            Assert.Multiple(() => {
                CollectionAssert.AreEquivalent(testEntities, result);
                CollectionAssert.AreEquivalent(result, mockedContext.TestEntities.ToList());
            });
        }

        [Test]
        public void SetUpSet_ToList_ReturnsEnumeration() {
            var expectedResult = new List<TestEntity1>() { new TestEntity1(), new TestEntity1() };

            var builder = new DbContextMockBuilder<TestContext>();
            var mockedContext = builder.GetMockedDbContext();
            mockedContext.Set<TestEntity1>().AddRange(expectedResult);
            mockedContext.SaveChanges();

            var result1 = mockedContext.Set<TestEntity1>().ToList();
            var result2 = mockedContext.Set<TestEntity1>().ToList();

            Assert.Multiple(() => {
                CollectionAssert.AreEquivalent(result1, result2);
                CollectionAssert.AreEquivalent(expectedResult, result1);

                CollectionAssert.AreEquivalent(expectedResult, mockedContext.TestEntities.ToList());
            });
        }

        [Test]
        public async Task SetUpSet_ToListAsync_ReturnsEnumeration() {
            var expectedResult = new List<TestEntity1>() { new TestEntity1(), new TestEntity1() };

            var builder = new DbContextMockBuilder<TestContext>();
            var mockedContext = builder.GetMockedDbContext();
            mockedContext.Set<TestEntity1>().AddRange(expectedResult);
            mockedContext.SaveChanges();

            var result1 = await mockedContext.Set<TestEntity1>().ToListAsync();
            var result2 = await mockedContext.Set<TestEntity1>().ToListAsync();

            Assert.Multiple(() => {
                CollectionAssert.AreEquivalent(result1, result2);
                CollectionAssert.AreEquivalent(expectedResult, result1);

                CollectionAssert.AreEquivalent(expectedResult, mockedContext.TestEntities.ToList());
            });
        }

        [Test]
        public void SetUpSet_Any_ReturnsEnumeration() {
            var testEntities = new List<TestEntity1>() { new TestEntity1(), new TestEntity1() };

            var builder = new DbContextMockBuilder<TestContext>();
            var mockedContext = builder.GetMockedDbContext();
            mockedContext.Set<TestEntity1>().AddRange(testEntities);
            mockedContext.SaveChanges();

            var result1 = mockedContext.Set<TestEntity1>().Any();
            var result2 = mockedContext.Set<TestEntity1>().Any();

            Assert.Multiple(() => {
                Assert.IsTrue(result1);
                Assert.IsTrue(result2);
            });
        }

        [Test]
        public async Task SetUpSet_AnyAsync_ReturnsEnumeration() {
            var testEntities = new List<TestEntity1>() { new TestEntity1(), new TestEntity1() };

            var builder = new DbContextMockBuilder<TestContext>();
            var mockedContext = builder.GetMockedDbContext();
            mockedContext.Set<TestEntity1>().AddRange(testEntities);
            mockedContext.SaveChanges();

            var result1 = await mockedContext.Set<TestEntity1>().AnyAsync();
            var result2 = await mockedContext.Set<TestEntity1>().AnyAsync();

            Assert.Multiple(() => {
                Assert.IsTrue(result1);
                Assert.IsTrue(result2);
            });
        }

        [Test]
        public void SetUpFromSql_AnyStoredProcedureWithNoParametersToList_ReturnsExpectedResult() {
            var expectedResult = new List<TestEntity1> { new TestEntity1() };

            var builder = new DbContextMockBuilder<TestContext>();
            builder.AddFromSqlResultFor(x => x.TestEntities, expectedResult);
            var mockedContext = builder.GetMockedDbContext();

            var actualResult1 = mockedContext.Set<TestEntity1>().FromSql("sp_NoParams").ToList();
            var actualResult2 = mockedContext.Set<TestEntity1>().FromSql("sp_NoParams").ToList();

            Assert.Multiple(() => {
                CollectionAssert.AreEquivalent(expectedResult, actualResult1);
                CollectionAssert.AreEquivalent(actualResult1, actualResult2);
            });
        }

        [Test]
        public void SetUpFromSql_SpecifiedStoredProcedureAndParametersToList_ReturnsExpectedResult() {
            var expectedResult = new List<TestEntity1> { new TestEntity1() };
            var sqlParameters = new List<SqlParameter>() { new SqlParameter("@SomeParameter2", "Value2") };
            
            var builder = new DbContextMockBuilder<TestContext>();
            builder.AddFromSqlResultFor(x => x.TestEntities, "sp_Specified", sqlParameters, expectedResult);
            var mockedContext = builder.GetMockedDbContext();

            var actualResult1 = mockedContext.Set<TestEntity1>().FromSql("[dbo].[sp_Specified] @SomeParameter1 @SomeParameter2", new SqlParameter("@someparameter2", "Value2")).ToList();
            var actualResult2 = mockedContext.Set<TestEntity1>().FromSql("[dbo].[sp_Specified] @SomeParameter1 @SomeParameter2", new SqlParameter("@someparameter2", "Value2")).ToList();

            Assert.Multiple(() => {
                CollectionAssert.AreEquivalent(expectedResult, actualResult1);
                CollectionAssert.AreEquivalent(actualResult1, actualResult2);
            });
        }

        [Test]
        public void SetUpFromSql_MockQueryProviderWithSpecifiedStoredProcedureAndParametersToList_ReturnsExpectedResult() {
            var expectedResult = new List<TestEntity1> { new TestEntity1() };
            var sqlParameters = new List<SqlParameter>() { new SqlParameter("@SomeParameter2", "Value2") };
            
            var mockQueryProvider = new Mock<IQueryProvider>();
            mockQueryProvider.SetUpFromSql("sp_Specified", sqlParameters, expectedResult);

            var builder = new DbContextMockBuilder<TestContext>();
            builder.AddQueryProviderMockFor(x => x.TestEntities, mockQueryProvider);
            var mockedContext = builder.GetMockedDbContext();

            var actualResult1 = mockedContext.Set<TestEntity1>().FromSql("[dbo].[sp_Specified] @SomeParameter1 @SomeParameter2", new SqlParameter("@someparameter2", "Value2")).ToList();
            var actualResult2 = mockedContext.Set<TestEntity1>().FromSql("[dbo].[sp_Specified] @SomeParameter1 @SomeParameter2", new SqlParameter("@someparameter2", "Value2")).ToList();

            Assert.Multiple(() => {
                CollectionAssert.AreEquivalent(expectedResult, actualResult1);
                CollectionAssert.AreEquivalent(actualResult1, actualResult2);
            });
        }

        [Test]
        public void SetUpFromSql_SpecifiedStoredProcedureWithInvalidParametersToList_ReturnsEmptyEnumeration() {
            var testEntity1 = new TestEntity1() { Id = Guid.NewGuid() };
            var sqlParameters = new List<SqlParameter>
            {
                new SqlParameter("@xxxapplicationName", "Test Application"),
                new SqlParameter("@xxxdate", DateTime.Today),
                new SqlParameter("@xxxids", testEntity1.Id),
                new SqlParameter("@xxxidsSeparator", ',')
            };
            
            var builder = new DbContextMockBuilder<TestContext>();
            builder.AddFromSqlResultFor(x => x.TestEntities, "ById", sqlParameters, new List<TestEntity1> { testEntity1 });
            var mockedContext = builder.GetMockedDbContext();

            var actualResult = mockedContext.Set<TestEntity1>().FromSql("[dbo].[sp_GetLocationsById] @applicationName, @date, @ids, @idsSeparator", 
                new List<SqlParameter>
                    {
                        new SqlParameter("@applicationName", "Test Application"),
                        new SqlParameter("@date", DateTime.Today),
                        new SqlParameter("@ids", testEntity1.Id),
                        new SqlParameter("@idsSeparator", ',')
                    }.ToArray()
                ).ToList();

            Assert.Multiple(() => {
                Assert.IsNotNull(actualResult);
                Assert.IsFalse(actualResult.Any());
            });
        }

        [Test]
        public void SetUpFromSql_SpecifiedStoredProcedureWithNullParameterValueToList_ReturnsExpectedResult() {
            var expectedResult = new List<TestEntity1> { new TestEntity1() };
            var sqlParameters = new List<SqlParameter>() { new SqlParameter("@SomeParameter2", SqlDbType.DateTime) };
            
            var builder = new DbContextMockBuilder<TestContext>();
            builder.AddFromSqlResultFor(x => x.TestEntities, "sp_Specified", sqlParameters, expectedResult);
            var mockedContext = builder.GetMockedDbContext();

            var actualResult1 = mockedContext.Set<TestEntity1>().FromSql("[dbo].[sp_Specified] @SomeParameter1 @SomeParameter2", new SqlParameter("@someparameter2", SqlDbType.DateTime)).ToList();
            var actualResult2 = mockedContext.Set<TestEntity1>().FromSql("[dbo].[sp_Specified] @SomeParameter1 @SomeParameter2", new SqlParameter("@someparameter2", SqlDbType.DateTime)).ToList();

            Assert.Multiple(() => {
                CollectionAssert.AreEquivalent(expectedResult, actualResult1);
                CollectionAssert.AreEquivalent(actualResult1, actualResult2);
            });
        }

        [Test]
        public void GetDbSetMock_ByType_ReturnsDbSetMock() {
            var expectedResult = new List<TestEntity1>() { new TestEntity1(), new TestEntity1() };

            var builder = new DbContextMockBuilder<TestContext>();
            var mockedContext = builder.GetMockedDbContext();
            mockedContext.Set<TestEntity1>().AddRange(expectedResult);
            mockedContext.SaveChanges();

            var dbSetMock = builder.GetDbSetMockFor<TestEntity1>();

            CollectionAssert.AreEquivalent(expectedResult, dbSetMock.Object.ToList());
        }

        [Test]
        public void GetDbSetMock_ByExpression_ReturnsDbSetMock() {
            var expectedResult = new List<TestEntity1>() { new TestEntity1(), new TestEntity1() };

            var builder = new DbContextMockBuilder<TestContext>();
            var mockedContext = builder.GetMockedDbContext();
            mockedContext.Set<TestEntity1>().AddRange(expectedResult);
            mockedContext.SaveChanges();

            var dbSetMock = builder.GetDbSetMockFor(x => x.TestEntities);

            CollectionAssert.AreEquivalent(expectedResult, dbSetMock.Object.ToList());
        }

        [Test]
        public void GetMockedDbSet_ByType_ReturnsDbSetMock() {
            var expectedResult = new List<TestEntity1>() { new TestEntity1(), new TestEntity1() };

            var builder = new DbContextMockBuilder<TestContext>();
            var mockedContext = builder.GetMockedDbContext();
            mockedContext.Set<TestEntity1>().AddRange(expectedResult);
            mockedContext.SaveChanges();

            var mockedDbSet = builder.GetMockedDbSetFor<TestEntity1>();

            CollectionAssert.AreEquivalent(expectedResult, mockedDbSet.ToList());
        }

        [Test]
        public void GetMockedDbSet_ByExpression_ReturnsDbSetMock() {
            var expectedResult = new List<TestEntity1>() { new TestEntity1(), new TestEntity1() };

            var builder = new DbContextMockBuilder<TestContext>();
            var mockedContext = builder.GetMockedDbContext();
            mockedContext.Set<TestEntity1>().AddRange(expectedResult);
            mockedContext.SaveChanges();

            var mockedDbSet = builder.GetMockedDbSetFor(x => x.TestEntities);

            CollectionAssert.AreEquivalent(expectedResult, mockedDbSet.ToList());
        }
    }
}
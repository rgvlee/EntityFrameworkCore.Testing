using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using EntityFrameworkCore.Testing.Moq.Extensions;
using Moq;

namespace EntityFrameworkCore.Testing.Moq.Tests {
    [TestFixture]
    public class MockedDbQueryTests {
        [Test]
        public void SetUpFromSql_AnyStoredProcedureWithNoParametersToList_ReturnsExpectedResult() {
            var expectedResult = new List<TestEntity2>() { new TestEntity2(), new TestEntity2() };

            var builder = new DbContextMockBuilder<TestContext>();
            builder.AddSetUpFor(x => x.TestView, expectedResult).AddFromSqlResultFor(x => x.TestView, expectedResult);
            var mockedContext = builder.GetMockedDbContext();
            
            var actualResult1 = mockedContext.Query<TestEntity2>().FromSql("sp_NoParams").ToList();
            var actualResult2 = mockedContext.Query<TestEntity2>().FromSql("sp_NoParams").ToList();

            Assert.Multiple(() => {
                CollectionAssert.AreEquivalent(expectedResult, actualResult1);
                CollectionAssert.AreEquivalent(actualResult1, actualResult2);
            });
        }

        [Test]
        public async Task SetUpFromSql_AnyStoredProcedureWithNoParametersAsyncToList_ReturnsExpectedResult() {
            var expectedResult = new List<TestEntity2>() { new TestEntity2(), new TestEntity2() };

            var builder = new DbContextMockBuilder<TestContext>();
            builder.AddSetUpFor(x => x.TestView, expectedResult).AddFromSqlResultFor(x => x.TestView, expectedResult);
            var mockedContext = builder.GetMockedDbContext();

            var actualResult1 = await mockedContext.Query<TestEntity2>().FromSql("sp_NoParams").ToListAsync();
            var actualResult2 = await mockedContext.Query<TestEntity2>().FromSql("sp_NoParams").ToListAsync();

            Assert.Multiple(() => {
                CollectionAssert.AreEquivalent(expectedResult, actualResult1);
                CollectionAssert.AreEquivalent(actualResult1, actualResult2);
            });
        }

        [Test]
        public void SetUpFromSql_SpecifiedStoredProcedureWithParametersToList_ReturnsExpectedResult() {
            var expectedResult = new List<TestEntity2> { new TestEntity2() };
            var sqlParameters = new List<SqlParameter> { new SqlParameter("@SomeParameter2", "Value2")};

            var builder = new DbContextMockBuilder<TestContext>();
            builder.AddSetUpFor(x => x.TestView, expectedResult).AddFromSqlResultFor(x => x.TestView, "sp_Specified", sqlParameters, expectedResult);
            var mockedContext = builder.GetMockedDbContext();

            var actualResult1 = mockedContext.Query<TestEntity2>().FromSql("[dbo].[sp_Specified] @SomeParameter1 @SomeParameter2", new SqlParameter("@someparameter2", "Value2")).ToList();
            var actualResult2 = mockedContext.Query<TestEntity2>().FromSql("[dbo].[sp_Specified] @SomeParameter1 @SomeParameter2", new SqlParameter("@someparameter2", "Value2")).ToList();

            Assert.Multiple(() => {
                CollectionAssert.AreEquivalent(expectedResult, actualResult1);
                CollectionAssert.AreEquivalent(actualResult1, actualResult2);
            });
        }

        [Test]
        public void SetUpQuery_ToList_ReturnsEnumeration() {
            var expectedResult = new List<TestEntity2>() { new TestEntity2(), new TestEntity2() };

            var builder = new DbContextMockBuilder<TestContext>();
            builder.AddSetUpFor(x => x.TestView, expectedResult);
            var mockedContext = builder.GetMockedDbContext();

            var actualResult1 = mockedContext.Query<TestEntity2>().ToList();
            var actualResult2 = mockedContext.Query<TestEntity2>().ToList();

            Assert.Multiple(() => {
                CollectionAssert.AreEquivalent(expectedResult, actualResult1);
                CollectionAssert.AreEquivalent(actualResult1, actualResult2);

                CollectionAssert.AreEquivalent(expectedResult, mockedContext.TestView.ToList());
            });
        }

        [Test]
        public async Task SetUpQuery_ToListAsync_ReturnsEnumeration() {
            var expectedResult = new List<TestEntity2>() { new TestEntity2(), new TestEntity2() };

            var builder = new DbContextMockBuilder<TestContext>();
            builder.AddSetUpFor(x => x.TestView, expectedResult);
            var mockedContext = builder.GetMockedDbContext();

            var actualResult1 = await mockedContext.Query<TestEntity2>().ToListAsync();
            var actualResult2 = await mockedContext.Query<TestEntity2>().ToListAsync();

            Assert.Multiple(() => {
                CollectionAssert.AreEquivalent(expectedResult, actualResult1);
                CollectionAssert.AreEquivalent(actualResult1, actualResult2);

                CollectionAssert.AreEquivalent(expectedResult, mockedContext.TestView.ToList());
            });
        }

        [Test]
        public void SetUpQuery_Any_ReturnsTrue() {
            var testEntities = new List<TestEntity2>() { new TestEntity2() { Id = new Guid() }, new TestEntity2() { Id = new Guid() } };

            var builder = new DbContextMockBuilder<TestContext>();
            builder.AddSetUpFor(x => x.TestView, testEntities);
            var mockedContext = builder.GetMockedDbContext();

            var actualResult1 = mockedContext.Query<TestEntity2>().Any();
            var actualResult2 = mockedContext.Query<TestEntity2>().Any();
            
            Assert.Multiple(() => {
                Assert.IsTrue(actualResult1);
                Assert.IsTrue(actualResult2);
            });
        }

        [Test]
        public async Task SetUpQuery_AnyAsync_ReturnsTrue() {
            var testEntities = new List<TestEntity2>() { new TestEntity2() { Id = new Guid() }, new TestEntity2() { Id = new Guid() } };

            var builder = new DbContextMockBuilder<TestContext>();
            builder.AddSetUpFor(x => x.TestView, testEntities);
            var mockedContext = builder.GetMockedDbContext();

            var actualResult1 = await mockedContext.Query<TestEntity2>().AnyAsync();
            var actualResult2 = await mockedContext.Query<TestEntity2>().AnyAsync();
            
            Assert.Multiple(() => {
                Assert.IsTrue(actualResult1);
                Assert.IsTrue(actualResult2);
            });
        }

        [Test]
        public void SetUpQuery_First_ReturnsFirstEntity() {
            var testEntities = new List<TestEntity2>() { new TestEntity2() { Id = new Guid() }, new TestEntity2() { Id = new Guid() } };
            var expectedResult = testEntities.First();

            var builder = new DbContextMockBuilder<TestContext>();
            builder.AddSetUpFor(x => x.TestView, testEntities);
            var mockedContext = builder.GetMockedDbContext();

            var actualResult1 = mockedContext.Query<TestEntity2>().First();
            var actualResult2 = mockedContext.Query<TestEntity2>().First();

            Assert.Multiple(() => {
                Assert.AreSame(expectedResult, actualResult1);
                Assert.AreSame(actualResult1, actualResult2);
            });
        }

        [Test]
        public async Task SetUpQuery_FirstAsync_ReturnsFirstEntity() {
            var testEntities = new List<TestEntity2>() { new TestEntity2() { Id = new Guid() }, new TestEntity2() { Id = new Guid() } };
            var expectedResult = testEntities.First();

            var builder = new DbContextMockBuilder<TestContext>();
            builder.AddSetUpFor(x => x.TestView, testEntities);
            var mockedContext = builder.GetMockedDbContext();

            var actualResult1 = await mockedContext.Query<TestEntity2>().FirstAsync();
            var actualResult2 = await mockedContext.Query<TestEntity2>().FirstAsync();

            Assert.Multiple(() => {
                Assert.AreSame(expectedResult, actualResult1);
                Assert.AreSame(actualResult1, actualResult2);
            });
        }

        [Test]
        public void GetDbQueryMock_ByType_ReturnsDbQueryMock() {
            var expectedResult = new List<TestEntity2>() { new TestEntity2(), new TestEntity2() };

            var builder = new DbContextMockBuilder<TestContext>();
            builder.AddSetUpFor(x => x.TestView, expectedResult);

            var dbQueryMock = builder.GetDbQueryMockFor<TestEntity2>();

            CollectionAssert.AreEquivalent(expectedResult, dbQueryMock.Object.ToList());
        }

        [Test]
        public void GetDbQueryMock_ByExpression_ReturnsDbQueryMock() {
            var expectedResult = new List<TestEntity2>() { new TestEntity2(), new TestEntity2() };

            var builder = new DbContextMockBuilder<TestContext>();
            builder.AddSetUpFor(x => x.TestView, expectedResult);

            var dbQueryMock = builder.GetDbQueryMockFor(x => x.TestView);

            CollectionAssert.AreEquivalent(expectedResult, dbQueryMock.Object.ToList());
        }

        [Test]
        public void GetMockedDbQuery_ByType_ReturnsDbQueryMock() {
            var expectedResult = new List<TestEntity2>() { new TestEntity2(), new TestEntity2() };

            var builder = new DbContextMockBuilder<TestContext>();
            builder.AddSetUpFor(x => x.TestView, expectedResult);

            var mockedDbQuery = builder.GetMockedDbQueryFor<TestEntity2>();

            CollectionAssert.AreEquivalent(expectedResult, mockedDbQuery.ToList());
        }

        [Test]
        public void GetMockedDbQuery_ByExpression_ReturnsDbQueryMock() {
            var expectedResult = new List<TestEntity2>() { new TestEntity2(), new TestEntity2() };

            var builder = new DbContextMockBuilder<TestContext>();
            builder.AddSetUpFor(x => x.TestView, expectedResult);

            var mockedDbQuery = builder.GetMockedDbQueryFor(x => x.TestView);

            CollectionAssert.AreEquivalent(expectedResult, mockedDbQuery.ToList());
        }

        [Test]
        public void SetUpQueryUsingEnumerationByExpressionOnMockedDbContextPostBuilder_Enumeration_ReturnsExpectedResult()
        {
            var expectedResult = new List<TestEntity2>() { new TestEntity2(), new TestEntity2() };

            var builder = new DbContextMockBuilder<TestContext>();
            var dbContextMock = builder.GetDbContextMock();
            var mockedDbContext = builder.GetMockedDbContext();

            //Assuming that at this point our tests don't have access to the builder; but we have
            //retained the dbContextMock and mockedDbContext
            dbContextMock.SetUpDbQueryFor(x => x.TestView, expectedResult);
            
            CollectionAssert.AreEquivalent(expectedResult, mockedDbContext.TestView.ToList());
        }

        [Test]
        public void SetUpQueryUsingDbQueryMockByExpressionOnMockedDbContextPostBuilder_Enumeration_ReturnsExpectedResult() {
            var expectedResult = new List<TestEntity2>() { new TestEntity2(), new TestEntity2() };

            var builder = new DbContextMockBuilder<TestContext>();
            var dbContextMock = builder.GetDbContextMock();
            var mockedDbContext = builder.GetMockedDbContext();

            //Assuming that at this point our tests don't have access to the builder; but we have
            //retained the dbContextMock and mockedDbContext
            var dbQueryMock = mockedDbContext.CreateDbQueryMockFor(x => x.TestView, expectedResult);
            dbContextMock.SetUpDbQueryFor(x => x.TestView, dbQueryMock);

            CollectionAssert.AreEquivalent(expectedResult, mockedDbContext.TestView.ToList());
        }
        
        [Test]
        public void SetUpQueryUsingEnumerationByTypeOnMockPostBuilder_Enumeration_ReturnsExpectedResult() {
            var expectedResult = new List<TestEntity2>() { new TestEntity2(), new TestEntity2() };

            var builder = new DbContextMockBuilder<TestContext>();
            var dbContextMock = builder.GetDbContextMock();
            var mockedDbContext = builder.GetMockedDbContext();

            //Assuming that at this point our tests don't have access to the builder; but we have
            //retained the dbContextMock and mockedDbContext
            dbContextMock.SetUpDbQueryFor(expectedResult);

            CollectionAssert.AreEquivalent(expectedResult, mockedDbContext.TestView.ToList());
        }

        [Test]
        public void SetUpQueryUsingDbQueryMockByTypeOnMockedDbContextPostBuilder_Enumeration_ReturnsExpectedResult() {
            var expectedResult = new List<TestEntity2>() { new TestEntity2(), new TestEntity2() };

            var builder = new DbContextMockBuilder<TestContext>();
            var dbContextMock = builder.GetDbContextMock();
            var mockedDbContext = builder.GetMockedDbContext();

            //Assuming that at this point our tests don't have access to the builder; but we have
            //retained the dbContextMock and mockedDbContext
            var dbQueryMock = mockedDbContext.CreateDbQueryMockFor(expectedResult);
            dbContextMock.SetUpDbQueryFor(dbQueryMock);

            CollectionAssert.AreEquivalent(expectedResult, mockedDbContext.TestView.ToList());
        }

        [Test]
        public void SetUpQuery_First_ReturnsExpectedResult() {
            var dataSource = new List<TestEntity2>() { new TestEntity2() { Id = Guid.NewGuid() }, new TestEntity2() { Id = Guid.NewGuid() } };

            var expectedResult = dataSource.First();

            var builder = new DbContextMockBuilder<TestContext>();
            builder.AddSetUpFor(x => x.TestView, dataSource);
            var mockedContext = builder.GetMockedDbContext();

            var actualResult1 = mockedContext.Query<TestEntity2>().First(x => x.Id == expectedResult.Id);
            var actualResult2 = mockedContext.Query<TestEntity2>().First(x => x.Id == expectedResult.Id);

            Assert.Multiple(() => {
                Assert.AreSame(expectedResult, actualResult1);
                Assert.AreSame(actualResult1, actualResult2);

                Assert.AreSame(expectedResult, mockedContext.TestView.First(x => x.Id == expectedResult.Id));
            });
        }

        [Test]
        public void SetUpQuery_Where_ReturnsExpectedResult() {
            var expectedResult = new List<TestEntity2>() { new TestEntity2() { Id = Guid.NewGuid() }, new TestEntity2() { Id = Guid.NewGuid() } };

            var builder = new DbContextMockBuilder<TestContext>();
            builder.AddSetUpFor(x => x.TestView, expectedResult);
            var mockedContext = builder.GetMockedDbContext();

            var actualResult1 = mockedContext.Query<TestEntity2>().Where(x => x.Id != default(Guid));
            var actualResult2 = mockedContext.Query<TestEntity2>().Where(x => x.Id != default(Guid));

            Assert.Multiple(() => {
                CollectionAssert.AreEquivalent(expectedResult, actualResult1);
                CollectionAssert.AreEquivalent(actualResult1, actualResult2);

                CollectionAssert.AreEquivalent(expectedResult, mockedContext.TestView.Where(x => x.Id != default(Guid)));
            });
        }

        [Test]
        public void SetUpQueryUsingEnumerationByTypeOnMockPostBuilder_ReturnsExpectedResult() {
            var expectedResult = new List<TestEntity2>() { new TestEntity2(), new TestEntity2() };

            var builder = new DbContextMockBuilder<TestContext>();
            var dbContextMock = builder.GetDbContextMock();
            var mockedDbContext = builder.GetMockedDbContext();

            //Assuming that at this point our tests don't have access to the builder; but we have
            //retained the dbContextMock and mockedDbContext
            dbContextMock.SetUpDbQueryFor(expectedResult);

            CollectionAssert.AreEquivalent(expectedResult, mockedDbContext.TestView.ToList());
        }

        [Test]
        public void SetUpQueryUsingDbQueryMockByTypeOnMockedDbContextPostBuilder_Where_ReturnsExpectedResult() {
            var expectedResult = new List<TestEntity2>() { new TestEntity2() { Id = Guid.NewGuid() }, new TestEntity2() { Id = Guid.NewGuid() } };

            var builder = new DbContextMockBuilder<TestContext>();
            var dbContextMock = builder.GetDbContextMock();
            var mockedDbContext = builder.GetMockedDbContext();

            //Assuming that at this point our tests don't have access to the builder; but we have
            //retained the dbContextMock and mockedDbContext
            var dbQueryMock = mockedDbContext.CreateDbQueryMockFor(expectedResult);
            dbContextMock.SetUpDbQueryFor(dbQueryMock);

            var actualResult1 = mockedDbContext.Query<TestEntity2>().Where(x => x.Id != default(Guid));
            var actualResult2 = mockedDbContext.Query<TestEntity2>().Where(x => x.Id != default(Guid));

            Assert.Multiple(() => {
                CollectionAssert.AreEquivalent(expectedResult, actualResult1);
                CollectionAssert.AreEquivalent(actualResult1, actualResult2);

                CollectionAssert.AreEquivalent(expectedResult, mockedDbContext.TestView.Where(x => x.Id != default(Guid)));
            });
        }

        [Test]
        public void SetUpQueryUsingEnumerationByTypeOnMockPostBuilder_Where_ReturnsExpectedResult() {
            var expectedResult = new List<TestEntity2>() { new TestEntity2() { Id = Guid.NewGuid() }, new TestEntity2() { Id = Guid.NewGuid() } };

            var builder = new DbContextMockBuilder<TestContext>();
            var dbContextMock = builder.GetDbContextMock();
            var mockedDbContext = builder.GetMockedDbContext();

            //Assuming that at this point our tests don't have access to the builder; but we have
            //retained the dbContextMock and mockedDbContext
            dbContextMock.SetUpDbQueryFor(expectedResult);

            var actualResult1 = mockedDbContext.Query<TestEntity2>().Where(x => x.Id != default(Guid));
            var actualResult2 = mockedDbContext.Query<TestEntity2>().Where(x => x.Id != default(Guid));

            Assert.Multiple(() => {
                CollectionAssert.AreEquivalent(expectedResult, actualResult1);
                CollectionAssert.AreEquivalent(actualResult1, actualResult2);

                CollectionAssert.AreEquivalent(expectedResult, mockedDbContext.TestView.Where(x => x.Id != default(Guid)));
            });
        }
    }
}
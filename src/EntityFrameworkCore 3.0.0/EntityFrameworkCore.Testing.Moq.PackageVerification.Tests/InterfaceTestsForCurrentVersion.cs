using System;
using System.Linq;
using Castle.DynamicProxy;
using EntityFrameworkCore.Testing.Common.Tests;
using EntityFrameworkCore.Testing.Moq.Extensions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Moq.PackageVerification.Tests
{
    public class InterfaceTestsForCurrentVersion
    {
        private DbContextOptions<TestDbContext> DbContextOptions => new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        [SetUp]
        public virtual void SetUp()
        {
            //LoggerHelper.LoggerFactory.AddConsole(LogLevel.Debug);
        }

        [Test]
        public void HelpersCreateMockedDbContextFor_DbContext_CreatesMockedDbContext()
        {
            var dbContextToMock = new TestDbContext(DbContextOptions);

            var mocked = Helpers.Create.MockedDbContextFor(dbContextToMock);

            Assert.Multiple(() =>
            {
                Assert.That(mocked, Is.Not.Null);
                Assert.That(ProxyUtil.IsProxy(mocked), Is.True);
            });
        }

        [Test]
        public void HelpersCreateMockedDbContextFor_Type_CreatesMockedDbContext()
        {
            var mocked = Helpers.Create.MockedDbContextFor<TestDbContext>();

            Assert.Multiple(() =>
            {
                Assert.That(mocked, Is.Not.Null);
                Assert.That(ProxyUtil.IsProxy(mocked), Is.True);
            });
        }

        [Test]
        public void HelpersCreateMockedDbContextFor_Factory_CreatesMockedDbContext()
        {
            TestDbContext CreateDbContextToMock()
            {
                return new TestDbContext(DbContextOptions);
            }

            var mocked = Helpers.Create.MockedDbContextFor(CreateDbContextToMock);

            Assert.Multiple(() =>
            {
                Assert.That(mocked, Is.Not.Null);
                Assert.That(ProxyUtil.IsProxy(mocked), Is.True);
            });
        }

        [Test]
        public void HelpersCreateMockedDbSetFor_DbSet_CreatesMockedDbSet()
        {
            var dbContextToMock = new TestDbContext(DbContextOptions);

            var mocked = Helpers.Create.MockedDbSetFor(dbContextToMock.TestEntities);

            Assert.Multiple(() =>
            {
                Assert.That(mocked, Is.Not.Null);
                Assert.That(ProxyUtil.IsProxy(mocked), Is.True);
            });
        }

        [Test]
        public void HelpersCreateMockedDbQueryFor_DbQuery_CreatesMockedDbQuery()
        {
            var dbContextToMock = new TestDbContext(DbContextOptions);

            var mocked = Helpers.Create.MockedDbQueryFor(dbContextToMock.TestView);

            Assert.Multiple(() =>
            {
                Assert.That(mocked, Is.Not.Null);
                Assert.That(ProxyUtil.IsProxy(mocked), Is.True);
            });
        }

        [Test]
        public void HelpersCreateMockedQueryProviderFor_Sequence_CreatesMockedQueryProvider()
        {
            var dbContextToMock = new TestDbContext(DbContextOptions);

            var mocked = Helpers.Create.MockedQueryProviderFor(dbContextToMock.TestView.AsQueryable());

            Assert.Multiple(() =>
            {
                Assert.That(mocked, Is.Not.Null);
                Assert.That(ProxyUtil.IsProxy(mocked), Is.True);
            });
        }

        [Test]
        public void CreateMockedDbContext_DbContext_CreatesMockedDbContext()
        {
            var dbContextToMock = new TestDbContext(DbContextOptions);

            var mocked = dbContextToMock.CreateMockedDbContext();

            Assert.Multiple(() =>
            {
                Assert.That(mocked, Is.Not.Null);
                Assert.That(ProxyUtil.IsProxy(mocked), Is.True);
            });
        }

        [Test]
        public void DbContextCreateMock_DbContext_CreatesMockedDbContext()
        {
            var dbContextToMock = new TestDbContext(DbContextOptions);

            var mocked = dbContextToMock.CreateMock();

            Assert.Multiple(() =>
            {
                Assert.That(mocked, Is.Not.Null);
                Assert.That(ProxyUtil.IsProxy(mocked), Is.True);
            });
        }

        [Test]
        public void CreateMockedDbSet_DbSet_CreatesMockedDbSet()
        {
            var dbContextToMock = new TestDbContext(DbContextOptions);

            var mocked = dbContextToMock.TestEntities.CreateMockedDbSet();

            Assert.Multiple(() =>
            {
                Assert.That(mocked, Is.Not.Null);
                Assert.That(ProxyUtil.IsProxy(mocked), Is.True);
            });
        }

        [Test]
        public void DbSetCreateMock_DbSet_CreatesMockedDbSet()
        {
            var dbContextToMock = new TestDbContext(DbContextOptions);

            var mocked = dbContextToMock.TestEntities.CreateMock();

            Assert.Multiple(() =>
            {
                Assert.That(mocked, Is.Not.Null);
                Assert.That(ProxyUtil.IsProxy(mocked), Is.True);
            });
        }

        [Test]
        public void CreateMockedDbQuery_DbQuery_CreatesMockedDbQuery()
        {
            var dbContextToMock = new TestDbContext(DbContextOptions);

            var mocked = dbContextToMock.TestView.CreateMockedDbQuery();

            Assert.Multiple(() =>
            {
                Assert.That(mocked, Is.Not.Null);
                Assert.That(ProxyUtil.IsProxy(mocked), Is.True);
            });
        }

        [Test]
        public void DbQueryCreateMock_DbQuery_CreatesMockedDbQuery()
        {
            var dbContextToMock = new TestDbContext(DbContextOptions);

            var mocked = dbContextToMock.TestEntities.CreateMock();

            Assert.Multiple(() =>
            {
                Assert.That(mocked, Is.Not.Null);
                Assert.That(ProxyUtil.IsProxy(mocked), Is.True);
            });
        }

        [Test]
        public void CreateMockedQueryProvider_Sequence_CreatesMockedQueryProvider()
        {
            var dbContextToMock = new TestDbContext(DbContextOptions);

            var mocked = ((IQueryable<TestQuery>) dbContextToMock.TestView).Provider.CreateMockedQueryProvider(dbContextToMock.TestView.AsQueryable());

            Assert.Multiple(() =>
            {
                Assert.That(mocked, Is.Not.Null);
                Assert.That(ProxyUtil.IsProxy(mocked), Is.True);
            });
        }

        [Test]
        public void QueryProviderCreateMock_Sequence_CreatesMockedQueryProvider()
        {
            var dbContextToMock = new TestDbContext(DbContextOptions);

            var mocked = ((IQueryable<TestQuery>) dbContextToMock.TestView).Provider.CreateMock(dbContextToMock.TestView.AsQueryable());

            Assert.Multiple(() =>
            {
                Assert.That(mocked, Is.Not.Null);
                Assert.That(ProxyUtil.IsProxy(mocked), Is.True);
            });
        }
    }
}
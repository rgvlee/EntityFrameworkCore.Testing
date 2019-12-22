using System;
using System.Linq;
using Castle.DynamicProxy;
using EntityFrameworkCore.Testing.Common.Helpers;
using EntityFrameworkCore.Testing.Common.Tests;
using EntityFrameworkCore.Testing.NSubstitute.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.NSubstitute.PackageVerification.Tests
{
    public class InterfaceTestsForCurrentVersion
    {
        private DbContextOptions<TestDbContext> DbContextOptions => new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        [SetUp]
        public virtual void SetUp()
        {
            LoggerHelper.LoggerFactory.AddConsole(LogLevel.Debug);
        }

        [Test]
        public void HelpersCreateSubstituteDbContextFor_DbContext_CreatesMockedDbContext()
        {
            var dbContextToMock = new TestDbContext(DbContextOptions);

            var mocked = Helpers.Create.SubstituteDbContextFor(dbContextToMock);

            Assert.Multiple(() =>
            {
                Assert.That(mocked, Is.Not.Null);
                Assert.That(ProxyUtil.IsProxy(mocked), Is.True);
            });
        }

        [Test]
        public void HelpersCreateSubstituteFor_DbContext_CreatesMockedDbContext()
        {
            var dbContextToMock = new TestDbContext(DbContextOptions);

            var mocked = Helpers.Create.SubstituteFor(dbContextToMock);

            Assert.Multiple(() =>
            {
                Assert.That(mocked, Is.Not.Null);
                Assert.That(ProxyUtil.IsProxy(mocked), Is.True);
            });
        }

        [Test]
        public void HelpersCreateSubstituteDbContextFor_Type_CreatesMockedDbContext()
        {
            var mocked = Helpers.Create.SubstituteDbContextFor<TestDbContext>();

            Assert.Multiple(() =>
            {
                Assert.That(mocked, Is.Not.Null);
                Assert.That(ProxyUtil.IsProxy(mocked), Is.True);
            });
        }

        [Test]
        public void HelpersCreateSubstituteDbContextFor_Factory_CreatesMockedDbContext()
        {
            TestDbContext CreateDbContextToMock()
            {
                return new TestDbContext(DbContextOptions);
            }

            var mocked = Helpers.Create.SubstituteDbContextFor(CreateDbContextToMock);

            Assert.Multiple(() =>
            {
                Assert.That(mocked, Is.Not.Null);
                Assert.That(ProxyUtil.IsProxy(mocked), Is.True);
            });
        }

        [Test]
        public void HelpersCreateSubstituteDbSetFor_DbSet_CreatesMockedDbSet()
        {
            var dbContextToMock = new TestDbContext(DbContextOptions);

            var mocked = Helpers.Create.SubstituteDbSetFor(dbContextToMock.TestEntities);

            Assert.Multiple(() =>
            {
                Assert.That(mocked, Is.Not.Null);
                Assert.That(ProxyUtil.IsProxy(mocked), Is.True);
            });
        }

        [Test]
        public void HelpersCreateSubstituteFor_DbSet_CreatesMockedDbSet()
        {
            var dbContextToMock = new TestDbContext(DbContextOptions);

            var mocked = Helpers.Create.SubstituteFor(dbContextToMock.TestEntities);

            Assert.Multiple(() =>
            {
                Assert.That(mocked, Is.Not.Null);
                Assert.That(ProxyUtil.IsProxy(mocked), Is.True);
            });
        }

        [Test]
        public void HelpersCreateSubstituteDbQueryFor_DbQuery_CreatesMockedDbQuery()
        {
            var dbContextToMock = new TestDbContext(DbContextOptions);

            var mocked = Helpers.Create.SubstituteDbQueryFor(dbContextToMock.TestView);

            Assert.Multiple(() =>
            {
                Assert.That(mocked, Is.Not.Null);
                Assert.That(ProxyUtil.IsProxy(mocked), Is.True);
            });
        }

        [Test]
        public void HelpersCreateSubstituteFor_DbQuery_CreatesMockedDbQuery()
        {
            var dbContextToMock = new TestDbContext(DbContextOptions);

            var mocked = Helpers.Create.SubstituteFor(dbContextToMock.TestView);

            Assert.Multiple(() =>
            {
                Assert.That(mocked, Is.Not.Null);
                Assert.That(ProxyUtil.IsProxy(mocked), Is.True);
            });
        }

        [Test]
        public void HelpersCreateSubstituteQueryProviderFor_Sequence_CreatesMockedQueryProvider()
        {
            var dbContextToMock = new TestDbContext(DbContextOptions);

            var mocked = Helpers.Create.SubstituteQueryProviderFor(dbContextToMock.TestView.AsQueryable());

            Assert.Multiple(() =>
            {
                Assert.That(mocked, Is.Not.Null);
                Assert.That(ProxyUtil.IsProxy(mocked), Is.True);
            });
        }

        [Test]
        public void HelpersCreateSubstituteFor_Sequence_CreatesMockedQueryProvider()
        {
            var dbContextToMock = new TestDbContext(DbContextOptions);

            var mocked = Helpers.Create.SubstituteFor(dbContextToMock.TestView.AsQueryable());

            Assert.Multiple(() =>
            {
                Assert.That(mocked, Is.Not.Null);
                Assert.That(ProxyUtil.IsProxy(mocked), Is.True);
            });
        }

        [Test]
        public void CreateDbContextSubstitute_DbContext_CreatesMockedDbContext()
        {
            var dbContextToMock = new TestDbContext(DbContextOptions);

            var mocked = dbContextToMock.CreateDbContextSubstitute();

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
        public void CreateDbSetSubstitute_DbSet_CreatesMockedDbSet()
        {
            var dbContextToMock = new TestDbContext(DbContextOptions);

            var mocked = dbContextToMock.TestEntities.CreateDbSetSubstitute();

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
        public void CreateDbQuerySubstitute_DbQuery_CreatesMockedDbQuery()
        {
            var dbContextToMock = new TestDbContext(DbContextOptions);

            var mocked = dbContextToMock.TestView.CreateDbQuerySubstitute();

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
        public void CreateQueryProviderSubstitute_Sequence_CreatesMockedQueryProvider()
        {
            var dbContextToMock = new TestDbContext(DbContextOptions);

            var mocked = ((IQueryable<TestQuery>) dbContextToMock.TestView).Provider.CreateQueryProviderSubstitute(dbContextToMock.TestView.AsQueryable());

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
using System;
using System.Linq;
using Castle.DynamicProxy;
using EntityFrameworkCore.Testing.Common.Helpers;
using EntityFrameworkCore.Testing.Common.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Moq.Tests
{
    public partial class CreateFactoryTests
    {
        //private DbContextOptions<TestDbContext> DbContextOptions => new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        [SetUp]
        public virtual void SetUp()
        {
            LoggerHelper.LoggerFactory.AddConsole(LogLevel.Debug);
        }

        //[Test]
        //public void HelpersCreateMockedDbContextFor_DbContext_CreatesMockedDbContext()
        //{
        //    var dbContextToMock = new TestDbContext(DbContextOptions);

        //    var mocked = Helpers.Create.MockedDbContextFor(dbContextToMock);

        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(mocked, Is.Not.Null);
        //        Assert.That(ProxyUtil.IsProxy(mocked), Is.True);
        //    });
        //}

        [Test]
        public void HelpersCreateMockedDbContextFor_NullDbContext_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var mocked = Helpers.Create.MockedDbContextFor((TestDbContext) null);
            });
        }

        [Test]
        public void CreateMockedDbContextFor_DbContext_CreatesMockedDbContext()
        {
            var dbContextToMock = new TestDbContext(DbContextOptions);

            var mocked = Create.MockedDbContextFor(dbContextToMock);

            Assert.Multiple(() =>
            {
                Assert.That(mocked, Is.Not.Null);
                Assert.That(ProxyUtil.IsProxy(mocked), Is.True);
            });
        }

        [Test]
        public void CreateMockedDbContextFor_NullDbContext_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var mocked = Create.MockedDbContextFor((TestDbContext) null);
            });
        }

        //[Test]
        //public void HelpersCreateMockedDbContextFor_Type_CreatesMockedDbContext()
        //{
        //    var mocked = Helpers.Create.MockedDbContextFor<TestDbContext>();

        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(mocked, Is.Not.Null);
        //        Assert.That(ProxyUtil.IsProxy(mocked), Is.True);
        //    });
        //}

        [Test]
        public void HelpersCreateMockedDbContextFor_TypeWithNoConstructors_ThrowsException()
        {
            Assert.Throws<MissingMethodException>(() =>
            {
                var mocked = Helpers.Create.MockedDbContextFor<TestDbContextWithNoConstructors>();
            });
        }

        [Test]
        public void CreateMockedDbContextFor_Type_CreatesMockedDbContext()
        {
            var mocked = Create.MockedDbContextFor<TestDbContext>();

            Assert.Multiple(() =>
            {
                Assert.That(mocked, Is.Not.Null);
                Assert.That(ProxyUtil.IsProxy(mocked), Is.True);
            });
        }

        [Test]
        public void CreateMockedDbContextFor_TypeWithNoConstructors_ThrowsException()
        {
            Assert.Throws<MissingMethodException>(() =>
            {
                var mocked = Create.MockedDbContextFor<TestDbContextWithNoConstructors>();
            });
        }

        //[Test]
        //public void HelpersCreateMockedDbContextFor_Factory_CreatesMockedDbContext()
        //{
        //    TestDbContext Factory()
        //    {
        //        return new TestDbContext(DbContextOptions);
        //    }

        //    var mocked = Helpers.Create.MockedDbContextFor(Factory);

        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(mocked, Is.Not.Null);
        //        Assert.That(ProxyUtil.IsProxy(mocked), Is.True);
        //    });
        //}

        [Test]
        public void HelpersCreateMockedDbContextFor_NullFactory_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var mocked = Helpers.Create.MockedDbContextFor((Func<TestDbContext>) null);
            });
        }

        [Test]
        public void CreateMockedDbContextUsingResultFrom_Factory_CreatesMockedDbContext()
        {
            TestDbContext Factory()
            {
                return new TestDbContext(DbContextOptions);
            }

            var mocked = Create.MockedDbContextUsingResultFrom(Factory);

            Assert.Multiple(() =>
            {
                Assert.That(mocked, Is.Not.Null);
                Assert.That(ProxyUtil.IsProxy(mocked), Is.True);
            });
        }

        [Test]
        public void CreateMockedDbContextUsingResultFrom_NullFactory_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var mocked = Create.MockedDbContextUsingResultFrom((Func<TestDbContext>) null);
            });
        }

        //[Test]
        //public void HelpersCreateMockedDbSetFor_DbSet_CreatesMockedDbSet()
        //{
        //    var dbContextToMock = new TestDbContext(DbContextOptions);

        //    var mocked = Helpers.Create.MockedDbSetFor(dbContextToMock.TestEntities);

        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(mocked, Is.Not.Null);
        //        Assert.That(ProxyUtil.IsProxy(mocked), Is.True);
        //    });
        //}

        [Test]
        public void HelpersCreateMockedDbSetFor_NullDbSet_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var mocked = Helpers.Create.MockedDbSetFor((DbSet<TestEntity>) null);
            });
        }

        [Test]
        public void CreateMockedDbSetFor_DbSet_CreatesMockedDbSet()
        {
            var dbContextToMock = new TestDbContext(DbContextOptions);

            var mocked = Create.MockedDbSetFor(dbContextToMock.TestEntities);

            Assert.Multiple(() =>
            {
                Assert.That(mocked, Is.Not.Null);
                Assert.That(ProxyUtil.IsProxy(mocked), Is.True);
            });
        }

        [Test]
        public void CreateMockedDbSetFor_NullDbSet_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var mocked = Create.MockedDbSetFor((DbSet<TestEntity>) null);
            });
        }

        //[Test]
        //public void HelpersCreateMockedDbQueryFor_DbQuery_CreatesMockedDbQuery()
        //{
        //    var dbContextToMock = new TestDbContext(DbContextOptions);

        //    var mocked = Helpers.Create.MockedDbQueryFor(dbContextToMock.TestView);

        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(mocked, Is.Not.Null);
        //        Assert.That(ProxyUtil.IsProxy(mocked), Is.True);
        //    });
        //}

        [Test]
        public void HelpersCreateMockedDbQueryFor_NullDbQuery_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var mocked = Helpers.Create.MockedDbQueryFor((DbQuery<TestEntity>) null);
            });
        }

        [Test]
        public void CreateMockedDbQueryFor_DbQuery_CreatesMockedDbQuery()
        {
            var dbContextToMock = new TestDbContext(DbContextOptions);

            var mocked = Create.MockedDbQueryFor(dbContextToMock.TestView);

            Assert.Multiple(() =>
            {
                Assert.That(mocked, Is.Not.Null);
                Assert.That(ProxyUtil.IsProxy(mocked), Is.True);
            });
        }

        [Test]
        public void CreateMockedDbQueryFor_NullDbQuery_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var mocked = Create.MockedDbQueryFor((DbQuery<TestEntity>) null);
            });
        }

        //[Test]
        //public void HelpersCreateMockedQueryProviderFor_Queryable_CreatesMockedQueryProvider()
        //{
        //    var dbContextToMock = new TestDbContext(DbContextOptions);

        //    var mocked = Helpers.Create.MockedQueryProviderFor(dbContextToMock.TestView.AsQueryable());

        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(mocked, Is.Not.Null);
        //        Assert.That(ProxyUtil.IsProxy(mocked), Is.True);
        //    });
        //}

        [Test]
        public void HelpersCreateMockedQueryProviderFor_NullQueryable_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var mocked = Helpers.Create.MockedQueryProviderFor((IQueryable<TestEntity>) null);
            });
        }

        [Test]
        public void CreateMockedQueryProviderFor_Queryable_CreatesMockedQueryProvider()
        {
            var dbContextToMock = new TestDbContext(DbContextOptions);

            var mocked = Create.MockedQueryProviderFor(dbContextToMock.TestView.AsQueryable());

            Assert.Multiple(() =>
            {
                Assert.That(mocked, Is.Not.Null);
                Assert.That(ProxyUtil.IsProxy(mocked), Is.True);
            });
        }

        [Test]
        public void CreateMockedQueryProviderFor_NullQueryable_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var mocked = Create.MockedQueryProviderFor((IQueryable<TestEntity>) null);
            });
        }
    }
}
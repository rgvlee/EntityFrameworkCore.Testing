﻿using System;
using System.Linq;
using Castle.DynamicProxy;
using EntityFrameworkCore.Testing.Common.Helpers;
using EntityFrameworkCore.Testing.Common.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Moq.Tests
{
    public class CreateFactoryTests
    {
        [SetUp]
        public virtual void SetUp()
        {
            LoggerHelper.LoggerFactory.AddConsole(LogLevel.Debug);
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
        public void CreateMockedQueryProviderFor_Queryable_CreatesMockedQueryProvider()
        {
            var dbContextToMock = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

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
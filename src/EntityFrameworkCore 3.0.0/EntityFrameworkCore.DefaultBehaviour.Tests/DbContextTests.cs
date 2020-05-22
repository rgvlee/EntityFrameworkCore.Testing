﻿using System;
using System.Linq;
using EntityFrameworkCore.Testing.Common;
using EntityFrameworkCore.Testing.Common.Tests;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.DefaultBehaviour.Tests
{
    [TestFixture]
    public class DbContextTests : BaseForTests
    {
        [SetUp]
        public override void SetUp()
        {
            DbContext = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            base.SetUp();
        }

        protected TestDbContext DbContext;

        [Test]
        public virtual void ExecuteSqlCommand_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var actualResult = DbContext.Database.ExecuteSqlCommand("sp_NoParams");
            });
        }

        [Test]
        public virtual void ExecuteSqlInterpolated_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var actualResult = DbContext.Database.ExecuteSqlInterpolated($"sp_NoParams");
            });
        }

        [Test]
        public virtual void ExecuteSqlRaw_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var actualResult = DbContext.Database.ExecuteSqlRaw("sp_NoParams");
            });
        }

        [Test]
        public virtual void Query_TypeNotIncludedInModel_ThrowsException()
        {
            Assert.Multiple(() =>
            {
                var ex = Assert.Throws<InvalidOperationException>(() => DbContext.Query<NotRegisteredEntity>().ToList());
                Assert.That(ex.Message, Is.EqualTo(string.Format(ExceptionMessages.CannotCreateDbSetTypeNotIncludedInModel, nameof(NotRegisteredEntity))));
            });
        }

        [Test]
        public virtual void Set_TypeNotIncludedInModel_ThrowsException()
        {
            Assert.Multiple(() =>
            {
                var ex = Assert.Throws<InvalidOperationException>(() => DbContext.Set<NotRegisteredEntity>().ToList());
                Assert.That(ex.Message, Is.EqualTo(string.Format(ExceptionMessages.CannotCreateDbSetTypeNotIncludedInModel, nameof(NotRegisteredEntity))));
            });
        }
    }
}
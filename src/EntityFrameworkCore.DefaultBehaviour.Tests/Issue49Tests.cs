using System;
using EntityFrameworkCore.Testing.Common.Tests;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.DefaultBehaviour.Tests
{
    public class Issue49Tests : Issue49Tests<TestDbContext, TestEntity>
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            DbContext = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        }
    }
}
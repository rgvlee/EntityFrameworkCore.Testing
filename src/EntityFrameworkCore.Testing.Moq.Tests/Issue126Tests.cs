using System;
using EntityFrameworkCore.Testing.Common.Tests;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Moq.Tests
{
    public class Issue126Tests : Issue126Tests<TestDbContext>
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            DbContextFactory = () => Create.MockedDbContextFor<TestDbContext>(options);
        }
    }
}
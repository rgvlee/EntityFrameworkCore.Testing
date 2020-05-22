using System;
using EntityFrameworkCore.Testing.Common.Tests;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Moq.Tests
{
    [TestFixture]
    public class DbContextTestsUsingConstructorParameters : BaseForDbContextTests<TestDbContext>
    {
        [SetUp]
        public override void SetUp()
        {
            MockedDbContext = Create.MockedDbContextFor<TestDbContext>(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            base.SetUp();
        }
    }
}
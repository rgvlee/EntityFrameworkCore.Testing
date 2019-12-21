using System;
using EntityFrameworkCore.Testing.Common.Tests;
using EntityFrameworkCore.Testing.NSubstitute.Helpers;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.NSubstitute.Tests
{
    [TestFixture]
    public class ByPropertyDbQueryExceptionTests : ReadOnlyDbSetExceptionTests<TestQuery>
    {
        [SetUp]
        public override void SetUp()
        {
            var dbContextToMock = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            MockedDbContext = Create.SubstituteDbContextFor(dbContextToMock);
            base.SetUp();
        }

        protected TestDbContext MockedDbContext;

        protected override DbSet<TestQuery> DbSet => MockedDbContext.TestView;
    }
}
using EntityFrameworkCore.Testing.Common.Tests;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.NSubstitute.Tests
{
    [TestFixture]
    public class ByPropertyReadOnlyDbSetExceptionTests : ReadOnlyDbSetExceptionTests<TestReadOnlyEntity>
    {
        [SetUp]
        public override void SetUp()
        {
            MockedDbContext = Create.MockedDbContextFor<TestDbContext>();
            base.SetUp();
        }

        protected TestDbContext MockedDbContext;

        protected override DbSet<TestReadOnlyEntity> DbSet => MockedDbContext.TestReadOnlyEntities;
    }
}
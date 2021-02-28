using EntityFrameworkCore.Testing.Common.Tests;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.NSubstitute.Tests
{
    public class ByPropertyReadOnlyDbSetExceptionTests : ReadOnlyDbSetExceptionTests<TestReadOnlyEntity>
    {
        protected TestDbContext MockedDbContext;

        protected override DbSet<TestReadOnlyEntity> DbSet => MockedDbContext.TestReadOnlyEntities;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            MockedDbContext = Create.MockedDbContextFor<TestDbContext>();
        }
    }
}
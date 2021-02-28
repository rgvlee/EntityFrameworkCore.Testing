using EntityFrameworkCore.Testing.Common.Tests;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.NSubstitute.Tests
{
    public class ByTypeReadOnlyDbSetExceptionTests : ReadOnlyDbSetExceptionTests<TestReadOnlyEntity>
    {
        protected TestDbContext MockedDbContext;

        protected override DbSet<TestReadOnlyEntity> DbSet => MockedDbContext.Set<TestReadOnlyEntity>();

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            MockedDbContext = Create.MockedDbContextFor<TestDbContext>();
        }
    }
}
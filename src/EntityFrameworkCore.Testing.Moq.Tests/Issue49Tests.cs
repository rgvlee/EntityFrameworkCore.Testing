using EntityFrameworkCore.Testing.Common.Tests;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Moq.Tests
{
    public class Issue49Tests : Issue49Tests<TestDbContext, TestEntity>
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            DbContext = Create.MockedDbContextFor<TestDbContext>();
        }
    }
}
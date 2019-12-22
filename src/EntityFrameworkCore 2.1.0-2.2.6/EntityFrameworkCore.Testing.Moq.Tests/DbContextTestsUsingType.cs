using EntityFrameworkCore.Testing.Common.Tests;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Moq.Tests
{
    [TestFixture]
    public class DbContextTestsUsingType : DbContextTestsBase<TestDbContext>
    {
        [SetUp]
        public override void SetUp()
        {
            MockedDbContext = Create.MockedDbContextFor<TestDbContext>();
            base.SetUp();
        }
    }
}
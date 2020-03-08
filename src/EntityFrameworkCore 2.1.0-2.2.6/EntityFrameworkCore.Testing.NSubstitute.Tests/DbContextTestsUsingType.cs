using EntityFrameworkCore.Testing.Common.Tests;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.NSubstitute.Tests
{
    [TestFixture]
    public class DbContextTestsUsingType : BaseForDbContextTests<TestDbContext>
    {
        [SetUp]
        public override void SetUp()
        {
            MockedDbContext = Create.MockedDbContextFor<TestDbContext>();
            base.SetUp();
        }
    }
}
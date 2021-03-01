using EntityFrameworkCore.Testing.Common.Tests;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.NSubstitute.Tests
{
    public class DbContextTestsUsingType : BaseForDbContextTests<TestDbContext>
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            MockedDbContext = Create.MockedDbContextFor<TestDbContext>();
        }
    }
}
using EntityFrameworkCore.Testing.Common.Tests;
using EntityFrameworkCore.Testing.NSubstitute.Helpers;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.NSubstitute.Tests
{
    [TestFixture]
    public class DbContextTestsUsingType : DbContextTestsBase<TestDbContext>
    {
        [SetUp]
        public override void SetUp()
        {
            MockedDbContext = Create.SubstituteDbContextFor<TestDbContext>();
            base.SetUp();
        }
    }
}
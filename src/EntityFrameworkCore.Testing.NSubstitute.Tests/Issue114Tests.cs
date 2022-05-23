using NUnit.Framework;

namespace EntityFrameworkCore.Testing.NSubstitute.Tests
{
    public class Issue114Tests : Common.Tests.Issue114Tests
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
        }

        protected override TestDbContext MockedDbContextFactory()
        {
            return Create.MockedDbContextFor<TestDbContext>();
        }
    }
}
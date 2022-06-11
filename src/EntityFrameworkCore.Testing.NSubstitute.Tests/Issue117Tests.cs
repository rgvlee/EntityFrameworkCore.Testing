namespace EntityFrameworkCore.Testing.NSubstitute.Tests
{
    public class Issue117Tests : Common.Tests.Issue117Tests
    {
        protected override TestDbContext MockedDbContextFactory()
        {
            return Create.MockedDbContextFor<TestDbContext>();
        }
    }
}
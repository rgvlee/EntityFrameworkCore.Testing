namespace EntityFrameworkCore.Testing.NSubstitute.Tests
{
    public class Issue114Tests : Common.Tests.Issue114Tests
    {
        protected override TestDbContext MockedDbContextFactory()
        {
            return Create.MockedDbContextFor<TestDbContext>();
        }
    }
}
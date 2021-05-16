using EntityFrameworkCore.Testing.Common.Tests;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.NSubstitute.Tests
{
    public class Issue91Tests : Common.Tests.Issue91Tests
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            DbContextFactory = () => Create.MockedDbContextFor<Issue91DbContext>(DbContextOptions);
        }
    }
}
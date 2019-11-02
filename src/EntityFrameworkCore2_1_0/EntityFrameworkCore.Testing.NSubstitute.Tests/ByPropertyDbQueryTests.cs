using System.Linq;
using EntityFrameworkCore.Testing.Common.Tests;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.NSubstitute.Tests
{
    [TestFixture]
    public class ByPropertyDbQueryTests : DbQueryTestsBase
    {
        protected override IQueryable<TestQuery> Queryable => MockedDbContext.TestView;
    }
}
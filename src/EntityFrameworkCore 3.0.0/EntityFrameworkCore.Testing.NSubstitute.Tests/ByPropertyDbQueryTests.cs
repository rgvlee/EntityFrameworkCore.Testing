using System.Linq;
using EntityFrameworkCore.Testing.Common.Tests;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.NSubstitute.Tests
{
    [TestFixture]
    public class ByPropertyDbQueryTests : DbQueryTestsBase<TestQuery>
    {
        protected override IQueryable<TestQuery> Queryable => MockedDbContext.TestView;
    }
}
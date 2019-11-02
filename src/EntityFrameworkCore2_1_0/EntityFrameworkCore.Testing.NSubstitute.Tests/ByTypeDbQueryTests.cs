using System.Linq;
using EntityFrameworkCore.Testing.Common.Tests;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.NSubstitute.Tests
{
    [TestFixture]
    public class ByTypeDbQueryTests : DbQueryTestsBase
    {
        protected override IQueryable<TestQuery> Queryable => MockedDbContext.Query<TestQuery>();
    }
}
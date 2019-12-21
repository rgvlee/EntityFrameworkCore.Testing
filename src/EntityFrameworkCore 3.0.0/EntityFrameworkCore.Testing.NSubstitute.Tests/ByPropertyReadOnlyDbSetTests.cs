using System.Linq;
using EntityFrameworkCore.Testing.Common.Tests;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.NSubstitute.Tests
{
    [TestFixture]
    public class ByPropertyReadOnlyDbSetTests : DbQueryTestsBase<TestReadOnlyEntity>
    {
        protected override IQueryable<TestReadOnlyEntity> Queryable => MockedDbContext.Set<TestReadOnlyEntity>();
    }
}
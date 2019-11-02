using System.Linq;
using EntityFrameworkCore.Testing.Common.Tests;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Moq.Tests
{
    [TestFixture]
    public class ByTypeReadOnlyDbSetTests : DbQueryTestsBase<TestReadOnlyEntity>
    {
        protected override IQueryable<TestReadOnlyEntity> Queryable => MockedDbContext.Set<TestReadOnlyEntity>();
    }
}
using System.Linq;
using EntityFrameworkCore.Testing.Common.Tests;

namespace EntityFrameworkCore.Testing.NSubstitute.Tests
{
    public class ByPropertyReadOnlyDbSetTests : BaseForDbQueryTests<TestReadOnlyEntity>
    {
        protected override IQueryable<TestReadOnlyEntity> Queryable => MockedDbContext.Set<TestReadOnlyEntity>();
    }
}
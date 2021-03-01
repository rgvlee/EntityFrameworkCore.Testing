using System.Linq;
using EntityFrameworkCore.Testing.Common.Tests;

namespace EntityFrameworkCore.Testing.Moq.Tests
{
    public class ByPropertyReadOnlyDbSetTests : BaseForDbQueryTests<TestReadOnlyEntity>
    {
        protected override IQueryable<TestReadOnlyEntity> Queryable => MockedDbContext.Set<TestReadOnlyEntity>();
    }
}
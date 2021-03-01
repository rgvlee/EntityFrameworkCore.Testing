using System.Linq;
using EntityFrameworkCore.Testing.Common.Tests;

namespace EntityFrameworkCore.Testing.Moq.Tests
{
    public class ByPropertyDbSetTests : BaseForDbSetTests<TestEntity>
    {
        protected override IQueryable<TestEntity> Queryable => MockedDbContext.TestEntities;
    }
}
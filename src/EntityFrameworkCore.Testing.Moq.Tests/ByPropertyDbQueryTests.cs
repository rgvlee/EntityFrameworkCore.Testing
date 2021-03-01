using System.Linq;
using EntityFrameworkCore.Testing.Common.Tests;

namespace EntityFrameworkCore.Testing.Moq.Tests
{
    public class ByPropertyDbQueryTests : BaseForDbQueryTests<ViewEntity>
    {
        protected override IQueryable<ViewEntity> Queryable => MockedDbContext.ViewEntities;
    }
}
using System.Linq;
using EntityFrameworkCore.Testing.Common.Tests;

namespace EntityFrameworkCore.Testing.NSubstitute.Tests
{
    public class ByPropertyDbQueryTests : BaseForDbQueryTests<ViewEntity>
    {
        protected override IQueryable<ViewEntity> Queryable => MockedDbContext.ViewEntities;
    }
}
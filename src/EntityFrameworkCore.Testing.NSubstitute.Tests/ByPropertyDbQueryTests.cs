using System.Linq;
using EntityFrameworkCore.Testing.Common.Tests;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.NSubstitute.Tests
{
    public class ByPropertyDbQueryTests : BaseForDbQueryTests<ViewEntity>
    {
        protected override IQueryable<ViewEntity> Queryable => MockedDbContext.ViewEntities;
    }
}
using System.Linq;
using EntityFrameworkCore.Testing.Common.Tests;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.NSubstitute.Tests
{
    [TestFixture]
    public class ByTypeDbQueryTests : BaseForDbQueryTests<ViewEntity>
    {
        protected override IQueryable<ViewEntity> Queryable => MockedDbContext.Query<ViewEntity>();
    }
}
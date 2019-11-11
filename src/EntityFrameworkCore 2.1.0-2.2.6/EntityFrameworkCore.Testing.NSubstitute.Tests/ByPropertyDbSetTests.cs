using System.Linq;
using EntityFrameworkCore.Testing.Common.Tests;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.NSubstitute.Tests
{
    [TestFixture]
    public class ByPropertyDbSetTests : DbSetTestsBase<TestEntity>
    {
        protected override IQueryable<TestEntity> Queryable => MockedDbContext.TestEntities;
    }
}
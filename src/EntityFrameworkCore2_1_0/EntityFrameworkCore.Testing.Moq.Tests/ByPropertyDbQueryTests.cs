using System.Linq;
using EntityFrameworkCore.Testing.Common.Tests;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Moq.Tests
{
    [TestFixture]
    public class ByPropertyDbQueryTests : DbQueryTestsBase
    {
        protected override IQueryable<TestQuery1> Queryable => MockedDbContext.TestView;
    }
}
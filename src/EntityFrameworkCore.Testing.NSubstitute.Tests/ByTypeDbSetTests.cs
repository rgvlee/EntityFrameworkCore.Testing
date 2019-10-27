using System.Linq;
using System.Linq.Expressions;
using EntityFrameworkCore.Testing.Common.Tests;
using NSubstitute;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.NSubstitute.Tests
{
    [TestFixture]
    public class ByTypeDbSetTests : DbSetTestsBase
    {
        protected override IQueryable<TestEntity1> Queryable => MockedDbContext.Set<TestEntity1>();

        [Test(Description = "This test ensures that method invoked via CallBase = true are verifiable")]
        public override void Select_ReturnsSequence()
        {
            base.Select_ReturnsSequence();

            Queryable.Provider
                .Received(2)
                .CreateQuery<TestEntity1>(Arg.Any<Expression>());

            Queryable.Provider
                .Received(2)
                .CreateQuery<TestEntity1>(Arg.Is<MethodCallExpression>(mce => mce.Method.Name.Equals(nameof(System.Linq.Queryable.Select))));
        }
    }
}
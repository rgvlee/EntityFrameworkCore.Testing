using System.Linq;
using System.Linq.Expressions;
using EntityFrameworkCore.Testing.Common.Tests;
using NSubstitute;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.NSubstitute.Tests
{
    [TestFixture]
    public class ByTypeDbSetTests : BaseForDbSetTests<TestEntity>
    {
        protected override IQueryable<TestEntity> Queryable => MockedDbContext.Set<TestEntity>();

        [Test(Description = "This test ensures that method invoked via CallBase = true are verifiable")]
        public override void Select_ReturnsSequence()
        {
            base.Select_ReturnsSequence();

            Queryable.Provider.Received(2).CreateQuery<TestEntity>(Arg.Any<Expression>());

            Queryable.Provider.Received(2).CreateQuery<TestEntity>(Arg.Is<MethodCallExpression>(mce => mce.Method.Name.Equals(nameof(System.Linq.Queryable.Select))));
        }
    }
}
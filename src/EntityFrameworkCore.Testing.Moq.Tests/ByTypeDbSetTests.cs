using System.Linq;
using System.Linq.Expressions;
using EntityFrameworkCore.Testing.Common.Tests;
using Moq;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Moq.Tests
{
    [TestFixture]
    public class ByTypeDbSetTests : DbSetTestsBase
    {
        protected override IQueryable<TestEntity1> Queryable => MockedDbContext.Set<TestEntity1>();

        [Test(Description = "This test ensures that method invoked via CallBase = true are verifiable")]
        public override void Select_ReturnsSequence()
        {
            base.Select_ReturnsSequence();
            var queryProviderMock = Mock.Get(Queryable.Provider);
            queryProviderMock.Verify(
                m => m.CreateQuery<TestEntity1>(It.IsAny<Expression>()),
                Times.Exactly(2)
            );

            queryProviderMock.Verify(
                m => m.CreateQuery<TestEntity1>(It.Is<MethodCallExpression>(mce => mce.Method.Name.Equals(nameof(System.Linq.Queryable.Select)))),
                Times.Exactly(2)
            );
        }
    }
}
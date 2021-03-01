using System.Linq;
using System.Linq.Expressions;
using EntityFrameworkCore.Testing.Common.Tests;
using Moq;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Moq.Tests
{
    public class ByTypeDbSetTests : BaseForDbSetTests<TestEntity>
    {
        protected override IQueryable<TestEntity> Queryable => MockedDbContext.Set<TestEntity>();

        [Test(Description = "This test ensures that method invoked via CallBase = true are verifiable")]
        public override void Select_ReturnsSequence()
        {
            base.Select_ReturnsSequence();

            var queryProviderMock = Mock.Get(Queryable.Provider);

            queryProviderMock.Verify(m => m.CreateQuery<TestEntity>(It.IsAny<Expression>()), Times.Exactly(2));

            queryProviderMock.Verify(m => m.CreateQuery<TestEntity>(It.Is<MethodCallExpression>(mce => mce.Method.Name.Equals(nameof(System.Linq.Queryable.Select)))),
                Times.Exactly(2));
        }
    }
}
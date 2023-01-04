using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;

namespace EntityFrameworkCore.Testing.Common
{
    public class FakeQueryRootExpression : EntityQueryRootExpression
    {
        public FakeQueryRootExpression(IAsyncQueryProvider asyncQueryProvider, IEntityType entityType) : base(asyncQueryProvider, entityType)
        {
            Type = typeof(IOrderedQueryable<>).MakeGenericType(entityType.ClrType);
        }

        public override Type Type { get; }
    }
}
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;

namespace EntityFrameworkCore.Testing.Common
{
    internal class QueryRootExpression : Microsoft.EntityFrameworkCore.Query.QueryRootExpression
    {
        public QueryRootExpression(IAsyncQueryProvider asyncQueryProvider, IEntityType entityType) : base(asyncQueryProvider, entityType)
        {
            Type = typeof(IOrderedQueryable<>).MakeGenericType(entityType.ClrType);
        }

        public override Type Type { get; }
    }
}
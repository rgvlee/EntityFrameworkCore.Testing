﻿using System.Linq;
using EntityFrameworkCore.Testing.Common.Tests;

namespace EntityFrameworkCore.Testing.NSubstitute.Tests
{
    public class ByTypeReadOnlyDbSetTests : BaseForDbQueryTests<TestReadOnlyEntity>
    {
        protected override IQueryable<TestReadOnlyEntity> Queryable => MockedDbContext.Set<TestReadOnlyEntity>();
    }
}
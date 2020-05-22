﻿using System.Linq;
using EntityFrameworkCore.Testing.Common.Tests;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.NSubstitute.Tests
{
    [TestFixture]
    public class ByPropertyDbSetTests : BaseForDbSetTests<TestEntity>
    {
        protected override IQueryable<TestEntity> Queryable => MockedDbContext.TestEntities;
    }
}
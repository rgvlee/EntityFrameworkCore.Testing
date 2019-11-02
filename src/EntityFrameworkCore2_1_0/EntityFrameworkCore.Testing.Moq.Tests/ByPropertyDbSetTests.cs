﻿using System.Linq;
using EntityFrameworkCore.Testing.Common.Tests;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Moq.Tests
{
    [TestFixture]
    public class ByPropertyDbSetTests : DbSetTestsBase
    {
        protected override IQueryable<TestEntity1> Queryable => MockedDbContext.TestEntities;
    }
}
﻿using System.Linq;
using EntityFrameworkCore.Testing.Common.Tests;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Moq.Tests
{
    [TestFixture]
    public class ByPropertyDbQueryTests : BaseForDbQueryTests<TestQuery>
    {
        protected override IQueryable<TestQuery> Queryable => MockedDbContext.TestView;
    }
}
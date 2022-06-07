﻿using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Common.Tests
{
    public abstract class Issue114Tests : BaseForTests
    {
        protected abstract TestDbContext MockedDbContextFactory();

        [Test]
        public void Any_ForReadOnlyEntityWithNoDbContextProperty_IsFalse()
        {
            var mockedContext = MockedDbContextFactory();

            mockedContext.Query<Foo>().Any().Should().BeFalse();
        }

        public class TestDbContext : DbContext
        {
            public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Query<Foo>().ToView("Foo");
            }
        }

        public class Foo
        {
            public string Bar { get; set; }
        }
    }
}
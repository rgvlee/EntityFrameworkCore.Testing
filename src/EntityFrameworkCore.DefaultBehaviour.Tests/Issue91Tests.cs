using System;
using System.Linq;
using EntityFrameworkCore.Testing.Common.Tests;
using NUnit.Framework;

namespace EntityFrameworkCore.DefaultBehaviour.Tests
{
    public class Issue91Tests : Testing.Common.Tests.Issue91Tests
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            DbContextFactory = () => new Issue91DbContext(DbContextOptions);
        }

        [Test]
        public override void SelectWithExpressionFunc_ReturnsSequence()
        {
            var dbContext = DbContextFactory();
            dbContext.Set<Foo>().AddRange(Foos);
            dbContext.SaveChanges();

            Assert.Throws<InvalidCastException>(() =>
            {
                var actualQuxs = dbContext.Set<Foo>().Select(Expression).ToList();
            });
        }

        [Test]
        public override void SelectWithAnonymousExpressionFunc_ReturnsSequence()
        {
            var dbContext = DbContextFactory();
            dbContext.Set<Foo>().AddRange(Foos);
            dbContext.SaveChanges();

            Assert.Throws<InvalidCastException>(() =>
            {
                var actualQuxs = dbContext.Set<Foo>().Select(foo => new Qux
                {
                    TotalWeight = foo.Bars.Sum(y => y.Weight),
                    Heights = foo.Bazs.Select(y => y.Weight)
                }).ToList();
            });
        }
    }
}